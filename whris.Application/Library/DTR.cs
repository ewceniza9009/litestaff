using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using whris.Application.CQRS.TrnDtr.Commands;
using whris.Application.Dtos;
using whris.Application.Queries.TrnDtr;
using whris.Data.Data;
using whris.Data.Models;
namespace whris.Application.Library
{
    public class DtrBatchProcessor
    {
        // In-memory caches for all the data we need.
        private readonly Dictionary<(int EmployeeId, DateTime Date), int> _changedShiftsLookup;
        private readonly Dictionary<int, int> _defaultEmployeeShiftsLookup;
        private readonly Dictionary<int, int> _fallbackEmployeeShiftsLookup;
        private readonly Dictionary<int, int> _employeeBranchLookup;
        private readonly Dictionary<(DateTime Date, int BranchId), int> _dayTypeLookup;

        // The constructor does ALL the database work one time.
        public DtrBatchProcessor(List<GetEmployees.Employee> employees, DateTime startDate, DateTime endDate, int? changeShiftId, HRISContext context)
        {
            var employeeIds = employees.Select(e => e.Id).ToList();

            // 1. Get all changed shifts for this batch in ONE query.
            if (changeShiftId.HasValue)
            {
                // This query uses a composite key (EmployeeId, Date), which is less likely to have duplicates,
                // but adding the GroupBy is a safe practice.
                _changedShiftsLookup = context.TrnChangeShiftLines
                    .Where(csl => csl.ChangeShiftId == changeShiftId && employeeIds.Contains(csl.EmployeeId) && csl.Date.Date >= startDate.Date && csl.Date.Date <= endDate.Date)
                    .GroupBy(csl => new { csl.EmployeeId, csl.Date.Date }) // Group by the composite key
                    .ToDictionary(g => (g.Key.EmployeeId, g.Key.Date), g => g.First().ShiftCodeId); // Take the first one in each group
            }
            else
            {
                _changedShiftsLookup = new Dictionary<(int, DateTime), int>();
            }

            // 2. Get all default employee shift codes, handling potential duplicates.
            _defaultEmployeeShiftsLookup = context.MstEmployeeShiftCodes
                .Where(esc => employeeIds.Contains(esc.EmployeeId))
                .GroupBy(esc => esc.EmployeeId) // <-- Group by the key first
                .ToDictionary(g => g.Key, g => g.First().ShiftCodeId); // <-- Then select the first item from each group

            // 3. Create a lookup for the fallback shift code, handling potential duplicates in the source list.
            _fallbackEmployeeShiftsLookup = employees
                .GroupBy(e => e.Id) // <-- Group by the key first
                .ToDictionary(g => g.Key, g => g.First().ShiftCodeId); // <-- Then select the first item

            // 4. Create a lookup for employee branches, handling potential duplicates.
            _employeeBranchLookup = employees
                .GroupBy(e => e.Id) // <-- Group by the key first
                .ToDictionary(g => g.Key, g => g.First().BranchId); // <-- Then select the first item

            // 5. Get all relevant day types in ONE query.
            var relevantBranchIds = employees.Select(e => e.BranchId).Distinct().ToList();
            _dayTypeLookup = context.MstDayTypeDays
                .Where(dtd => dtd.Date.Date >= startDate.Date && dtd.Date.Date <= endDate.Date && relevantBranchIds.Contains(dtd.BranchId))
                .GroupBy(dtd => new { dtd.Date.Date, dtd.BranchId }) // Group by the composite key
                .ToDictionary(g => (g.Key.Date, g.Key.BranchId), g => g.First().DayTypeId); // Take the first one
        }

        // This method is now extremely fast - it does NOT touch the database.
        public int GetShiftCode(int? changeShiftId, int employeeId, DateTime dtrDate)
        {
            if (changeShiftId.HasValue)
            {
                // First, try the change shift lookup
                if (_changedShiftsLookup.TryGetValue((employeeId, dtrDate.Date), out var shiftId))
                {
                    return shiftId;
                }
                // If not found, fall back to the default employee shift
                if (_defaultEmployeeShiftsLookup.TryGetValue(employeeId, out var defaultShiftId))
                {
                    return defaultShiftId;
                }
            }

            // Finally, use the fallback from the main employee record
            return _fallbackEmployeeShiftsLookup.GetValueOrDefault(employeeId, 0);
        }

        // This method is also extremely fast - it does NOT touch the database.
        public int GetDayType(int employeeId, DateTime dtrDate)
        {
            var branchId = _employeeBranchLookup.GetValueOrDefault(employeeId, 0);
            if (branchId == 0) return 1; // Default if employee has no branch

            if (_dayTypeLookup.TryGetValue((dtrDate.Date, branchId), out var dayTypeId))
            {
                return dayTypeId;
            }

            return 1; // Default Day Type Id
        }
    }

    public class DTR
    {
        //static HRISContext _context
        //{
        //    get => new HRISContext();
        //}
        static DateTime DefaultDate = new DateTime(1990, 09, 15);
        static DateOnly DefaultDateOnly = new DateOnly(1990, 09, 15);

        #region Assign Values
        public static int ComputeShiftCode(int? changeShiftId, int? employeeId, DateTime dtrDate, HRISContext _context)
        {
            var result = 0;

            if (changeShiftId is not null)
            {
                result = _context.TrnChangeShiftLines
                    ?.FirstOrDefault(x => x.ChangeShiftId == changeShiftId
                        && x.EmployeeId == employeeId
                        && x.Date.Date == dtrDate.Date)
                    ?.ShiftCodeId ?? 0;

                if (result > 0)
                {
                    return result;
                }
                else
                {
                    return _context.MstEmployeeShiftCodes
                        ?.FirstOrDefault(x => x.EmployeeId == employeeId)
                        ?.ShiftCodeId ?? 0;
                }
            }

            result = _context.MstEmployees
                    ?.FirstOrDefault(x => x.Id == employeeId)
                    ?.ShiftCodeId ?? 0;

            return result;
        }

        public static bool ComputeRestDay(TrnDtrline line, IEnumerable<MstShiftCodeDay> shiftCodeDays) 
        {
            var shiftCodeDay = shiftCodeDays
                .Where(x => x.ShiftCodeId == line.ShiftCodeId && 
                    x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                .ToList();

            if (shiftCodeDay.Any())
            {
                return shiftCodeDay.FirstOrDefault()?.RestDay ?? false;
            }

            return false;
        } 

        public static bool ComputeOnLeave(TrnDtrline line, HRISContext _context) 
        {
            var result = false;
            var leaveApplicationId = _context.TrnDtrs.FirstOrDefault(x => x.Id == line.Dtrid)?.LeaveApplicationId ?? 0;   
            
            if(leaveApplicationId != 0 && line.EmployeeId != 0) 
            {
                var leaveApplication = _context.TrnLeaveApplicationLines
                    .Where(x => x.LeaveApplicationId == leaveApplicationId &&
                        x.EmployeeId == line.EmployeeId &&
                        x.Date.Date == line.Date.Date);


                if (leaveApplication?.Any() ?? false) 
                {
                    return true;
                }
            }

            return result;
        }

        public static bool ComputeAbsent(TrnDtrline line, HRISContext _context) 
        {
            //Absent
            if (line.TimeIn1 is null &&
                line.TimeOut1 is null &&
                line.TimeIn2 is null &&
                line.TimeIn2 is null &&
                !line.OfficialBusiness &&
                !line.OnLeave &&
                !line.RestDay &&
                line.DayTypeId == 1)
            {
                return true;
            }

            //Leave with/without Pay = Absent
            if (line.TimeIn1 is null && 
                line.TimeOut1 is null && 
                line.TimeIn2 is null && 
                line.TimeIn2 is null && 
                !line.OfficialBusiness &&
                line.OnLeave && 
                !line.RestDay && 
                line.DayTypeId == 1)
            {
                //return !(_context.TrnLeaveApplicationLines
                //    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId && 
                //        x.Date.Date == line.Date.Date)?.WithPay ?? false);
                var isWithPay = _context.TrnLeaveApplicationLines
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId &&
                        x.Date.Date == line.Date.Date)?.WithPay ?? false;

                if (!isWithPay)
                {
                    var payrollTypeId = _context.MstEmployees
                       ?.FirstOrDefault(x => x.Id == line.EmployeeId)
                       ?.PayrollTypeId ?? 0;

                    if (payrollTypeId == 2)
                    {
                        return false;
                    }
                    else 
                    {
                        return true;
                    }
                }
                else 
                {
                    return false;
                }
               
            }

            //Absent on holiday
            if (line.TimeIn1 is null &&
                line.TimeOut1 is null &&
                line.TimeIn2 is null &&
                line.TimeIn2 is null &&
                !line.OfficialBusiness &&
                !line.OnLeave &&
                !line.RestDay &&
                line.DayTypeId != 1) 
            {
                var payrollTypeId = _context.MstEmployees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

                if (payrollTypeId == 2) 
                {
                    return _context.MstDayTypeDays
                        .FirstOrDefault(x => x.Date.Date == line.Date.Date)
                        ?.WithAbsentInFixed ?? false;
                }
            }

            return false;
        }

        public static int ComputeDayType(int? employeeId, DateTime dtrDate, HRISContext _context)
        {
            var result = 0;
            var branchId = _context.MstEmployees.FirstOrDefault(x => x.Id == employeeId)?.BranchId ?? 0;

            result = _context.MstDayTypeDays
                ?.FirstOrDefault(x => x.Date.Date == dtrDate.Date
                    && x.BranchId == branchId)
                ?.DayTypeId ?? 1;

            return result;
        }

        public static decimal ComputeRegularHours(TrnDtrline line, IEnumerable<MstShiftCodeDay> shiftCodeDays, bool isEligibleForHolidayPay, HRISContext _context) 
        {
            if (DateOnly.FromDateTime(line.Date) == DateOnly.Parse("06/18/2025"))
            {

            }

            var leaveApplicationId = 0;
            var leaveWithPay = false;
            var leaveStatus = false;
            var leaveWithHours = 0m;

            var dayType = 0;
            var branchId = 0;
            var dateAfterHoliday = DefaultDate;
            var dateBeforeHoliday = DefaultDate;

            var shiftCodeDay = shiftCodeDays
                ?.FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                    x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper());

            if (shiftCodeDay is not null)
            {
                if (line.TimeIn1 is not null ||
                    line.TimeOut1 is not null ||
                    line.TimeIn2 is not null ||
                    line.TimeOut2 is not null)
                {
                    if (line.TimeIn1 == null && line.TimeOut1 == null) 
                    {
                        line.HalfdayAbsent = true;
                        return shiftCodeDay.NumberOfHours / 2;
                    }

                    if (line.TimeIn2 == null && line.TimeOut2 == null)
                    {
                        line.HalfdayAbsent = true;
                        return shiftCodeDay.NumberOfHours / 2;
                    }

                    return shiftCodeDay.NumberOfHours;
                }
                else 
                {
                    if (isEligibleForHolidayPay && line.DayTypeId > 1 && line.RestDay) 
                    {
                        var dayTypeId = line?.DayTypeId ?? 1;

                        if (!(line?.Absent ?? true) && dayTypeId == 3 & (line?.TimeIn1 is null && line?.TimeOut1 is null && line?.TimeIn2 is null && line?.TimeOut2 is null)) 
                        {
                            return 0;
                        }

                        return shiftCodeDay.NumberOfHours;
                    }

                    leaveApplicationId = _context.TrnDtrs
                        .FirstOrDefault(x => x.Id == line.Dtrid)
                        ?.LeaveApplicationId ?? 0;

                    if (leaveApplicationId != 0 && line.EmployeeId != 0) 
                    {
                        var leaveApplication = _context.TrnLeaveApplicationLines
                            .FirstOrDefault(x => x.LeaveApplicationId == leaveApplicationId &&
                                x.EmployeeId == line.EmployeeId &&
                                x.Date.Date == line.Date.Date);

                        if (leaveApplication is not null)
                        {
                            leaveWithPay = leaveApplication.WithPay;
                            leaveWithHours = leaveApplication.NumberOfHours;
                            leaveStatus = true;
                        }
                        else 
                        {
                            leaveStatus = false;
                        }
                    }

                    if (leaveStatus && leaveWithPay)
                    {
                        return leaveWithHours;
                    }

                    var payrollTypeId = _context.MstEmployees
                       ?.FirstOrDefault(x => x.Id == line.EmployeeId)
                       ?.PayrollTypeId ?? 0;

                    branchId = _context.MstEmployees
                        ?.FirstOrDefault(x => x.Id == line.EmployeeId)
                        ?.BranchId ?? 0;

                    var dayTypeDay = _context.MstDayTypeDays
                        .FirstOrDefault(x => x.Date.Date == line.Date.Date &&
                            x.BranchId == branchId);

                    if (payrollTypeId == 3)
                    {
                        var dayTypeId = dayTypeDay?.DayTypeId ?? 1;

                        if (line != null && (line.TimeIn1 is null && line.TimeOut1 is null && line.TimeIn2 is null && line.TimeOut2 is null) && dayTypeId > 1)
                        {
                            return 0;
                        }
                    }

                    if (true) //if (payrollTypeId == 1) //For now lets just comment this
                    {
						if (dayTypeDay is not null)
                        {
                            dayType = dayTypeDay.DayTypeId;
                            dateAfterHoliday = dayTypeDay.DateAfter;
                            dateBeforeHoliday = dayTypeDay.DateBefore;
                        }

                        if (dayType == 3 && line.TimeIn1 is null && line.TimeOut1 is null && line.TimeIn2 is null && line.TimeOut2 is null)
                        {
                            return 0;
                        }

                        if (dayType == 2 || dayType == 3)
                        {
                            var before = _context.TrnDtrlines.Any(x => x.EmployeeId == line.EmployeeId &&
                                x.Date.Date == dateBeforeHoliday.Date &&
                                !x.Absent);

                            var after = _context.TrnDtrlines.Any(x => x.EmployeeId == line.EmployeeId &&
                               x.Date.Date == dateAfterHoliday.Date &&
                               !x.Absent);

                            var isTrue = before && after;

                            if (isTrue)
                            {
                                if (line != null && line.RestDay && line.TimeIn1 == null && line.TimeOut1 == null && line.TimeIn2 == null && line.TimeOut2 == null) 
                                {
                                    return 0;
                                }

                                return shiftCodeDay.NumberOfHours;
                            }
                        }
                    }
                }
            }            

            //if (shiftCodeDay is not null &&
            //    shiftCodeDay.TimeIn1 is not null &&
            //    shiftCodeDay.TimeOut1 is not null &&
            //    shiftCodeDay.TimeIn2 is not null &&
            //    shiftCodeDay.TimeOut2 is not null)
            //{
            //    if (line.TimeIn1 is not null || 
            //        line.TimeOut1 is not null || 
            //        line.TimeIn2 is not null ||
            //        line.TimeOut2 is not null) 
            //    {
            //        return shiftCodeDay?.NumberOfHours ?? 0;
            //    }
            //}
            //else if(shiftCodeDay is not null &&
            //    line.TimeIn1 is not null &&
            //    line.TimeOut2 is not null)
            //{
            //    return shiftCodeDay?.NumberOfHours ?? 0;
            //}

            return 0;
        }

        public static decimal ComputeNightHours(TrnDtrline line, IEnumerable<MstShiftCodeDay> shiftCodeDays) 
        {
            if (line.Absent) 
            {
                return 0;
            }

            var nightTimeStart = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} 10:00 PM");
            var nightTimeEnd = DateTime.Parse($"{line.Date.AddDays(1).ToString("MM/dd/yyyy")} 6:00 AM");

            var tIn1 = TimeOnly.FromDateTime(line.TimeIn1 ?? DefaultDate);
            var tIn2 = TimeOnly.FromDateTime(line.TimeIn2 ?? DefaultDate);

            var tIn1Date = (line.TimeIn1 ?? DefaultDate).Date;
            var tOut2Date = (line.TimeOut2 ?? DefaultDate).Date;

            var nightTimeOnlyStart = TimeOnly.FromDateTime(nightTimeStart);
            var nightTimeOnlyEnd = TimeOnly.FromDateTime(nightTimeEnd);

            var is4swipes = line?.TimeIn1 != null && line?.TimeOut1 != null && line?.TimeIn2 != null && line?.TimeOut2 != null;
            var is2swipes = line?.TimeIn1 != null && line?.TimeOut2 != null;

            //if (tIn1Date == tOut2Date && tIn1 < nightTimeOnlyStart && tIn2 < nightTimeOnlyStart && is4swipes)
            //{
            //    nightTimeStart = nightTimeStart.AddDays(-1);
            //    nightTimeEnd = nightTimeEnd.AddDays(-1);
            //}

            //if (((tIn1Date != nightTimeStart.Date && tIn1 < nightTimeOnlyStart) || (tIn1Date == nightTimeStart.Date && tIn1 < nightTimeOnlyEnd)) && is2swipes)
            //{
            //    nightTimeStart = nightTimeStart.AddDays(-1);
            //    nightTimeEnd = nightTimeEnd.AddDays(-1);
            //}

            if (!is2swipes && !is4swipes) 
            {
                return 0;
            }

            if (line is not null && tIn1Date < line.Date) 
            {
                nightTimeStart = nightTimeStart.AddDays(-1);
                nightTimeEnd = nightTimeEnd.AddDays(-1);
            }

            if (line is not null && tIn1Date > line.Date)
            {
                nightTimeStart = nightTimeStart.AddDays(1);
                nightTimeEnd = nightTimeEnd.AddDays(1);
            }

            var stOut1 = shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == line?.ShiftCodeId && 
                    x.Day == line.Date.ToString("dddd"))
                ?.TimeOut1 ?? DefaultDate;
            
            var stIn2 = shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == line?.ShiftCodeId && 
                    x.Day == line.Date.ToString("dddd"))
                ?.TimeIn2 ?? DefaultDate;

            var stOut1Date = line?.TimeOut1 ?? line?.Date ?? DefaultDate;
            var stIn2Date = line?.TimeIn2 ?? line?.Date ?? DefaultDate;

            if (line?.TimeIn2 is not null) 
            {
                stIn2Date = line?.TimeIn2 ?? DefaultDate;
            }

            var shiftTimeOut1 = DateTime.Parse($"{stOut1Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", stOut1)} ");
            var shiftTimeIn2 = DateTime.Parse($"{stIn2Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", stIn2)} ");

            var actualNightTimeStart = line?.TimeIn1 ?? DefaultDate;
            var actualNightTimeEnd = line?.TimeOut2 ?? DefaultDate;

            var numberOfHours = 0m;

            //if (line is not null && line.TimeIn1 >= nightTimeStart || (line is not null && line.TimeIn1 < nightTimeStart && line.TimeOut2 > nightTimeStart)) 
            //{
            //    if (line.TimeIn1 >= nightTimeStart && line.TimeIn1 > nightTimeEnd)
            //    {
            //        actualNightTimeStart = line.TimeIn1 ?? DefaultDate;
            //    }
            //    else 
            //    {
            //        if (line.TimeIn1 < nightTimeStart)
            //        {
            //            actualNightTimeStart = nightTimeStart;
            //        }
            //        else 
            //        {
            //            if (line?.TimeIn2 != null && line.TimeIn2 > nightTimeStart)
            //            {
            //                if (line.TimeIn2 > shiftTimeIn2)
            //                {
            //                    actualNightTimeStart = line?.TimeIn2 ?? DefaultDate;
            //                }
            //                else
            //                {
            //                    actualNightTimeStart = shiftTimeIn2;
            //                }
            //            }
            //        }
            //    }

            //    if (line?.TimeOut2 != null && line.TimeOut2 <= nightTimeEnd && line.TimeOut2 > nightTimeStart)
            //    {
            //        actualNightTimeEnd = line.TimeOut2 ?? DefaultDate;
            //    }
            //    else 
            //    {
            //        actualNightTimeEnd = nightTimeEnd;
            //    }

            //    numberOfHours = (decimal)(actualNightTimeEnd - actualNightTimeStart).TotalHours;
            //}

            if (line is not null && line.TimeIn1 >= nightTimeStart && line.TimeIn1 > nightTimeEnd)
            {
                actualNightTimeStart = line.TimeIn1 ?? DefaultDate;
                
            }
            else
            {
                if (line is not null && line.TimeIn1 < nightTimeStart)
                {
                    actualNightTimeStart = nightTimeStart;

                    var isLongShift = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours > 20;
                    if (isLongShift)
                    {
                        actualNightTimeStart = line.TimeIn1 ?? DefaultDate;
                    }
                }
                else
                {
                    if (line?.TimeIn2 != null && line.TimeIn2 > nightTimeStart)
                    {
                        if (line.TimeIn2 > shiftTimeIn2)
                        {
                            actualNightTimeStart = line?.TimeIn2 ?? DefaultDate;
                        }
                        else
                        {
                            actualNightTimeStart = shiftTimeIn2;
                        }
                    }
                }
            }

            if (line?.TimeOut2 != null && line.TimeOut2 <= nightTimeEnd && line.TimeOut2 > nightTimeStart)
            {
                actualNightTimeEnd = line.TimeOut2 ?? DefaultDate;
            }
            else
            {
                actualNightTimeEnd = nightTimeEnd;
            }

            numberOfHours = (decimal)(actualNightTimeEnd - actualNightTimeStart).TotalHours;

            var s = line?.ShiftCodeId ?? 0;
            var d = line?.Date.ToString("dddd") ?? "NA" ;

            var nighHours = shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == s && x.Day.ToUpper() == d.ToUpper())
                ?.NightHours ?? 0;

            if (numberOfHours > nighHours) 
            {
                numberOfHours = nighHours;
            }

            if (numberOfHours < 0) 
            {
                numberOfHours = 0;
            }

            return Math.Round(numberOfHours, 5);
        }

        public static decimal ComputeOverTimeHours(TrnDtrline line, HRISContext _context)
        {
            //var regHours = 0m;
            var oTHours = 0m;
            var oTLimitHours = 0m;
            //var actualOTHours = 0m;
            //var actualOTHoursOnIn = 0m;
            //var shiftTimeIn1  = DefaultDate;
            //var shiftTimeOut2 = DefaultDate;

            var overTimeId = line.Dtr?.OvertTimeId ?? 0;

            oTHours = _context.TrnOverTimeLines
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId &&
                    x.OverTimeId == overTimeId &&
                    x.Date.Date == line.Date.Date)
                ?.OvertimeHours ?? 0;

            oTLimitHours = _context.TrnOverTimeLines
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId &&
                    x.OverTimeId == overTimeId &&
                    x.Date.Date == line.Date.Date)
                ?.OvertimeLimitHours ?? 0;

            if (oTHours > 0) 
            {
                if (oTHours > oTLimitHours) 
                {
                    oTHours = oTLimitHours;
                }

                //if (line.TimeOut2 is not null)
                //{
                //    if (line.RestDay)
                //    {
                //        regHours = _context.MstShiftCodeDays
                //        .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                //            x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                //        ?.NumberOfHours ?? 0;

                //        if (line.TimeIn1 is not null && line.TimeOut2 is not null)
                //        {
                //            actualOTHoursOnIn = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours;

                //            if (actualOTHoursOnIn > 0)
                //            {
                //                actualOTHours = actualOTHoursOnIn;
                //            }
                //            else
                //            {
                //                actualOTHours = 0;
                //            }
                //        }
                //        else
                //        {
                //            actualOTHours = 0;
                //        }

                //        if (actualOTHours > regHours)
                //        {
                //            actualOTHours = actualOTHours - regHours;
                //        }
                //        else
                //        {
                //            actualOTHours = 0;
                //        }

                //        if (oTHours > regHours)
                //        {
                //            oTHours = oTHours - regHours;

                //            if (oTHours > actualOTHours)
                //            {
                //                oTHours = actualOTHours;
                //            }
                //        }
                //        else
                //        {
                //            oTHours = 0;
                //        }
                //    }
                //    else
                //    {
                //        var tOut2 = (_context.MstShiftCodeDays
                //            .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                //                x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                //            ?.TimeOut2 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                //            .ToString("HH:mm tt");

                //        shiftTimeOut2 = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {tOut2}");

                //        if (shiftTimeOut2 != DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                //        {
                //            actualOTHours = (decimal)((line.TimeOut2 ?? DefaultDate) - shiftTimeOut2).TotalHours;

                //            if (actualOTHours <= 0)
                //            {
                //                actualOTHours = 0;
                //            }
                //            else
                //            {
                //                if (line.TimeIn1 is not null)
                //                {
                //                    var tIn1 = (_context.MstShiftCodeDays
                //                        .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                //                            x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                //                        ?.TimeIn1 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                //                        .ToString("HH:mm tt");
                //                    shiftTimeIn1 = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {tIn1}");

                //                    if (shiftTimeIn1 != DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                //                    {
                //                        actualOTHoursOnIn = (decimal)(shiftTimeIn1 - (line.TimeIn1 ?? DefaultDate)).TotalHours;

                //                        if (actualOTHoursOnIn > 0)
                //                        {
                //                            // If DFirst("IncludeTimeInOT", "SysCurrent") = True Then
                //                            actualOTHours = actualOTHours + actualOTHoursOnIn;
                //                        }
                //                    }
                //                }
                //            }
                //        }

                //        if (actualOTHours <= 0)
                //        {
                //            actualOTHours = 0;
                //        }

                //        if (oTHours > actualOTHours)
                //        {
                //            if (oTLimitHours > 0)
                //            {
                //                oTHours = oTLimitHours * ((int)(actualOTHours / oTLimitHours));
                //            }
                //            else
                //            {
                //                oTHours = actualOTHours;
                //            }
                //        }
                //    }
                //}
                //else 
                //{
                //    oTHours = 0;
                //}

                if (line.TimeOut2 is null)
                {
                    oTHours = 0;
                }
            }

            return Math.Round(oTHours, 5);
        }

        public static decimal ComputeOvertimeNightHours(TrnDtrline line, HRISContext _context) 
        {
            var otHours = 0m;
            var totalWorkHours = 0m;

            var overTimeId = line.Dtr?.OvertTimeId ?? 0;

            otHours = _context.TrnOverTimeLines
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId &&
                    x.OverTimeId == overTimeId && x.Date.Date == line.Date.Date)
                ?.OvertimeNightHours ?? 0;

            if (otHours > 0) 
            {
                totalWorkHours = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours;
                totalWorkHours = totalWorkHours < 0 ? 0 : totalWorkHours;

                if (otHours > totalWorkHours) 
                {
                    otHours = totalWorkHours;
                } 
            }

            return Math.Round(otHours, 5);
        }

        public static decimal ComputeGrossTotalHours(TrnDtrline line) 
        {
            return line.RegularHours + line.OvertimeHours + line.OvertimeNightHours;
        }

        public static decimal ComputeTardyLateHours(TrnDtrline line, IEnumerable<MstShiftCodeDay> shiftCodeDays, IEnumerable<MstEmployee> employees, HRISContext _context) 
        {
            if (DateOnly.FromDateTime(line.Date) == DateOnly.Parse("10/24/2024"))
            {

            }

            var numberOfHours = 0m;

            var datTimeIn1 = (shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                    x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                ?.TimeIn1 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                .ToString("HH:mm tt");
            var shiftTimeIn1 = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {datTimeIn1}");

            var datTimeIn2 = (shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                    x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                ?.TimeIn2 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                .ToString("HH:mm tt");
            var shiftTimeIn2 = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {datTimeIn2}");

            var datTimeOut1 = (shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                    x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                ?.TimeOut1 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                .ToString("HH:mm tt");
            var shiftTimeOut1 = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {datTimeOut1}");

            var numberOfGraceMinutes = shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                    x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                ?.LateGraceMinute ?? 0;

            var numberOfFlexHours = shiftCodeDays
               .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                   x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
               ?.LateFlexibility ?? 0;

            var payrollTypeId = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

            shiftTimeIn1 = shiftTimeIn1.AddMinutes((double)numberOfGraceMinutes);
            shiftTimeIn2 = shiftTimeIn2.AddMinutes((double)numberOfGraceMinutes);

            var isSCContainsIsTommorow = shiftCodeDays.Any(x => x.ShiftCodeId == line.ShiftCodeId && x.IsTommorow == true);

            if (isSCContainsIsTommorow) 
            {
                var shiftTimeIn1InShiftDates = DefaultDate;                

                if (line.ShiftDates is not null && line.ShiftDates.Length > 0)
                {
					var shiftDates = line.ShiftDates.Split(",");

                    shiftTimeIn1InShiftDates = DateTime.Parse(shiftDates[0]);
				}

				if (shiftTimeIn1.Date != line.Date.Date || (line.ShiftDates is not null && shiftTimeIn1InShiftDates.Date != line.Date.Date)) 
                {
                    shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
                }                
            }

            if (line?.TimeIn2 is not null) 
            {
                var a = TimeOnly.FromDateTime(DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", line.TimeIn1)}"));
                var b = TimeOnly.FromDateTime(DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", line.TimeIn2)}"));

                if (a > b)
                {
                    shiftTimeIn2 = shiftTimeIn2.AddDays(1);
                }
            }

            if (line?.TimeIn1 is not null && shiftTimeIn1 != DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
            {
                var diff = (decimal)((line?.TimeIn1 ?? DefaultDate) - shiftTimeIn1).TotalHours;

                if (diff < -20)
                {
                    shiftTimeIn1 = shiftTimeIn1.AddDays(-1);

                    numberOfHours = numberOfHours + (decimal)((line?.TimeIn1 ?? DefaultDate) - shiftTimeIn1).TotalHours;
                }
                else if (diff > 20) 
                {
                    shiftTimeIn1 = shiftTimeIn1.AddDays(1);

                    numberOfHours = numberOfHours + (decimal)((line?.TimeIn1 ?? DefaultDate) - shiftTimeIn1).TotalHours;
                }
                else
                {
                    numberOfHours = numberOfHours + (decimal)((line?.TimeIn1 ?? DefaultDate) - shiftTimeIn1).TotalHours;
                }
            }
            else
            {
                if (line?.TimeIn1 is not null && line.TimeOut1 is not null && line.TimeIn2 is not null && line.TimeOut2 is not null)
                {
                    if (shiftTimeOut1 != DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                    {
                        shiftTimeOut1 = shiftTimeOut1.AddMinutes((double)numberOfGraceMinutes);
                        numberOfHours = numberOfHours + (decimal)((line?.TimeIn1 ?? DefaultDate) - shiftTimeOut1).TotalHours;
                    }
                }
            }

            if (numberOfHours < 0) 
            {
                numberOfHours = 0;
            }

            //if (line?.TimeIn2 is not null && shiftTimeIn1 != DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
            //{
            //    var diff = (decimal)((line?.TimeIn2 ?? DefaultDate) - shiftTimeIn2).TotalHours;

            //    numberOfHours = numberOfHours + (diff < 0 ? 0 : diff);
            //}

            if (payrollTypeId == 2) 
            {
                numberOfHours = 0;
            }

            if (line?.OnLeave ?? false)
            {
                var leaveApplicationId = _context.TrnDtrs
                    .FirstOrDefault(x => x.Id == line.Dtrid)
                    ?.LeaveApplicationId ?? 0;

                var leaveWithPay = false;
                var leaveWithHours = 0m;

                if (leaveApplicationId != 0 && line.EmployeeId != 0)
                {
                    var leaveApplication = _context.TrnLeaveApplicationLines
                        .FirstOrDefault(x => x.LeaveApplicationId == leaveApplicationId &&
                            x.EmployeeId == line.EmployeeId &&
                            x.Date.Date == line.Date.Date);

                    if (leaveApplication is not null)
                    {
                        leaveWithPay = leaveApplication.WithPay;
                        leaveWithHours = leaveApplication.NumberOfHours;
                    }
                }

                if (leaveWithPay && !line.Absent)
                {
                    if (numberOfHours > leaveWithHours)
                    {
                        numberOfHours = numberOfHours - leaveWithHours;
                    }
                    else
                    {
                        numberOfHours = 0;
                    }
                }
                else
                {
                    if (numberOfHours < 0)
                    {
                        numberOfHours = Math.Round(numberOfHours, 0);
                    }
                    else
                    {
                        numberOfHours = Math.Abs(Math.Round(numberOfHours, 5));
                    }
                }
            }
            else 
            {
                if (numberOfHours > 0 && !(line?.Absent ?? false))
                {
                    if (numberOfFlexHours > 0)
                    {
                        numberOfHours = 0;
                    }
                    else
                    {
                        numberOfHours = Math.Round(numberOfHours, 5);
                    }
                }
                else 
                {
                    numberOfHours = 0;
                }
            }

            if (numberOfHours > 0)
            {
                var empId = line?.EmployeeId ?? 0;
                var isEmpFlex = employees.FirstOrDefault(x => x.Id == empId)?.IsFlex ?? false;

                if (isEmpFlex)
                {
                    return 0;
                }

                return Math.Round(numberOfHours, 5);
            }            

            return 0;
        }
  
        public static decimal ComputeTardyUndertimeHours(TrnDtrline line, IEnumerable<MstShiftCodeDay> shiftCodeDays, HRISContext _context) 
        {
            if (DateOnly.FromDateTime(line.Date) == DateOnly.Parse("05/03/2025"))
            {

            }

            var numberOfHours = 0m;
            var actualNumberOfHours = 0m;
            var shiftTimeIn1 = DefaultDate;
            var shiftTimeIn2 = DefaultDate;
            var shiftTimeOut2 = DefaultDate;
            //var shiftTimeOut1 = DefaultDate;
            var shiftNumberOfHours = 0m;
            var shiftCodeId = 0;

            if (!line.Absent) 
            {
                //shiftCodeId = line?.ShiftCodeId ?? 0;

                shiftCodeId = line?.ShiftCodeId ?? 0;
                var lineDay = line?.Date.ToString("dddd")?.ToUpper() ?? "NA";

                shiftNumberOfHours = shiftCodeDays
                    .FirstOrDefault(x => x.ShiftCodeId == shiftCodeId && x.Day.ToUpper() == lineDay)
                    ?.NumberOfHours ?? 0;

                var tIn1 = (shiftCodeDays
                    .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                        x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                    ?.TimeIn1 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                    .ToString("HH:mm tt");
                shiftTimeIn1 = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {tIn1}");

                var tIn2 = (shiftCodeDays
                    .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                        x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                    ?.TimeIn2 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                    .ToString("HH:mm tt");
                shiftTimeIn2 = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {tIn2}");

                var tOut2 = (shiftCodeDays
                    .FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
                        x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper())
                    ?.TimeOut2 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                    .ToString("HH:mm tt");
                shiftTimeOut2 = DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {tOut2}");

                var isFlexBreak = _context.MstEmployees?.FirstOrDefault(x => x.Id == line.EmployeeId)?.IsFlexBreak ?? false;

                if (isFlexBreak)
                {
                    if (line?.TimeIn1 == null && line?.TimeOut1 == null && line?.TimeIn2 == null && line?.TimeOut2 == null)
                    {
                        return Math.Round(numberOfHours, 5);
                    }

                    var timeIn1 = shiftTimeIn1;
                    var timeOut2 = line?.TimeOut2 ?? DefaultDate;

                    var timeOut1 = line?.TimeOut1 ?? DefaultDate;
                    var timeIn2 = line?.TimeIn2 ?? DefaultDate;

                    if (timeOut1 == DefaultDate && timeIn2 != DefaultDate) 
                    {
                        timeOut1 = timeIn2.AddHours(-1);
                    }

                    if (timeIn2 == DefaultDate && timeOut1 != DefaultDate)
                    {
                        timeIn2 = timeOut1.AddHours(1);
                    }

                    if (timeOut2 > shiftTimeOut2) 
                    {
                        timeOut2 = shiftTimeOut2;
                    }

                    var breakHours = (decimal)(timeIn2 - timeOut1).TotalHours;
                    var deductHours = 0m;

                    //if (breakHours < 1)
                    //{
                    //    deductHours = 1 - breakHours;
                    //}

                    var firstHalfInHours = (decimal)(timeOut1 - timeIn1).TotalHours;
                    var secondHalfInHours = (decimal)(timeOut2 - timeIn2).TotalHours;                    

                    actualNumberOfHours = firstHalfInHours + secondHalfInHours;

                    var shiftCodeSetup = _context.MstShiftCodes
                            .FirstOrDefault(x => x.Id == shiftCodeId);
                    var isStraight = shiftCodeSetup?.Remarks?.ToUpper()?.Contains("STRAIGHT") ?? false;

                    if (timeOut1 == DefaultDate && timeIn2 == DefaultDate && isStraight)
                    {
                        actualNumberOfHours = (decimal)(timeOut2 - timeIn1).TotalHours;
                    }

                    if (timeOut1 == DefaultDate && timeIn2 == DefaultDate && !isStraight)
                    {
                        var halfDayHours = shiftNumberOfHours / 2;
                        var halfDayActualHours = (decimal)(shiftTimeOut2 - timeIn1).TotalHours;
                        actualNumberOfHours = halfDayHours < halfDayActualHours ? halfDayHours : halfDayActualHours;
                    }

                    numberOfHours = shiftNumberOfHours - (actualNumberOfHours - deductHours);
                }
                else 
                {

                    if (line?.TimeIn2 is not null)
                    {
                        var a = TimeOnly.FromDateTime(DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", line.TimeIn1)}"));
                        var b = TimeOnly.FromDateTime(DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", line.TimeIn2)}"));

                        if (a > b)
                        {
                            shiftTimeIn2 = shiftTimeIn2.AddDays(1);
                        }
                    }

                    shiftCodeId = line?.ShiftCodeId ?? 0;

                    var lineDayStr = line?.Date.ToString("dddd")?.ToUpper() ?? "NA";
                    var lateFlexibility = shiftCodeDays
                        .FirstOrDefault(x => x.ShiftCodeId == shiftCodeId &&
                            x.Day.ToUpper() == lineDayStr)
                        ?.LateFlexibility ?? 0;

                    var empId = line?.EmployeeId ?? 0;
                    var payrollTypeId = _context.MstEmployees.FirstOrDefault(x => x.Id == empId)?.PayrollTypeId ?? 0;                    

                    if (line?.TimeIn1 is not null && line.TimeOut1 is not null && line.TimeIn2 is not null && line.TimeOut2 is not null)
                    {
                        var time1 = (decimal)((line.TimeOut1 ?? DefaultDate) - shiftTimeIn1).TotalHours; //(line.TimeIn1 ?? DefaultDate)).TotalHours;
                        var time2 = (decimal)((line.TimeOut2 ?? DefaultDate) - shiftTimeIn2).TotalHours; //(line.TimeIn2 ?? DefaultDate)).TotalHours;

                        if (lateFlexibility > 0)
                        {
                            time1 = (decimal)((line.TimeOut1 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours;
                            time2 = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn2 ?? DefaultDate)).TotalHours;
                        }

                        actualNumberOfHours = time1 + time2;
                        numberOfHours = shiftNumberOfHours - actualNumberOfHours;
                    }
                    else
                    {
                        if (line?.TimeIn1 is not null && line?.TimeOut2 is not null)
                        {
                            //Add a break 
                            shiftNumberOfHours += 1;

                            actualNumberOfHours = (decimal)((line.TimeOut2 ?? DefaultDate) - shiftTimeIn1).TotalHours;

                            if ((line.TimeOut2 ?? DefaultDate) > shiftTimeOut2)
                            {
                                actualNumberOfHours = (decimal)(shiftTimeOut2 - shiftTimeIn1).TotalHours;
                            }

                            if (actualNumberOfHours < 0)
                            {
                                var isSCContainsIsTommorow = shiftCodeDays.Any(x => x.ShiftCodeId == shiftCodeId && x.IsTommorow == true);

                                if (isSCContainsIsTommorow)
                                {
                                    shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
                                    actualNumberOfHours = (decimal)((line.TimeOut2 ?? DefaultDate) - shiftTimeIn1).TotalHours;
                                }
                            }

                            if (lateFlexibility > 0)
                            {
                                actualNumberOfHours = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours;
                            }
                        }
                    }

                    if (actualNumberOfHours < shiftNumberOfHours)
                    {
                        var isFlex = _context.MstEmployees?.FirstOrDefault(x => x.Id == empId)?.IsFlex ?? false;

                        if (isFlex)
                        {
                            numberOfHours = shiftNumberOfHours - actualNumberOfHours;

                            if (actualNumberOfHours == 0)
                            {
                                numberOfHours = 0;
                            }
                        }

                        //This code prevents undertime computation when timeIn1 is greater shifttTimeIn1, no need to write this
                        //if (shiftTimeIn1 >= line?.TimeIn1) 
                        //{
                        //    numberOfHours = shiftNumberOfHours - actualNumberOfHours;
                        //}

                        else
                        {
                            numberOfHours = shiftNumberOfHours - actualNumberOfHours;

                            if (actualNumberOfHours == 0)
                            {
                                numberOfHours = 0;
                            }

                            if (line?.TimeOut1 is null && line?.TimeIn2 is null)
                            {
                                if (line is not null && line?.TimeIn1 is not null && line?.TimeOut2 is not null)
                                {
                                    var totalHoursWorked = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours;

                                    if (totalHoursWorked <= ((shiftNumberOfHours - 1) / 2))
                                    {
                                        numberOfHours = (shiftNumberOfHours - 1) - actualNumberOfHours;
                                    }
                                }

                            }
                        }
                    }

                    if (numberOfHours < 0)
                    {
                        numberOfHours = 0;
                    }

                    if (payrollTypeId == 2)
                    {
                        numberOfHours = 0;
                    }
                }
            }

            if (line?.OnLeave ?? false)
            {
                var leaveApplicationId = _context.TrnDtrs
                    .FirstOrDefault(x => x.Id == line.Dtrid)
                    ?.LeaveApplicationId ?? 0;

                var leaveWithPay = false;
                var leaveWithHours = 0m;

                if (leaveApplicationId != 0 && line.EmployeeId != 0)
                {
                    var leaveApplication = _context.TrnLeaveApplicationLines
                        .FirstOrDefault(x => x.LeaveApplicationId == leaveApplicationId &&
                            x.EmployeeId == line.EmployeeId &&
                            x.Date.Date == line.Date.Date);

                    if (leaveApplication is not null)
                    {
                        leaveWithPay = leaveApplication.WithPay;
                        leaveWithHours = leaveApplication.NumberOfHours;
                    }
                }

                if (leaveWithPay && !line.Absent)
                {
                    if (numberOfHours > leaveWithHours)
                    {
                        numberOfHours = numberOfHours - leaveWithHours;
                    }
                    else
                    {
                        numberOfHours = 0;
                    }
                }
                else
                {
                    if (line?.RegularHours is null || line?.RegularHours == 0)
                    {
                        numberOfHours = Math.Round(numberOfHours, 0);
                    }
                    else
                    {
                        numberOfHours = Math.Abs(Math.Round(numberOfHours, 5));
                    }
                }
            }

            if (numberOfHours > 0)
            {
                return Math.Round(numberOfHours, 5);
            }

            return Math.Round(numberOfHours, 5);
        }

        public static decimal ComputeTardyUndertimeHoursv2(TrnDtrline line, IEnumerable<MstShiftCodeDay> shiftCodeDays, HRISContext _context)
        {
            if (DateOnly.FromDateTime(line.Date) == DateOnly.Parse("05/03/2025"))
            {

            }

            var result = 0m; // Tardy undertime hour(s) rendered

            var defaultTime = DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM");

            var lateHours = line.TardyLateHours;
            var hoursRendered = 0m; // Total hour(s) rendered
            var shiftNumberOfHours = 0m;
            var shiftCodeId = line?.ShiftCodeId ?? 0;

            var lineDayStr = line?.Date.ToString("dddd")?.ToUpper() ?? "NA";
            var lateFlexibility = shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == shiftCodeId &&
                    x.Day.ToUpper() == lineDayStr)
                ?.LateFlexibility ?? 0;

            var empId = line?.EmployeeId ?? 0;
            var payrollTypeId = _context.MstEmployees.FirstOrDefault(x => x.Id == empId)?.PayrollTypeId ?? 0;

            shiftNumberOfHours = shiftCodeDays
                .FirstOrDefault(x => x.ShiftCodeId == shiftCodeId && x.Day.ToUpper() == lineDayStr)
                ?.NumberOfHours ?? 0;

            if (line != null)
            {
                if (!line.Absent) 
                {
                    var shiftDates = line.ShiftDates?.Split(",");                    

                    var shiftTimeIn1 = defaultTime;
                    var shiftTimeOut1 = defaultTime;
                    var shiftTimeIn2 = defaultTime;
                    var shiftTimeOut2 = defaultTime;                    

                    if (shiftDates is not null && shiftDates.Count() > 0)
                    {
                        shiftTimeIn1 = DateTime.Parse(shiftDates[0]);
                        if(!string.IsNullOrEmpty(shiftDates[1])) shiftTimeOut1 = DateTime.Parse(shiftDates[1]);
                        if(!string.IsNullOrEmpty(shiftDates[2])) shiftTimeIn2 = DateTime.Parse(shiftDates[2]);
                        shiftTimeOut2 = DateTime.Parse(shiftDates[3]);

                        var shiftCodeId2 = _context.TrnChangeShiftLines
                            .Where(x => x.ChangeShiftId == line.Dtr.ChangeShiftId &&
                                x.EmployeeId == line.EmployeeId &&
                                x.Date.Date == line.Date.Date)
                            .FirstOrDefault()
                            ?.ShiftCodeId;

						if (shiftCodeId2 != null) 
                        {
                            var shiftCodeDay = _context.MstShiftCodeDays.FirstOrDefault(x => x.ShiftCodeId == shiftCodeId2 && x.Day == line.Date.DayOfWeek.ToString());

                            if (shiftCodeDay != null)
                            {
                                var origShiftTimeIn1 = shiftTimeIn1;
                                var origShiftTimeOut1 = shiftTimeOut1;
                                var origShiftTimeIn2 = shiftTimeIn2;
                                var origShiftTimeOut2 = shiftTimeOut2;

                                shiftTimeIn1 = DateTime.Parse($"{shiftTimeIn1.Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", shiftCodeDay.TimeIn1)}");

                                var discrepancyHrs = (shiftTimeIn1 - origShiftTimeIn1).TotalHours;

                                if (discrepancyHrs > 18) 
                                {
                                    shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
								}

								if (discrepancyHrs < -18)
								{
									shiftTimeIn1 = shiftTimeIn1.AddDays(1);
								}

								if (!string.IsNullOrEmpty(shiftDates[1]))
                                {
                                    shiftTimeOut1 = DateTime.Parse($"{shiftTimeOut1.Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", shiftCodeDay.TimeOut1)}");

									discrepancyHrs = (shiftTimeOut1 - origShiftTimeOut1).TotalHours;

									if (discrepancyHrs > 18)
									{
										shiftTimeOut1 = shiftTimeOut1.AddDays(-1);
									}

									if (discrepancyHrs < -18)
									{
										shiftTimeOut1 = shiftTimeOut1.AddDays(1);
									}
								}

                                if (!string.IsNullOrEmpty(shiftDates[2]))
                                {
                                    shiftTimeIn2 = DateTime.Parse($"{shiftTimeIn2.Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", shiftCodeDay.TimeIn2)}");
                                }

                                shiftTimeOut2 = DateTime.Parse($"{shiftTimeOut2.Date.ToString("MM/dd/yyyy")} {string.Format("{0:hh:mm tt}", shiftCodeDay.TimeOut2)}");
                            }
                        }

                        var isSCContainsIsTommorow = shiftCodeDays.Any(x => x.ShiftCodeId == shiftCodeId && x.IsTommorow == true);

                        var gap = (decimal)((line.TimeIn1 ?? DefaultDate) - shiftTimeIn1).TotalHours;

                        if (isSCContainsIsTommorow && (line.Date == shiftTimeIn1.Date || gap <= 20))
                        {
                            if (gap >= -4 && gap <= 4)
                            {

                            }
                            else
                            {
                                shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
                                if (!string.IsNullOrEmpty(shiftDates[1])) shiftTimeOut1 = shiftTimeOut1.AddDays(-1);
                                if (!string.IsNullOrEmpty(shiftDates[2])) shiftTimeIn2 = shiftTimeIn2.AddDays(-1);
                                shiftTimeOut2 = shiftTimeOut2.AddDays(-1);
                            }
                        }
                        else 
                        {
                            if (isSCContainsIsTommorow) 
                            {
                                if(gap >= -4 && gap <= 4)
                                {

                                }
                                else
                                {
                                    shiftTimeIn1 = shiftTimeIn1.AddDays(1);
                                    if (!string.IsNullOrEmpty(shiftDates[1])) shiftTimeOut1 = shiftTimeOut1.AddDays(1);
                                    if (!string.IsNullOrEmpty(shiftDates[2])) shiftTimeIn2 = shiftTimeIn2.AddDays(1);
                                    shiftTimeOut2 = shiftTimeOut2.AddDays(1);
                                }
                            }
                        }
                    }

                    var isFlexBreak = _context.MstEmployees?.FirstOrDefault(x => x.Id == line.EmployeeId)?.IsFlexBreak ?? false;

                    if (isFlexBreak)
                    {
                        if (line?.TimeIn1 == null && line?.TimeOut1 == null && line?.TimeIn2 == null && line?.TimeOut2 == null)
                        {
                            return 0m;
                        }

                        var timeIn1 = shiftTimeIn1;
                        var timeOut2 = line?.TimeOut2 ?? DefaultDate;

                        var timeOut1 = line?.TimeOut1 ?? DefaultDate;
                        var timeIn2 = line?.TimeIn2 ?? DefaultDate;

                        if (timeOut1 == DefaultDate && timeIn2 != DefaultDate)
                        {
                            timeOut1 = timeIn2.AddHours(-1);
                        }

                        if (timeIn2 == DefaultDate && timeOut1 != DefaultDate)
                        {
                            timeIn2 = timeOut1.AddHours(1);
                        }

                        if (timeOut2 > shiftTimeOut2)
                        {
                            timeOut2 = shiftTimeOut2;
                        }

                        var breakHours = (decimal)(timeIn2 - timeOut1).TotalHours;
                        var deductHours = 0m;

                        //if (breakHours < 1) 
                        //{
                        //    deductHours = 1 - breakHours;
                        //}

                        var firstHalfInHours = (decimal)(timeOut1 - timeIn1).TotalHours;
                        var secondHalfInHours = (decimal)(timeOut2 - timeIn2).TotalHours;

                        var actualNumberOfHours = firstHalfInHours + secondHalfInHours;

                        var shiftCodeSetup = _context.MstShiftCodes
                            .FirstOrDefault(x => x.Id == shiftCodeId);
                        var isStraight = shiftCodeSetup?.Remarks?.ToUpper()?.Contains("STRAIGHT") ?? false;

                        if (timeOut1 == DefaultDate && timeIn2 == DefaultDate && isStraight) 
                        {
                            actualNumberOfHours = (decimal)(timeOut2 - timeIn1).TotalHours;
                            result = shiftNumberOfHours - actualNumberOfHours;
                        }

                        if (timeOut1 == DefaultDate && timeIn2 == DefaultDate && !isStraight)
                        {
                            var halfDayHours = shiftNumberOfHours / 2;
                            var halfDayActualHours = (decimal)(shiftTimeOut2 - timeIn1).TotalHours;
                            actualNumberOfHours = halfDayHours < halfDayActualHours ? halfDayHours : halfDayActualHours;
                        }

                        result = shiftNumberOfHours - (actualNumberOfHours - deductHours);

                        if (line != null && line.Absent)
                        {
                            return 0m;
                        }
                    }
                    else
                    {
                        // 4 Swipes
                        if (line.TimeIn1 != null && line.TimeOut1 != null && line.TimeIn2 != null && line.TimeOut2 != null)
                        {
                            var time1 = (decimal)((line.TimeOut1 ?? DefaultDate) - shiftTimeIn1).TotalHours;
                            var time2 = (decimal)((line.TimeOut2 ?? DefaultDate) - shiftTimeIn2).TotalHours;

                            if (time1 < 0)
                            {
                                time1 = 0;
                            }

                            if (time2 < 0)
                            {
                                time2 = 0;
                            }

                            hoursRendered = time1 + time2;
                        }

                        // 2 Swipes
                        if (line.TimeIn1 != null && line.TimeOut2 != null)
                        {
                            //Add a break 
                            shiftNumberOfHours += 1;

                            if (TimeOnly.FromDateTime(shiftTimeIn1) == TimeOnly.Parse("12:01 AM") || TimeOnly.FromDateTime(shiftTimeIn1) == TimeOnly.Parse("12:01 PM"))
                            {
                                var addSeconds = (shiftTimeIn1 - DateTime.Parse($"{shiftTimeIn1.ToString("MM/dd/yyyy")} 12:00 AM")).TotalSeconds;

                                if (shiftTimeIn1.ToString("tt") == "PM")
                                {
                                    addSeconds = (TimeOnly.FromDateTime(shiftTimeIn1) - TimeOnly.FromDateTime(DateTime.Parse($"12:00 PM"))).TotalSeconds;
                                }

                                shiftTimeIn1 = shiftTimeIn1.AddSeconds(addSeconds * -1);
                            }

                            hoursRendered = (decimal)((line.TimeOut2 ?? DefaultDate) - shiftTimeIn1).TotalHours;

                            if (lateFlexibility > 0)
                            {
                                hoursRendered = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours;
                            }
                        }

                        if (lateFlexibility > 0)
                        {
                            hoursRendered = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours;
                        }

                        if (hoursRendered < shiftNumberOfHours)
                        {
                            var isFlex = _context.MstEmployees?.FirstOrDefault(x => x.Id == empId)?.IsFlex ?? false;

                            if (isFlex)
                            {
                                result = shiftNumberOfHours - hoursRendered;

                                if (hoursRendered == 0)
                                {
                                    result = 0;
                                }
                            }
                            else
                            {
                                result = shiftNumberOfHours - hoursRendered;

                                if (hoursRendered == 0)
                                {
                                    result = 0;
                                }

                                if (line?.TimeOut1 is null && line?.TimeIn2 is null)
                                {
                                    if (line is not null && line?.TimeIn1 is not null && line?.TimeOut2 is not null)
                                    {
                                        var totalHoursWorked = (decimal)((line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)).TotalHours;

                                        if (totalHoursWorked <= ((shiftNumberOfHours - 1) / 2))
                                        {
                                            result = (shiftNumberOfHours - 1) - hoursRendered;
                                        }
                                    }

                                }
                            }
                        }

                        if (result < 0)
                        {
                            result = 0;
                        }

                        if (payrollTypeId == 2)
                        {
                            result = 0;
                        }
                    }                    
                }
            }

            if (line?.OnLeave ?? false)
            {
                var leaveApplicationId = _context.TrnDtrs
                    .FirstOrDefault(x => x.Id == line.Dtrid)
                    ?.LeaveApplicationId ?? 0;

                var leaveWithPay = false;
                var leaveWithHours = 0m;

                if (leaveApplicationId != 0 && line.EmployeeId != 0)
                {
                    var leaveApplication = _context.TrnLeaveApplicationLines
                        .FirstOrDefault(x => x.LeaveApplicationId == leaveApplicationId &&
                            x.EmployeeId == line.EmployeeId &&
                            x.Date.Date == line.Date.Date);

                    if (leaveApplication is not null)
                    {
                        leaveWithPay = leaveApplication.WithPay;
                        leaveWithHours = leaveApplication.NumberOfHours;
                    }
                }

                if (leaveWithPay && !line.Absent)
                {
                    if (result > leaveWithHours)
                    {
                        result = result - leaveWithHours;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    if (line?.RegularHours is null || line?.RegularHours == 0)
                    {
                        result = Math.Round(result, 0);
                    }
                    else
                    {
                        result = Math.Abs(Math.Round(result, 5));
                    }
                }
            }

            if (result > 0)
            {
                return Math.Round(result, 5);
            }

            if (result < 0) 
            {
                return 0;
            }

            return Math.Round(result, 5);
        }

        public static decimal ComputeNetTotalHours(TrnDtrline line) 
        {
            return line.RegularHours + line.OvertimeHours + line.OvertimeNightHours - line.TardyLateHours - line.TardyUndertimeHours;
        }

        public static decimal ComputeDayMultiplier(TrnDtrline line, IEnumerable<MstEmployee> employees, IEnumerable<MstDayTypeDay> dayTypeDays, HRISContext _context) 
        {
            var multiplier = 1m;
            var excludedInFixed = false;

            var branchId = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.BranchId ?? 0;

            var dayTypeDay = dayTypeDays
                .FirstOrDefault(x => x.Date.Date == line.Date.Date && 
                    x.BranchId == branchId);

            if (dayTypeDay is not null)
            {
                if (line.RestDay)
                {
                    multiplier = _context.MstDayTypes.FirstOrDefault(x => x.Id == dayTypeDay.DayTypeId)?.RestdayDays ?? 0;
                }
                else
                {
                    multiplier = _context.MstDayTypes.FirstOrDefault(x => x.Id == dayTypeDay.DayTypeId)?.WorkingDays ?? 0;
                }

                excludedInFixed = dayTypeDay.ExcludedInFixed;
            }
            else 
            {
                if (line.RestDay)
                {
                    multiplier = _context.MstDayTypes.FirstOrDefault(x => x.Id == line.DayTypeId)?.RestdayDays ?? 0;
                }

                excludedInFixed = false;
            }

            var payrollTypeId = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

            if (!excludedInFixed) 
            {
                if (payrollTypeId == 2) 
                {
                    if (multiplier > 1) 
                    {
                        multiplier = multiplier - 1;
                    }
                }
            }

            if (payrollTypeId == 1) 
            {
                var dayTypeId = 1;
                var dateAfterHoliday = DefaultDate;
                var dateBeforeHoliday = DefaultDate;

                if (dayTypeDay is null) 
                {
                    dayTypeId = dayTypeDay?.DayTypeId ?? 1;
                    dateAfterHoliday = dayTypeDay?.DateAfter ?? DefaultDate;
                    dateBeforeHoliday = dayTypeDay?.DateBefore ?? DefaultDate;
                }

                if (line.TimeIn1 is null && line.TimeOut1 is null && line.TimeIn2 is null && line.TimeOut2 is null) 
                {
                    if (dayTypeId == 2) 
                    {
                        var isTrue = _context.TrnDtrlines.Any(x => x.EmployeeId == line.EmployeeId &&
                            x.Date.Date == dateBeforeHoliday.Date &&
                            !x.Absent);

                        if (isTrue) 
                        {
                            if (line?.RestDay ?? false)
                            {
                                multiplier = multiplier * 1.6m;
                            }
                            else 
                            {
                                multiplier = multiplier - 1;
                            }
                        }
                    }
                }
            }           

            if (payrollTypeId == 3)
            {
                var dayTypeId = dayTypeDay?.DayTypeId ?? 1;

                if (dayTypeId > 1) 
                {
                    multiplier--;
                }                
            }

            return multiplier;
        }

        public static decimal ComputeRatePerHour(TrnDtrline line, IEnumerable<MstEmployee> employees) 
        {
            var rate = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.HourlyRate ?? 0;

            return rate;
        }

        public static decimal ComputeRatePerNightHour(TrnDtrline line, IEnumerable<MstEmployee> employees)
        {
            var rate = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.NightHourlyRate ?? 0;

            return rate;
        }

        public static decimal ComputeRatePerOvertimeHour(TrnDtrline line, IEnumerable<MstEmployee> employees)
        {
            var rate = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.OvertimeHourlyRate ?? 0;

            return rate;
        }

        public static decimal ComputeRatePerOvertimeNightHour(TrnDtrline line, IEnumerable<MstEmployee> employees)
        {
            var rate = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.OvertimeNightHourlyRate ?? 0;

            return rate;
        }

        public static decimal ComputeRatePerHourTardy(TrnDtrline line, IEnumerable<MstEmployee> employees)
        {
            var rate = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.TardyHourlyRate ?? 0;

            return Math.Round(rate, 5);
        }

        public static decimal ComputeRegularAmount(TrnDtrline line)
        {
            var rate = line.RegularHours * line.RatePerHour;

            return Math.Round(rate, 5); ;
        }

        public static decimal ComputeNightAmount(TrnDtrline line, bool isEligibleForHolidayPay = false)
        {
            var multiplier = 0m;

            switch (line.DayTypeId)
            {
                case 2:
                    multiplier = line.RatePerNightHour * (isEligibleForHolidayPay ? 2 : 1); //* (line.DayMultiplier + 0.6m);
                    break;
                case 3:
                    multiplier = line.RatePerNightHour * (isEligibleForHolidayPay ? 1.3m : 1); //* (line.DayMultiplier * 1.3m);
                    break;
                default:
                    multiplier = line.RatePerNightHour * (isEligibleForHolidayPay ? 1 : 1); //* line.DayMultiplier;
                    break;
            }

            if (line.RestDay)
            {
                switch (line.DayTypeId)
                {
                    case 2:
                        multiplier = line.RatePerNightHour * (isEligibleForHolidayPay ? 2.6m : 1); //* (line.DayMultiplier + 0.6m);
                        break;
                    case 3:
                        multiplier = line.RatePerNightHour * (isEligibleForHolidayPay ? 1.5m : 1); //* (line.DayMultiplier * 1.3m);
                        break;
                    default:
                        multiplier = line.RatePerNightHour * (isEligibleForHolidayPay ? 1.3m : 1); //* line.DayMultiplier;
                        break;
                }
            }

            var rate = line.NightHours * multiplier; //* line.DayMultiplier;

            return Math.Round(rate, 5);
        }

        public static decimal ComputeOverTimeAmount(TrnDtrline line, IEnumerable<MstEmployee> employees, IEnumerable<MstDayTypeDay> dayTypeDays, bool isEligibleForHolidayPay = false) 
        {
            var amount = 0m;
            var branchId = 0;
            var excludedInFixed = false;

            //var overtimeMultiplier = Config.IsCustomComputeOvertimeAmountOnRestDay ? 1.69m : 1;

            var payrollTypeId = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

            if (payrollTypeId == 2)
            {
                branchId = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.BranchId ?? 0;

                var dayTypeDay = dayTypeDays
                   .FirstOrDefault(x => x.Date.Date == line.Date.Date &&
                       x.BranchId == branchId);

                if (dayTypeDay is not null)
                {
                    excludedInFixed = dayTypeDay.ExcludedInFixed;
                }
                else
                {
                    excludedInFixed = true;
                }

                if (!excludedInFixed)
                {
                    //amount = line.OvertimeHours * line.RatePerOvertimeHour * (line.DayMultiplier + 1);

                    switch (line.DayTypeId) 
                    {
                        case 2:
                            amount = line.OvertimeHours * line.RatePerOvertimeHour; //* (line.DayMultiplier  + 1.6m);
                            break;
                        case 3:
                            amount = line.OvertimeHours * line.RatePerOvertimeHour; //* (line.DayMultiplier * 1.3m);
                            break;
                        default:
                            amount = line.OvertimeHours * line.RatePerOvertimeHour; //* (line.DayMultiplier + 1);
                            break;
                    }
                }
                else
                {
                    //amount = line.OvertimeHours * line.RatePerOvertimeHour * line.DayMultiplier;

                    switch (line.DayTypeId)
                    {
                        case 2:
                            amount = line.OvertimeHours * line.RatePerOvertimeHour; //* (line.DayMultiplier + 0.6m);
                            break;
                        case 3:
                            amount = line.OvertimeHours * line.RatePerOvertimeHour; //* (line.DayMultiplier * 0.3m);
                            break;
                        default:
                            amount = line.OvertimeHours * line.RatePerOvertimeHour; //* line.DayMultiplier;
                            break;
                    }
                }
            }
            else 
            {
                //amount = line.OvertimeHours * line.RatePerOvertimeHour * line.DayMultiplier;

                switch (line.DayTypeId)
                {
                    case 2:
                        amount = line.OvertimeHours * line.RatePerOvertimeHour * (isEligibleForHolidayPay ? 2.6m : 1.25m); //* (line.DayMultiplier + 0.6m);
                        break;
                    case 3:
                        amount = line.OvertimeHours * line.RatePerOvertimeHour * (isEligibleForHolidayPay ? 1.69m : 1.25m); //* (line.DayMultiplier * 1.3m);
                        break;
                    default:
                        amount = line.OvertimeHours * line.RatePerOvertimeHour * (isEligibleForHolidayPay ? 1.25m : 1.25m); //* line.DayMultiplier;
                        break;
                }
            }

            if (line.RestDay) 
            {
                //return Math.Round(amount * overtimeMultiplier, 2);

                switch (line.DayTypeId)
                {
                    case 2:
                        amount = line.OvertimeHours * line.RatePerOvertimeHour * (isEligibleForHolidayPay ? 3.38m : 1.69m); //* (line.DayMultiplier + 0.6m);
                        break;
                    case 3:
                        amount = line.OvertimeHours * line.RatePerOvertimeHour * (isEligibleForHolidayPay ? 1.95m : 1.69m); //* (line.DayMultiplier * 1.3m);
                        break;
                    default:
                        amount = line.OvertimeHours * line.RatePerOvertimeHour * (isEligibleForHolidayPay ? 1.69m : 1.69m); //* line.DayMultiplier;
                        break;
                }
            }

            return Math.Round(amount, 5);
        }

        public static decimal ComputeOvertimeNightAmount(TrnDtrline line, IEnumerable<MstEmployee> employees, IEnumerable<MstDayTypeDay> dayTypeDays) 
        {
            var amount = 0m;
            var branchId = 0;
            var excludedInFixed = false;

            var payrollTypeId = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

            if (payrollTypeId == 2)
            {
                branchId = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.BranchId ?? 0;

                var dayTypeDay = dayTypeDays
                   .FirstOrDefault(x => x.Date.Date == line.Date.Date &&
                       x.BranchId == branchId);

                if (dayTypeDay is not null)
                {
                    excludedInFixed = dayTypeDay.ExcludedInFixed;
                }
                else
                {
                    excludedInFixed = true;
                }

                if (!excludedInFixed)
                {
                    //amount = line.OvertimeHours * line.RatePerOvertimeHour * (line.DayMultiplier + 1);

                    switch (line.DayTypeId)
                    {
                        case 2:
                            amount = line.OvertimeNightHours * line.RatePerHour * (line.DayMultiplier + 1.6m);
                            break;
                        case 3:
                            amount = line.OvertimeNightHours * line.RatePerHour * 1.69m;
                            break;
                        default:
                            amount = line.OvertimeNightHours * line.RatePerOvertimeNightHour * (line.DayMultiplier + 1);
                            break;
                    }

                    amount = line.OvertimeNightHours * line.RatePerHour * (line.DayMultiplier + 1m);
                }
                else
                {
                    //amount = line.OvertimeHours * line.RatePerOvertimeHour * line.DayMultiplier;

                    switch (line.DayTypeId)
                    {
                        case 2:
                            amount = line.OvertimeNightHours * line.RatePerHour * (line.DayMultiplier + 0.6m);
                            break;
                        case 3:
                            amount = line.OvertimeNightHours * line.RatePerHour * (line.DayMultiplier * 0.3m);
                            break;
                        default:
                            amount = line.OvertimeNightHours * line.RatePerOvertimeNightHour * line.DayMultiplier;
                            break;
                    }
                }
            }
            else
            {
                //amount = line.OvertimeHours * line.RatePerOvertimeHour * line.DayMultiplier;

                switch (line.DayTypeId)
                {
                    case 2:
                        amount = line.OvertimeNightHours * line.RatePerHour * (line.DayMultiplier + 0.6m);
                        break;
                    case 3:
                        amount = line.OvertimeNightHours * line.RatePerHour * (line.DayMultiplier * 1.3m);
                        break;
                    default:
                        amount = line.OvertimeNightHours * line.RatePerOvertimeHour * line.DayMultiplier;
                        break;
                }
            }

            return Math.Round(amount, 2);
        }

        public static decimal ComputeTotalAmount(TrnDtrline line, IEnumerable<MstShiftCodeDay> shiftCodeDays, bool isEligibleForHolidayPay, HRISContext _context)
        {
            var amount = 0m;

            var dayTypeId = line.DayTypeId;
            var restDay = line.RestDay;

            //var timeIn1 = TimeOnly.FromDateTime(line?.TimeIn1 ?? DefaultDate);
            //var timeOut2 = TimeOnly.FromDateTime(line?.TimeOut2 ?? DefaultDate); 
            var isShiftCodeDayIsTommorow = shiftCodeDays.FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId && x.Day == line.Date.ToString("dddd"))?.IsTommorow ?? false;

            if (isShiftCodeDayIsTommorow) 
            {
                //dayTypeId > 1 && !restDay && 
                if (dayTypeId == 1 && line.TimeIn1 is null && line.TimeOut1 == null && line.TimeIn2 is null && line.TimeOut2 is null) 
                {
                    return Math.Round(amount, 5);
                }
            }

            var isVariable = _context.MstEmployees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId == 1;

            var sTimeIn1 = TimeOnly.FromDateTime(shiftCodeDays.FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId && x.Day == line.Date.ToString("dddd"))?.TimeIn1 ?? DefaultDate);
            var sTimeOut2 = TimeOnly.FromDateTime(shiftCodeDays.FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId && x.Day == line.Date.ToString("dddd"))?.TimeOut2 ?? DefaultDate);

            var dmRest = _context.MstDayTypes
                ?.FirstOrDefault(x => x.Id == dayTypeId)
                ?.RestdayDays ?? 0;
            var dmNormal = _context.MstDayTypes
                ?.FirstOrDefault(x => x.Id == dayTypeId)
                ?.WorkingDays ?? 0;            

            var deduction = 0m;

            if (!isEligibleForHolidayPay) 
            {
                dayTypeId = 1;
                dmRest = 1;
                dmNormal = 1;
            }

            var payrollTypeId = _context.MstEmployees
                ?.FirstOrDefault(x => x.Id == line.EmployeeId)
                ?.PayrollTypeId ?? 0;

            if (payrollTypeId == 3 && dayTypeId > 1)
            {
                dmRest--;
                dmNormal--;
            }

            if (line is not null)
            {
                if (dayTypeId == 1)
                {
                    if (restDay)
                    {
                        amount = (line.RegularAmount * dmRest) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                    }
                    else
                    {
                        amount = (line.RegularAmount * dmNormal) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                    }
                }
                else if (dayTypeId == 2)
                {
                    if (restDay)
                    {
                        if (line.TimeIn1 is null && line.TimeOut1 is null && line.TimeIn2 is null && line.TimeOut2 is null)
                        {
                            amount = (line.RegularAmount * 1) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                        }
                        else
                        {
                            amount = (line.RegularAmount * dmRest) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                            amount -= deduction;
                        }
                    }
                    else
                    {
                        if (line.TimeIn1 is null && line.TimeOut1 is null && line.TimeIn2 is null && line.TimeOut2 is null)
                        {
                            amount = (line.RegularAmount * (dmNormal - 1)) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                        }
                        else
                        {
                            amount = (line.RegularAmount * dmNormal) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                            amount -= deduction;
                        }
                    }
                }
                else if (dayTypeId == 3)
                {
                    if (restDay)
                    {
                        if (line.TimeIn1 is null && line.TimeOut1 is null && line.TimeIn2 is null && line.TimeOut2 is null) 
                        {
                            amount = (line.RegularAmount * (dmRest-1)) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                            amount -= deduction;
                        }
                        else
                        {
                            amount = (line.RegularAmount * dmRest) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                            amount -= deduction;
                        }
                    }
                    else
                    {
                        if (line.TimeIn1 is null && line.TimeOut1 is null && line.TimeIn2 is null && line.TimeOut2 is null)
                        {
                            //if (isVariable)
                            //{
                            //    amount = line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                            //}
                            //else
                            //{
                            //    var m = dmNormal - 1;
                            //    amount = (line.RegularAmount * (dmNormal - m)) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                            //}

                            amount = line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;

                            amount -= deduction;

                            if (amount < 0) 
                            {
                                amount = 0;
                            }
                        }
                        else 
                        {
                            amount = (line.RegularAmount * dmNormal) + line.NightAmount + line.OvertimeNightAmount + line.OvertimeAmount;
                            amount -= deduction;
                        }                            
                    }
                }                
            }           

            return Math.Round(amount, 5);
        }

        public static decimal ComputeTardyAmount(TrnDtrline line, HRISContext _context)
        {
            //var rate = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy + line.DayMultiplier);

            //return Math.Round(rate, 2);

            var amount = 0m;
            var dmRest = _context.MstDayTypes
                ?.FirstOrDefault(x => x.Id == line.DayTypeId)
                ?.RestdayDays ?? 0;
            var dmNormal = _context.MstDayTypes
                ?.FirstOrDefault(x => x.Id == line.DayTypeId)
                ?.WorkingDays ?? 0;

            //if (line.TardyLateHours < 0)
            //{
            //    amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * line.DayMultiplier);
            //}
            //else 
            //{
            //    if (line.DayTypeId == 1)
            //    {
            //        if (line.RestDay)
            //        {
            //            amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * dmRest);
            //        }
            //        else 
            //        {
            //            amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * line.DayMultiplier);
            //        }
            //    }
            //    else if (line.DayTypeId == 2)
            //    {
            //        if (line.RestDay)
            //        {
            //            amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * dmRest);
            //        }
            //        else
            //        {
            //            amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * dmNormal);
            //        }
            //    }
            //    else if (line.DayTypeId == 3) 
            //    {
            //        if (line.RestDay)
            //        {
            //            amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * dmRest);
            //        }
            //        else
            //        {
            //            amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * dmNormal);
            //        }
            //    }
            //}

            if (line.RestDay)
            {
                //amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * dmRest);
                amount = (line.TardyLateHours + line.TardyUndertimeHours) * line.RatePerHourTardy;
            }
            else 
            {
                //amount = (line.TardyLateHours + line.TardyUndertimeHours) * (line.RatePerHourTardy * dmNormal);
                amount = (line.TardyLateHours + line.TardyUndertimeHours) * line.RatePerHourTardy;
            }

            return Math.Round(amount, 5);
        }

        public static decimal ComputeRatePerAbsentDay(TrnDtrline line, IEnumerable<MstEmployee> employees)
        {
            var rate = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.AbsentDailyRate ?? 0;

            return Math.Round(rate, 5);
        }

        public static decimal ComputeAbsentAmount(TrnDtrline line)
        {
            var rate = 0m;

            if (line.Absent)
            {
                rate = line.RatePerAbsentDay;
            }
            else if (line.HalfdayAbsent) 
            {
                rate = line.RatePerAbsentDay / 2;
            }
            else
            {
                rate = 0;
            }

            return Math.Round(rate, 5);
        }

        public static decimal ComputeNetAmount(TrnDtrline line)
        {
            //var leaveAmount = 0m;

            //if (line.OnLeave && !line.Absent) 
            //{
            //    var regularHours = 0m;
            //    var shiftCodeDay = _context.MstShiftCodeDays
            //        ?.FirstOrDefault(x => x.ShiftCodeId == line.ShiftCodeId &&
            //        x.Day.ToUpper() == line.Date.ToString("dddd").ToUpper());

            //    regularHours = shiftCodeDay?.NumberOfHours ?? 0;

            //    leaveAmount = regularHours * line.RatePerHour * line.DayMultiplier;
            //}

            //var rate = Math.Abs(line.TotalAmount - line.TardyAmount - leaveAmount);

            var rate = line.TotalAmount - line.TardyAmount;

            return Math.Round(rate, 5);
        }

        public static List<int> GetEmployeeIds(int? departmentId, HRISContext _context) 
        {
            var employeeIds = new List<int>();

            if (departmentId is not null) 
            {
                return _context.MstEmployees
                    //.Where(x => x.DepartmentId == departmentId && x.IsLocked)
                    .Where(x => x.DepartmentId == departmentId)
                    .Select(x => x.Id)
                    ?.ToList() ?? new List<int>();
            }

            return _context.MstEmployees
                //.Where(x => x.IsLocked)
                .Select(x => x.Id)
                ?.ToList() ?? new List<int>();
        }

        public static int QuickChangeShift(HRISContext _context, DateTime timeIn1, int employeeId, DateTime dtrDate, int changeShiftId, int? origShiftCodeId = null)
        {
            //var result = 0;

            var changeShiftCodeId = _context.TrnChangeShiftLines
                    ?.FirstOrDefault(x => x.ChangeShiftId == changeShiftId
                        && x.EmployeeId == employeeId
                        && x.Date.Date == dtrDate.Date)
                    ?.ShiftCodeId ?? 0;

            if (changeShiftCodeId > 0)
            {
                return changeShiftCodeId;
            }
            else 
            {
                return origShiftCodeId ?? 0;
            }

            //var empShiftCodes = _context.MstEmployeeShiftCodes.Where(x => x.EmployeeId == employeeId);
            //var shiftCodeId = origShiftCodeId == null? empShiftCodes.FirstOrDefault(x => x.EmployeeId == employeeId)?.ShiftCodeId ?? 0 : (origShiftCodeId ?? 0);

            //result = (empShiftCodes?.Any(x => x.ShiftCodeId == shiftCodeId) ?? false) ? shiftCodeId : (empShiftCodes?.FirstOrDefault()?.ShiftCodeId ?? 0);

            //if (empShiftCodes is not null) 
            //{
            //    foreach (var item in empShiftCodes)
            //    {
            //        var shiftCodes = _context.MstShiftCodeDays.Where(x => x.ShiftCodeId == item.ShiftCodeId);

            //        foreach (var shiftCode in shiftCodes)
            //        {
            //            if (shiftCode.Day == dtrDate.ToString("dddd"))
            //            {
            //                if (string.Format("{0:tt}", shiftCode.TimeIn1) == string.Format("{0:tt}", timeIn1))
            //                {
            //                    if (Math.Abs(int.Parse(string.Format("{0:HHmm}", shiftCode.TimeIn1)) - int.Parse(string.Format("{0:HHmm}", timeIn1))) <= 120)
            //                    {
            //                        return shiftCode.ShiftCodeId;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //return result;
        }

        public static int QuickChangeShiftv2(HRISContext _context, IEnumerable<EmployeeShiftCodeDay.Record> employeeShiftCodeDays, IEnumerable<MstEmployeeShiftCode> employeeShiftCodes, int employeeId, DateTime dtrDate, int changeShiftId, int? origShiftCodeId = null) 
        {
            var result = 0;

            var changeShiftCodeId = _context.TrnChangeShiftLines
                    ?.FirstOrDefault(x => x.ChangeShiftId == changeShiftId
                        && x.EmployeeId == employeeId
                        && x.Date.Date == dtrDate.Date)
                    ?.ShiftCodeId ?? 0;

            if (changeShiftCodeId > 0)
            {
                return changeShiftCodeId;
            }

            var empShiftCodes = employeeShiftCodes.Where(x => x.EmployeeId == employeeId);
            var shiftCodeId = origShiftCodeId == null ? empShiftCodes.FirstOrDefault(x => x.EmployeeId == employeeId)?.ShiftCodeId ?? 0 : (origShiftCodeId ?? 0);

            result = (empShiftCodes?.Any(x => x.ShiftCodeId == shiftCodeId) ?? false) ? shiftCodeId : (empShiftCodes?.FirstOrDefault()?.ShiftCodeId ?? 0);

            if (employeeShiftCodeDays is not null && employeeShiftCodeDays.Any()) 
            {
                result = employeeShiftCodeDays?.OrderBy(x => x.Interval)?.FirstOrDefault()?.ShiftCodeId ?? result;
            }

            return result;
        }
        #endregion

        #region Prefetch Employees and ShiftCodeDays
        public static IEnumerable<MstEmployee> GetEmployees(HRISContext _context)
        {
            return _context.MstEmployees.ToArray();
        }

        public static IEnumerable<MstShiftCodeDay> GetShiftCodeDays(HRISContext _context)
        {
            return _context.MstShiftCodeDays.ToArray();
        } 
        #endregion

        #region Edit Process logs and dtr items
        internal static void ProcessDtrLog(AddDtrLinesByProcessDtr command, List<TrnDtrLineDto> dtrLines, HRISContext _context)
        {
            var getEmployees = new GetEmployees()
            {
                PayrollGroupId = command.PayrollGroupId,
                DepartmentId = command.DepartmentId,
                EmployeeId = command.EmployeeId,
            };

            var getEmployeeList = getEmployees.Result();
            var employeeIds = getEmployeeList.Select(x => x.Id).ToList();

            //_context.Database.ExecuteSqlRaw($"DELETE FROM TrnDTRLine WHERE DTRId={command.DTRId} AND EmployeeId IN({string.Join(", ", getEmployeeList.Select(x => x.Id))})");
            //_context.SaveChanges();

            // SAFE DELETE OPERATION
            _context.TrnDtrlines
                .Where(line => line.Dtrid == command.DTRId && employeeIds.Contains(line.EmployeeId))
                .ExecuteDelete(); // Use ExecuteDelete() for sync or ExecuteDeleteAsync() for async

            var batchProcessor = new DtrBatchProcessor(getEmployeeList, command.DateStart, command.DateEnd, command.ChangeShiftId, _context);

            foreach (var employee in getEmployeeList)
            {
                try
                {
                    var remarks = !employee.IsLocked ? "In-Active" : "";

                    for (var dtrDate = command.DateStart; dtrDate <= command.DateEnd; dtrDate = dtrDate.AddDays(1))
                    {
                        dtrLines.Add(new TrnDtrLineDto()
                        {
                            Dtrid = command.DTRId,
                            EmployeeId = employee.Id,
                            Date = dtrDate,
                            ShiftCodeId = batchProcessor.GetShiftCode(command.ChangeShiftId, employee.Id, dtrDate),
                            TimeIn1 = null,
                            TimeOut1 = null,
                            TimeIn2 = null,
                            TimeOut2 = null,
                            OfficialBusiness = false,
                            OnLeave = false,
                            Absent = false,
                            HalfdayAbsent = false,
                            RegularHours = 0,
                            NightHours = 0,
                            OvertimeHours = 0,
                            OvertimeNightHours = 0,
                            GrossTotalHours = 0,
                            TardyLateHours = 0,
                            TardyUndertimeHours = 0,
                            NetTotalHours = 0,
                            DayTypeId = batchProcessor.GetDayType(employee.Id, dtrDate),
                            RestDay = false,
                            DayMultiplier = 1,
                            RatePerHour = 0,
                            RatePerNightHour = 0,
                            RatePerOvertimeHour = 0,
                            RatePerOvertimeNightHour = 0,
                            RegularAmount = 0,
                            NightAmount = 0,
                            OvertimeAmount = 0,
                            OvertimeNightAmount = 0,
                            TotalAmount = 0,
                            RatePerHourTardy = 0,
                            RatePerAbsentDay = 0,
                            TardyAmount = 0,
                            AbsentAmount = 0,
                            NetAmount = 0,
                            Dtrremarks = remarks,
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
        }

        internal static void ProcessDtrLines(int changeShiftId, List<TmpDtrLogs> logs, List<TrnDtrLineDto> dtrLines, DateTime dateStart, DateTime dateEnd, HRISContext _context) 
        {
            var employeesInLog = logs.GroupBy(x => x.EmployeeId).Select(y => y.Key);

            var employees = _context.MstEmployees.ToArray();
            var shiftCodeDays = _context.MstShiftCodeDays.ToArray();
            var employeeShiftCodes = _context.MstEmployeeShiftCodes?.ToArray() ?? new MstEmployeeShiftCode[0];

            if (employeesInLog != null) 
            {
                foreach (var empId in employeesInLog)
                {
                    var shiftCodeId = 0;
                    var days = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

                    var firstDateOfLogWeeklyPerEmployee = DefaultDateOnly;
                    var dateOfLogWeeklyPerEmployee = DefaultDateOnly;

                    var isWorkDayCompleted = true;
                    var lastTimeOutOfWorkShift = DefaultDate;

                    var isFlex = false;
                    var isFlexBreak = false;
                    var isLongShift = false;
                    var isWorkDayCompletedFlex = true;
                    var shiftTimeIn1Flex = DefaultDate;
                    var shiftTimeOut2Flex = DefaultDate;

                    var currentProcessedEmployeeId = 0;

                    var shiftTimeOut2OfWork = DefaultDate;
                    var aWeekIsLapsed = false;

                    var fLogs = logs.Where(x => x.EmployeeId == empId); //&& (x.LogType == "I" || x.LogType == "O" ));
                    var dlineIsJumped = false;

                    var dline = new TrnDtrLineDto();

                    var toBeProcessedPayrollGroupId = employees?.FirstOrDefault(x => x.Id == (dtrLines.FirstOrDefault()?.EmployeeId ?? 0))?.PayrollGroupId ?? 0;

                    foreach (var log in fLogs) 
                    {
                        try
                        {
                            var logEmployeePayrollGroupId = employees?.FirstOrDefault(x => x.Id == (log?.EmployeeId ?? 0))?.PayrollGroupId ?? 0;
                            if (logEmployeePayrollGroupId != toBeProcessedPayrollGroupId)
                            {
                                continue;
                            }

                            var ldt = DateTime.Parse($"{log.Date} {string.Format("{0:hh:mm:00 tt}", log.Time)}");
                            var ldtComp = DateTime.Parse("04/13/2025 12:02:00 PM");
                            if (ldt == ldtComp)
                            //if (log.Date.ToShortDateString() == "12/23/2024") //&& log.LogType == "I")
                            {

                            }

                            if (dline is not null)
                            {
                                isFlex = employees?.FirstOrDefault(x => x.Id == log.EmployeeId)?.IsFlex ?? false;
                                isFlexBreak = employees?.FirstOrDefault(x => x.Id == log.EmployeeId)?.IsFlexBreak ?? false;
                                isLongShift = employees?.FirstOrDefault(x => x.Id == log.EmployeeId)?.IsLongShift ?? false;

                                if (isFlex)
                                {
                                    if (log.Date == new DateOnly(2025, 6, 10)) 
                                    {
                                    
                                    }

                                    if (currentProcessedEmployeeId != log.EmployeeId)
                                    {
                                        currentProcessedEmployeeId = log.EmployeeId;
                                        isWorkDayCompletedFlex = true;
                                    }

                                    dline = dtrLines.FirstOrDefault(x => x.EmployeeId == log.EmployeeId && DateOnly.FromDateTime(x.Date) == log.Date);


                                    if (dline is not null) 
                                    {
                                        //Prevent/continue looping when timeIn1/TimeIn2 is filled up
                                        if (dline.TimeIn1 != null && dline.TimeOut2 != null)
                                        {
                                            continue;
                                        }

                                        shiftTimeIn1Flex = dline.Date;
                                        var logDateTime = DateTime.Parse($"{log.Date} {string.Format("{0:hh:mm tt}", log.Time)}");

                                        if (isWorkDayCompletedFlex)
                                        {
                                            isWorkDayCompletedFlex = false;
                                            shiftTimeOut2Flex = DefaultDate;
                                        }
                                        else
                                        {
                                            if (shiftTimeIn1Flex.Date != dline.Date)
                                            {
                                                dline = dtrLines.FirstOrDefault(x => x.EmployeeId == log.EmployeeId && DateOnly.FromDateTime(x.Date) == log.Date.AddDays(-1)) ?? dline;
                                            }
                                        }

                                        dline.ShiftCodeId = employeeShiftCodes?.FirstOrDefault(x => x.EmployeeId == log.EmployeeId)?.ShiftCodeId ?? 0;

                                        if (dline.TimeIn1 == null && log.LogType == "I")
                                        {
                                            var nextLog = fLogs
                                                ?.Where(x => x.EmployeeId == log.EmployeeId && DateTime.Parse($"{x.Date} {x.Time}") > logDateTime)
                                                ?.OrderBy(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                ?.FirstOrDefault() ?? new TmpDtrLogs();

                                            var nextLogDateTime = DateTime.Parse($"{nextLog.Date} {nextLog.Time}");
                                            var intervalInHours = (nextLogDateTime - logDateTime).TotalHours;

                                            if (intervalInHours > 19)
                                            {
                                                if (nextLogDateTime.Date != logDateTime.Date)
                                                {
                                                    dline.TimeIn1 = logDateTime;
                                                }

                                                isWorkDayCompletedFlex = true;
                                                continue;
                                            }

                                            dline.TimeIn1 = logDateTime;

                                            var tempShiftTimeOut2 = logDateTime.AddHours(19);
                                            var filteredLog = fLogs?
                                                .Where(x => x.EmployeeId == log.EmployeeId && DateTime.Parse($"{x.Date} {x.Time}") <= tempShiftTimeOut2)
                                                .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                .FirstOrDefault();

                                            if (filteredLog is not null)
                                            {
                                                shiftTimeOut2Flex = DateTime.Parse($"{filteredLog.Date} {filteredLog.Time}");
                                            }

                                            if (shiftTimeIn1Flex == shiftTimeOut2Flex)
                                            {
                                                isWorkDayCompletedFlex = true;
                                            }

                                            continue;
                                        }

                                        if (dline.TimeIn1 != null && (logDateTime == shiftTimeOut2Flex) && dline.TimeOut2 == null)
                                        {
                                            dline.TimeOut2 = logDateTime;
                                            isWorkDayCompletedFlex = true;
                                        }

                                        if (dline.TimeIn1 == null && shiftTimeOut2Flex == DefaultDate && dline.TimeOut2 == null)
                                        {
                                            dline.TimeOut2 = logDateTime;
                                            isWorkDayCompletedFlex = true;
                                        }
                                    }                                    

                                    continue;
                                }
                                else
                                {
                                    if (currentProcessedEmployeeId != log.EmployeeId)
                                    {
                                        if (log.LogType == "O" || log.LogType == "0" || log.LogType == "1")
                                        {
                                            if (log.Date <= DateOnly.FromDateTime(dateStart))
                                            {
                                                continue;
                                            }
                                        }

                                        currentProcessedEmployeeId = log.EmployeeId;
                                        isWorkDayCompleted = true;
                                    }

                                    var dLineTimeIn1 = dline?.TimeIn1;
                                    var dLineTimeOut2 = dline?.TimeOut2;

                                    if (dLineTimeIn1 != null && dLineTimeOut2 != null)
                                    {
                                        if (aWeekIsLapsed)
                                        {
                                            aWeekIsLapsed = false;
                                        }
                                        else
                                        {

                                        }
                                    }

                                    if (dline is not null && isWorkDayCompleted)
                                    {
                                        if (!isLongShift && dline.NoTimeOut2 && dline.Is2SwipesOnly && (log.LogType == "0" || log.LogType == "1"))
                                        {
                                            isWorkDayCompleted = true;
                                            continue;
                                        }

                                        if ((log.LogType == "O" || log.LogType == "0") && dline.NoTimeOut2 && !dline.Is2SwipesOnly)
                                        {
                                            dline.TimeOut1 = DateTime.Parse($"{log.Date} {string.Format("{0:hh:mm tt}", log.Time)}");
                                            dline.HalfdayAbsent = true;
                                            continue;
                                        }

                                        if (dline is not null &&
                                            dline.TimeIn1 is not null &&
                                            dline.TimeOut1 is null &&
                                            dline.TimeIn2 is null &&
                                            dline.TimeOut2 is not null &&
                                            (log.LogType == "0" || log.LogType == "1"))
                                        {
                                            var shiftDates = dline.ShiftDates?.Split(",");
                                            var logDateTimeOnWorkDateCompleted = DateTime.Parse($"{log.Date} {log.Time}");

                                            if (shiftDates is not null &&
                                                (!string.IsNullOrEmpty(shiftDates[0]) &&
                                                    string.IsNullOrEmpty(shiftDates[1]) &&
                                                    string.IsNullOrEmpty(shiftDates[2]) &&
                                                    !string.IsNullOrEmpty(shiftDates[3])) &&
                                                logDateTimeOnWorkDateCompleted > lastTimeOutOfWorkShift)
                                            {
                                                isWorkDayCompleted = true;
                                                continue;
                                            }
                                        }

                                        dline = dtrLines.FirstOrDefault(x => x.EmployeeId == log.EmployeeId && DateOnly.FromDateTime(x.Date) == log.Date);                                        

                                        if (dline is not null && dline.TimeIn1 is not null && dline.TimeOut2 is not null && isWorkDayCompleted)
                                        {
                                            dline.IsSplitted = true;
                                            dline = dtrLines.FirstOrDefault(x => x.EmployeeId == log.EmployeeId && DateOnly.FromDateTime(x.Date) == log.Date.AddDays(1));
                                            if (dline is not null) dline.IsSplitted = true;

                                            dlineIsJumped = true;
                                        }

                                        if (isLongShift && log.LogType == "O")
                                        {
                                            var dlineDate = DateOnly.FromDateTime(dline?.Date ?? DefaultDate);
                                            var lastTimeOut2 = dtrLines
                                                .Where(x => x.EmployeeId == log.EmployeeId &&
                                                    x.TimeOut2 != null &&
                                                    x.TimeOut2 == DateTime.Parse($"{log.Date} {log.Time}"))
                                                .OrderByDescending(x => x.Date)
                                                .FirstOrDefault();

                                            if (lastTimeOut2 != null)
                                            {
                                                continue;
                                            }
                                        }

                                        if (isFlexBreak && log.LogType == "O")
                                        {
                                            dline = dtrLines.FirstOrDefault(x => x.EmployeeId == log.EmployeeId && DateOnly.FromDateTime(x.Date) == log.Date);
                                            if(dline is not null) dline.TimeOut2 = DateTime.Parse($"{log.Date} {string.Format("{0:hh:mm tt}", log.Time)}");

                                            isWorkDayCompleted = true;

                                            continue;
                                        }

                                        var employeeShiftCodeDaysSetup = new EmployeeShiftCodeDay();

                                        if (employeeShiftCodes is not null && dline is not null)
                                        {
                                            employeeShiftCodeDaysSetup.ParamEmployeeId = dline.EmployeeId;
                                            employeeShiftCodeDaysSetup.ParamDay = dline.Date.ToString("dddd");
                                            employeeShiftCodeDaysSetup.ParamLogTimeIn1 = DateTime.Parse($"{log.Date:d} {string.Format("{0:hh:mm tt}", log.Time)}");

                                            var employeeShiftCodeDays = employeeShiftCodeDaysSetup.Result(isLongShift ? log.LogType : null);

                                            dline.ShiftCodeId = shiftCodeId = QuickChangeShiftv2(_context, employeeShiftCodeDays, employeeShiftCodes, dline.EmployeeId, dline.Date, changeShiftId, dline.ShiftCodeId);

                                            if (shiftCodeId == 0)
                                            {
                                                dline.ShiftCodeId = shiftCodeId = employees?.FirstOrDefault(x => x.Id == log.EmployeeId)?.ShiftCodeId ?? 0;
                                            }

                                            isWorkDayCompleted = false;
                                        }
                                    }

                                    var logType = log.LogType;
                                    var logDateTime = DateTime.Parse($"{log.Date} {string.Format("{0:hh:mm tt}", log.Time)}");

                                    var shiftCodeDay = new MstShiftCodeDay();
                                    var filteredShiftCodeDays = shiftCodeDays.Where(x => x.ShiftCodeId == shiftCodeId);

                                restartTheOrderedShiftCodeDays:
                                    var dictNumericDays = new Dictionary<string, int>();

                                    if (firstDateOfLogWeeklyPerEmployee == DefaultDateOnly)
                                    {
                                        firstDateOfLogWeeklyPerEmployee = fLogs
                                            ?.OrderBy(x => x.Date)
                                            ?.FirstOrDefault(x => x.EmployeeId == log.EmployeeId)
                                            ?.Date ?? DefaultDateOnly;
                                    }

                                    dateOfLogWeeklyPerEmployee = firstDateOfLogWeeklyPerEmployee;

                                    var getIndexOfDay = days.IndexOf(firstDateOfLogWeeklyPerEmployee.ToString("dddd"));

                                    for (int i = 0; i < 7; i++)
                                    {
                                        if (getIndexOfDay >= days.Count)
                                        {
                                            getIndexOfDay = 0;
                                        }

                                        dictNumericDays.Add(days[getIndexOfDay], i);
                                        getIndexOfDay += 1;
                                    }

                                    var orderedShiftCodeDays = new List<MstShiftCodeDay>();

                                    foreach (var shiftCode in filteredShiftCodeDays)
                                    {
                                        shiftCode.Sort = dictNumericDays[shiftCode.Day];
                                        orderedShiftCodeDays.Add(shiftCode);
                                    }

                                    orderedShiftCodeDays = orderedShiftCodeDays.OrderBy(x => x.Sort).ToList();

                                    foreach (var shiftCode in orderedShiftCodeDays)
                                    {
                                        shiftCode.TimeIn1 = DateTime.Parse($"{dateOfLogWeeklyPerEmployee} {string.Format("{0:hh:mm tt}", shiftCode.TimeIn1)}");

                                        if (shiftCode?.TimeOut1 is not null)
                                        {
                                            var shiftTimeIn1TimeOnly = TimeOnly.FromDateTime((DateTime)shiftCode.TimeIn1);
                                            var shiftTimeOut1TimeOnly = TimeOnly.FromDateTime((DateTime)shiftCode.TimeOut1);

                                            if (shiftTimeIn1TimeOnly > shiftTimeOut1TimeOnly)
                                            {
                                                shiftCode.TimeOut1 = DateTime.Parse($"{dateOfLogWeeklyPerEmployee} {string.Format("{0:hh:mm tt}", shiftCode.TimeOut1)}").AddDays(1);
                                            }
                                            else
                                            {
                                                shiftCode.TimeOut1 = DateTime.Parse($"{dateOfLogWeeklyPerEmployee} {string.Format("{0:hh:mm tt}", shiftCode.TimeOut1)}");
                                            }
                                        }

                                        if (shiftCode?.TimeIn2 is not null)
                                        {
                                            var shiftTimeIn1TimeOnly = TimeOnly.FromDateTime((DateTime)shiftCode.TimeIn1);
                                            var shiftTimeIn2TimeOnly = TimeOnly.FromDateTime((DateTime)shiftCode.TimeIn2);

                                            if (shiftTimeIn1TimeOnly > shiftTimeIn2TimeOnly)
                                            {
                                                shiftCode.TimeIn2 = DateTime.Parse($"{dateOfLogWeeklyPerEmployee} {string.Format("{0:hh:mm tt}", shiftCode.TimeIn2)}").AddDays(1);
                                            }
                                            else
                                            {
                                                shiftCode.TimeIn2 = DateTime.Parse($"{dateOfLogWeeklyPerEmployee} {string.Format("{0:hh:mm tt}", shiftCode.TimeIn2)}");
                                            }
                                        }

                                        if (shiftCode?.TimeOut2 is not null)
                                        {
                                            var shiftTimeIn1TimeOnly = TimeOnly.FromDateTime((DateTime)shiftCode.TimeIn1);
                                            var shiftTimeOut2TimeOnly = TimeOnly.FromDateTime((DateTime)shiftCode.TimeOut2);

                                            if (shiftTimeIn1TimeOnly > shiftTimeOut2TimeOnly)
                                            {
                                                shiftCode.TimeOut2 = DateTime.Parse($"{dateOfLogWeeklyPerEmployee} {string.Format("{0:hh:mm tt}", shiftCode.TimeOut2)}").AddDays(1);
                                            }
                                            else
                                            {
                                                shiftCode.TimeOut2 = DateTime.Parse($"{dateOfLogWeeklyPerEmployee} {string.Format("{0:hh:mm tt}", shiftCode.TimeOut2)}");
                                            }
                                        }

                                        dateOfLogWeeklyPerEmployee = dateOfLogWeeklyPerEmployee.AddDays(1);
                                    }

                                    var logDateShiftDay = "NA";
                                    MstShiftCodeDay? firstShiftCode = null;

                                    foreach (var shiftCode in orderedShiftCodeDays)
                                    {
                                        if (firstShiftCode == null)
                                        {
                                            firstShiftCode = shiftCode;
                                        }

                                        DateTime shiftIn1 = DateTime.Parse(shiftCode?.TimeIn1?.ToString("G") ?? DefaultDate.ToString("G")).AddHours(-4);
                                        DateTime shiftOut2 = DateTime.Parse(shiftCode?.TimeOut2?.ToString("G") ?? DefaultDate.ToString("G")).AddHours(6);

                                        if (logDateTime >= shiftIn1 && logDateTime <= shiftOut2)
                                        {
                                            logDateShiftDay = shiftCode?.Day ?? "NA";
                                        }
                                    }

                                    if (logDateShiftDay is not "NA")
                                    {
                                        shiftCodeDay = orderedShiftCodeDays.FirstOrDefault(x => x.Day == logDateShiftDay);
                                    }
                                    else
                                    {
                                        var lastShiftCodeTimeOut = orderedShiftCodeDays.LastOrDefault()?.TimeOut2;

                                        if (logDateTime <= lastShiftCodeTimeOut)
                                        {
                                            var orderedShiftCodeDaysFilterForLastLogTime = logDateTime;

                                            if (log.LogType == "O" && dline?.TimeIn1 is not null)
                                            {
                                                orderedShiftCodeDaysFilterForLastLogTime = dline?.TimeIn1 ?? DefaultDate;
                                            }

                                            shiftCodeDay = orderedShiftCodeDays.FirstOrDefault(x => x.Day == orderedShiftCodeDaysFilterForLastLogTime.ToString("dddd"));

                                            if (shiftCodeDay == null)
                                            {
                                                shiftCodeDay = orderedShiftCodeDays.FirstOrDefault(x => x.Day == logDateTime.AddDays(1).ToString("dddd"));
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                firstDateOfLogWeeklyPerEmployee = firstDateOfLogWeeklyPerEmployee.AddDays(7);

                                                goto restartTheOrderedShiftCodeDays;

                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(ex.Message);
                                            }
                                        }
                                    }

                                    if ((shiftCodeDay?.TimeOut1 is null && shiftCodeDay?.TimeIn2 is null) && (log.LogType == "0" || log.LogType == "1") && !isLongShift)
                                    {
                                        if (dline is not null && isFlexBreak)
                                        {
                                            if (dline.TimeIn2 == null && log.LogType == "1") 
                                            {
                                                dline.TimeIn2 = logDateTime;
                                            }

                                            if (log.LogType == "0") 
                                            {
                                                dline.TimeOut1 = logDateTime;
                                            }

                                            var workDayLastLog = fLogs
                                                ?.Where(x => x.EmployeeId == dline.EmployeeId &&
                                                    DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1?.AddHours(-4) &&
                                                    DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) && 
                                                    x.LogType != "I")
                                                ?.OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                ?.FirstOrDefault();

                                            var lastLogDateTime = DateTime.Parse($"{workDayLastLog?.Date} {workDayLastLog?.Time}");

                                            if ((workDayLastLog?.LogType ?? "NA") != "O" && logDateTime == lastLogDateTime) 
                                            {
                                                isWorkDayCompleted = true;                                                
                                            }                                           
                                        }

                                        continue;
                                    }

                                    if (dline is not null && shiftCodeDay is not null)
                                    {
                                        dline.IsShiftCodeIsTommorow = shiftCodeDay.IsTommorow;

                                        if (dline.ShiftDates is null && log.LogType == "I")
                                        {
                                            var prevDLine = dtrLines
                                                .Where(x => x.EmployeeId == log.EmployeeId && x.Date < dline.Date)
                                                .OrderByDescending(x => x.Date)
                                                .FirstOrDefault();

                                            dline.ShiftDates = string.Join(",", shiftCodeDay.TimeIn1, shiftCodeDay.TimeOut1, shiftCodeDay.TimeIn2, shiftCodeDay.TimeOut2);

                                            if (isLongShift && prevDLine is not null)
                                            {
                                                if (logDateTime.Date == prevDLine.TimeOut2.GetValueOrDefault().Date && shiftCodeDay.IsTommorow)
                                                {
                                                    dline.ShiftDates = string.Join(",",
                                                        shiftCodeDay.TimeIn1?.AddDays(-1).ToString() ?? string.Empty,
                                                        shiftCodeDay.TimeOut1?.AddDays(-1).ToString() ?? string.Empty,
                                                        shiftCodeDay.TimeIn2?.AddDays(-1).ToString() ?? string.Empty,
                                                        shiftCodeDay.TimeOut2?.AddDays(-1).ToString() ?? string.Empty
                                                    );
                                                }
                                            }
                                        }

                                        if (fLogs is not null && shiftCodeDay is not null)
                                        {
                                            if (dlineIsJumped)
                                            {
                                                if (shiftCodeDay is not null)
                                                {
                                                    shiftCodeDay.TimeIn1?.AddDays(1);
                                                    shiftCodeDay.TimeOut2?.AddDays(1);
                                                }
                                            }

                                            var closestToShiftTime = fLogs
                                                .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                    DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1 &&
                                                    DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                    (x.LogType == "O" || x.LogType == "0"))
                                                .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                .FirstOrDefault();

                                            if (shiftCodeDay is not null && shiftCodeDay.TimeOut1 == null && shiftCodeDay.TimeIn2 == null)
                                            {
                                                closestToShiftTime = fLogs
                                                   .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                       DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1 &&
                                                       DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                       (x.LogType == "O"))
                                                   .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                   .FirstOrDefault();
                                            }

                                            if (closestToShiftTime is not null)
                                            {
                                                lastTimeOutOfWorkShift = DateTime.Parse($"{closestToShiftTime.Date} {closestToShiftTime.Time}");
                                            }
                                        }

                                        if (logType == "I" || logType == "1")
                                        {
                                            if ((shiftCodeDay?.TimeIn1 ?? dline.Date) != dline.Date && (shiftCodeDay?.TimeOut1 ?? dline.Date) != dline.Date && shiftCodeDay?.TimeOut2 != dline.Date)
                                            {
                                                if (shiftCodeDay?.TimeIn1 > logDateTime || (logDateTime >= shiftCodeDay?.TimeIn1 && logDateTime < shiftCodeDay?.TimeOut2))
                                                {
                                                    if (dline.TimeIn1 is null)
                                                    {
                                                        if (dline.NoTimeIn1)
                                                        {
                                                            dline.TimeIn2 = logDateTime;
                                                        }
                                                        else
                                                        {
                                                            dline.TimeIn1 = logDateTime;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (logDateTime < (dline.TimeIn1 ?? DefaultDate))
                                                        {
                                                            var nextLog = fLogs
                                                                ?.Where(x => x.EmployeeId == log.EmployeeId && DateTime.Parse($"{x.Date} {x.Time}") > logDateTime)
                                                                ?.OrderBy(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                                ?.FirstOrDefault() ?? new TmpDtrLogs();

                                                            var nextLogDateTime = DateTime.Parse($"{nextLog.Date} {nextLog.Time}");
                                                            var intervalInHours = (nextLogDateTime - logDateTime).TotalHours;

                                                            if (intervalInHours > 12)
                                                            {
                                                                continue;
                                                            }

                                                            dline.TimeIn1 = logDateTime;
                                                        }
                                                        else
                                                        {
                                                            var timeOut1Compare = shiftCodeDay?.TimeOut1;

                                                            if (timeOut1Compare is not null) 
                                                            {
                                                                var intervalInHours = (timeOut1Compare.Value - logDateTime).TotalHours;

                                                                if (intervalInHours > 2)
                                                                {
                                                                    continue;
                                                                }
                                                            }

                                                            dline.TimeIn2 = logDateTime;
                                                        }
                                                    }
                                                }
                                                else if (logDateTime > shiftCodeDay?.TimeOut1 && logDateTime < shiftCodeDay?.TimeOut2)
                                                {
                                                    if (dline.TimeIn2 is null)
                                                    {
                                                        dline.TimeIn2 = logDateTime;
                                                    }
                                                    else
                                                    {
                                                        if (logDateTime < (dline.TimeIn2 ?? DefaultDate))
                                                        {
                                                            dline.TimeIn2 = logDateTime;
                                                        }
                                                    }
                                                }
                                            }

                                            if ((shiftCodeDay?.TimeIn1 ?? dline.Date) != dline.Date && (shiftCodeDay?.TimeOut1 ?? dline.Date) == dline.Date && shiftCodeDay?.TimeOut2 != dline.Date)
                                            {
                                                if (logDateTime < shiftCodeDay?.TimeIn1 || (logDateTime >= shiftCodeDay?.TimeIn1 && logDateTime < shiftCodeDay?.TimeOut2))
                                                {
                                                    if (dline.TimeIn1 is null)
                                                    {
                                                        dline.TimeIn1 = logDateTime;
                                                    }
                                                    else
                                                    {
                                                        if (logDateTime < (dline.TimeIn1 ?? DefaultDate))
                                                        {
                                                            dline.TimeIn1 = logDateTime;
                                                        }
                                                    }
                                                }
                                            }

                                            if (fLogs is not null && shiftCodeDay is not null)
                                            {
                                                var workDaysWithOuts = fLogs
                                                    .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                        DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1?.AddHours(-4) &&
                                                        DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                        x.LogType == "O")
                                                    .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"));

                                                var is4Swipes = shiftCodeDay?.TimeIn1 != null && shiftCodeDay?.TimeOut1 != null && shiftCodeDay?.TimeIn2 != null && shiftCodeDay?.TimeOut2 != null;

                                                if (is4Swipes) 
                                                {
                                                    workDaysWithOuts = fLogs
                                                        .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                            DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1?.AddHours(-4) &&
                                                            DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                            (x.LogType == "0" || x.LogType == "O"))
                                                        .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"));
                                                }

                                                var workDayHasOut = workDaysWithOuts.Any();

                                                if (!workDayHasOut && !isFlexBreak)
                                                {
                                                    if (isLongShift)
                                                    {
                                                        if (shiftCodeDay.TimeOut1 == null && shiftCodeDay.TimeIn2 == null)
                                                        {
                                                            dline.Is2SwipesOnly = true;
                                                        }

                                                        var nextLog = fLogs
                                                            .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                                DateTime.Parse($"{x.Date} {x.Time}") > logDateTime)
                                                            .OrderBy(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                            .Select(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                            .FirstOrDefault();

                                                        var logCountForShift = fLogs
                                                            .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                                DateTime.Parse($"{x.Date} {x.Time}") >= logDateTime && DateTime.Parse($"{x.Date} {x.Time}") <= (shiftCodeDay?.TimeOut2 ?? DefaultDate).AddHours(8))
                                                            .Count();

                                                        var noOfHoursConsumed = (nextLog - logDateTime).TotalHours;

                                                        if (noOfHoursConsumed > 12)
                                                        {
                                                            if (noOfHoursConsumed > 36)
                                                            {
                                                                dline.NoTimeOut2 = true;
                                                                isWorkDayCompleted = true;

                                                                continue;
                                                            }

                                                            dline.TimeOut2 = nextLog;

                                                            if ((log.LogType == "I" || log.LogType == "1") && logCountForShift > 1)
                                                            {
                                                                var nLog = fLogs
                                                                    .Where(x => DateTime.Parse($"{x.Date} {x.Time}") == nextLog)
                                                                    .FirstOrDefault();

                                                                if (nLog is not null)
                                                                {
                                                                    nLog.LogType = "O";
                                                                }
                                                            }

                                                            isWorkDayCompleted = true;
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }

                                                    var closestToShiftTime1 = fLogs
                                                        .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                            DateTime.Parse($"{x.Date} {x.Time}") != dline?.TimeIn1 &&
                                                            DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1 &&
                                                            DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                            (x.LogType == "I"))
                                                        .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                        .FirstOrDefault();

                                                    var workDayLogsI = fLogs
                                                        .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                            DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1?.AddHours(-4) &&
                                                            DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8));

                                                    if (workDayLogsI != null && workDayLogsI.Any(x => x.LogType == "I"))
                                                    {
                                                        if (closestToShiftTime1 is not null)
                                                        {
                                                            lastTimeOutOfWorkShift = DateTime.Parse($"{closestToShiftTime1.Date} {closestToShiftTime1.Time}");
                                                            logType = "O";
                                                        }
                                                        else
                                                        {
                                                            if (shiftCodeDay?.TimeIn1 != null && shiftCodeDay?.TimeOut1 != null && shiftCodeDay?.TimeIn2 != null && shiftCodeDay?.TimeOut2 != null)
                                                            {
                                                                var workDayLastLog = fLogs
                                                                    .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                                        DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1?.AddHours(-4) &&
                                                                        DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                                        (x.LogType == "I" || x.LogType == "1"))
                                                                    .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                                    .FirstOrDefault();

                                                                if (workDayLastLog != null)
                                                                {
                                                                    var workDayLastLogDateTime = DateTime.Parse($"{workDayLastLog.Date} {workDayLastLog.Time}");

                                                                    if (workDayLastLogDateTime == logDateTime)
                                                                    {
                                                                        var workDaysWithOuts2 = fLogs
                                                                            .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                                                DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1?.AddHours(-4) &&
                                                                                DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                                                x.LogType == "O")
                                                                            .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"));

                                                                        var workDayHasOut2 = workDaysWithOuts2.Any();

                                                                        if (!workDayHasOut2)
                                                                        {
                                                                            dline.IsShiftCodeIsTommorow = shiftCodeDay?.IsTommorow ?? false;
                                                                            dline.NoTimeOut2 = true;
                                                                            isWorkDayCompleted = true;
                                                                            continue;
                                                                        }

                                                                        isWorkDayCompleted = true;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var workDayLastLog = fLogs
                                                                    .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                                        DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1?.AddHours(-4) &&
                                                                        DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                                        x.LogType == "I")
                                                                    .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                                    .FirstOrDefault();

                                                                if (workDayLastLog != null)
                                                                {
                                                                    var workDayLastLogDateTime = DateTime.Parse($"{workDayLastLog.Date} {workDayLastLog.Time}");

                                                                    if (workDayLastLogDateTime == logDateTime)
                                                                    {
                                                                        dline.IsShiftCodeIsTommorow = shiftCodeDay?.IsTommorow ?? false;
                                                                        dline.Is2SwipesOnly = true;
                                                                        dline.NoTimeOut2 = true;
                                                                        isWorkDayCompleted = true;

                                                                        continue;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                if (logDateTime == lastTimeOutOfWorkShift)
                                                {
                                                    if (dline != null)
                                                    {
                                                        dline.TimeOut2 = logDateTime;
                                                    }
                                                }
                                            }
                                        }
                                        else if (logType == "O" || logType == "0")
                                        {
                                            if ((shiftCodeDay?.TimeOut1 ?? dline.Date) != dline.Date && shiftCodeDay?.TimeOut2 != dline.Date && shiftCodeDay?.TimeIn2 != dline.Date)
                                            {
                                                if (logDateTime > shiftCodeDay?.TimeIn1 && logDateTime < shiftCodeDay?.TimeIn2)
                                                {
                                                    if (dline.TimeOut1 is null)
                                                    {
                                                        dline.TimeOut1 = logDateTime;

                                                        if (dline.TimeIn1 == null)
                                                        {
                                                            dline.NoTimeIn1 = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (dline.TimeOut1 is not null && logDateTime > dline.TimeOut1)
                                                        {
                                                            dline.TimeOut1 = logDateTime;
                                                        }
                                                    }
                                                }
                                                else if (logDateTime > shiftCodeDay?.TimeIn2)
                                                {
                                                    if (dline.TimeOut2 is null)
                                                    {
                                                        dline.TimeOut2 = logDateTime;
                                                    }
                                                    else
                                                    {
                                                        if (dline.TimeOut2 is not null && logDateTime > (dline.TimeOut2 ?? DefaultDate))
                                                        {
                                                            dline.TimeOut2 = logDateTime;
                                                        }
                                                    }
                                                }
                                            }

                                            if ((shiftCodeDay?.TimeOut1 ?? dline.Date) == dline.Date && shiftCodeDay?.TimeOut2 != dline.Date)
                                            {
                                                var shiftCodeDayTimeIn1 = TimeOnly.FromDateTime(shiftCodeDay?.TimeIn1 ?? DefaultDate);
                                                var shiftCodeDayTimeOut2 = TimeOnly.FromDateTime(shiftCodeDay?.TimeOut2 ?? DefaultDate);

                                                if (shiftCodeDayTimeIn1 > shiftCodeDayTimeOut2)
                                                {
                                                    if (dline.TimeOut2 is null)
                                                    {
                                                        dline.TimeOut2 = logDateTime;
                                                    }
                                                    else
                                                    {
                                                        if (dline.TimeOut2 is not null && logDateTime > (dline.TimeOut2 ?? DefaultDate))
                                                        {
                                                            dline.TimeOut2 = logDateTime;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (logDateTime > shiftCodeDay?.TimeIn1)
                                                    {
                                                        if (dline.TimeOut2 is null)
                                                        {
                                                            dline.TimeOut2 = logDateTime;
                                                        }
                                                        else
                                                        {
                                                            if (dline.TimeOut2 is not null && logDateTime > (dline.TimeOut2 ?? DefaultDate))
                                                            {
                                                                dline.TimeOut2 = logDateTime;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            var is4Swipes = shiftCodeDay?.TimeIn1 != null && shiftCodeDay?.TimeOut1 != null && shiftCodeDay?.TimeIn2 != null && shiftCodeDay?.TimeOut2 != null;

                                            if (is4Swipes)
                                            {
                                                var workDaysWithOuts = fLogs
                                                    .Where(x => x.EmployeeId == dline.EmployeeId &&
                                                        DateTime.Parse($"{x.Date} {x.Time}") >= shiftCodeDay?.TimeIn1?.AddHours(-4) &&
                                                        DateTime.Parse($"{x.Date} {x.Time}") <= shiftCodeDay?.TimeOut2?.AddHours(8) &&
                                                        x.LogType == "O")
                                                    .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"));

                                                isWorkDayCompleted = true;
                                            }
                                        }
                                    }

                                    if (logDateTime == lastTimeOutOfWorkShift && logType == "O")
                                    {
                                        isWorkDayCompleted = true;

                                        if (dlineIsJumped)
                                        {
                                            if (dline is not null)
                                            {
                                                if (dline.Date.Date == (dline.TimeOut2?.Date ?? DefaultDate))
                                                {
                                                    dline.IsSplitted = true;
                                                }
                                            }

                                            dlineIsJumped = false;
                                        }

                                        if (dline is not null && (shiftCodeDay?.IsTommorow ?? false))
                                        {
                                            if (DateOnly.FromDateTime(dline.TimeIn1 ?? DefaultDate) != DateOnly.FromDateTime(dline.TimeOut2 ?? DefaultDate))
                                            {
                                                dline.IsShiftCodeIsTommorow = true;
                                            }
                                        }
                                    }

                                    var lastLogDateTimeOfEmployeeForTheWeek = DefaultDate;
                                    var lastShiftDateTimeOfEmployeeForTheWeek = DefaultDate;
                                    var nextLogDateTimeOfEmployee = DefaultDate;

                                    if (fLogs is not null)
                                    {
                                        var lastShiftTimeOut2DateForTheWeek = DateOnly.FromDateTime(orderedShiftCodeDays
                                            ?.OrderByDescending(x => x.TimeOut2)
                                            ?.FirstOrDefault()?.TimeOut2 ?? DefaultDate);
                                        var lastShiftTimeOut2TimeForTheWeek = TimeOnly.FromDateTime(orderedShiftCodeDays
                                            ?.OrderByDescending(x => x.TimeOut2)
                                            ?.FirstOrDefault()?.TimeOut2 ?? DefaultDate);

                                        var lastLogOfEmployeeForTheWeek = fLogs.Where(x => x.EmployeeId == log.EmployeeId &&
                                                x.Date == lastShiftTimeOut2DateForTheWeek)
                                            .OrderByDescending(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                            .FirstOrDefault();

                                        if (lastLogOfEmployeeForTheWeek is not null)
                                        {
                                            lastLogDateTimeOfEmployeeForTheWeek = DateTime.Parse($"{lastLogOfEmployeeForTheWeek.Date} {lastLogOfEmployeeForTheWeek.Time}");
                                        }

                                        lastShiftDateTimeOfEmployeeForTheWeek = DateTime.Parse($"{lastShiftTimeOut2DateForTheWeek} {lastShiftTimeOut2TimeForTheWeek}");

                                        var nextLogOfEmployee = fLogs.Where(x => x.EmployeeId == log.EmployeeId &&
                                                DateTime.Parse($"{x.Date} {x.Time}") > logDateTime)
                                            .OrderBy(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                            .FirstOrDefault();

                                        if (nextLogOfEmployee is not null)
                                        {
                                            nextLogDateTimeOfEmployee = DateTime.Parse($"{nextLogOfEmployee.Date} {nextLogOfEmployee.Time}");

                                            if (isWorkDayCompleted && nextLogDateTimeOfEmployee > lastShiftDateTimeOfEmployeeForTheWeek.AddHours(2))
                                            {
                                                var dateTommorow = (dline?.Date ?? DefaultDate).AddDays(1);
                                                var isLogsHasTommorowDate = fLogs.Any(x => x.Date == DateOnly.FromDateTime(dateTommorow));

                                                if (!isLogsHasTommorowDate)
                                                {
                                                    continue;
                                                }

                                                var newShiftCodeDayTimeIn1 = DefaultDate;
                                                var employeeShiftCodeDaysSetup = new EmployeeShiftCodeDay();

                                                if (employeeShiftCodes is not null && dline is not null)
                                                {
                                                    employeeShiftCodeDaysSetup.ParamEmployeeId = dline.EmployeeId;
                                                    employeeShiftCodeDaysSetup.ParamDay = dline.Date.ToString("dddd");
                                                    employeeShiftCodeDaysSetup.ParamLogTimeIn1 = nextLogDateTimeOfEmployee;

                                                    var employeeShiftCodeDays = employeeShiftCodeDaysSetup.Result();
                                                    var newShiftCodeId = QuickChangeShiftv2(_context, employeeShiftCodeDays, employeeShiftCodes, dline.EmployeeId, dline.Date, changeShiftId, dline.ShiftCodeId);

                                                    if (shiftCodeId == 0)
                                                    {
                                                        newShiftCodeId = employees?.FirstOrDefault(x => x.Id == log.EmployeeId)?.ShiftCodeId ?? 0;
                                                    }

                                                    newShiftCodeDayTimeIn1 = shiftCodeDays.Where(x => x.ShiftCodeId == newShiftCodeId).FirstOrDefault()?.TimeIn1 ?? new DateTime();
                                                }

                                                var firstShiftTimeIn1OfEmployeeNextWeek = DateTime.Parse($"{firstDateOfLogWeeklyPerEmployee.AddDays(7)} {TimeOnly.FromDateTime(newShiftCodeDayTimeIn1)}");
                                                var nextLogDateTimeOfEmployeeTofirstShiftTimeIn1OfEmployeeNextWeekInterval = (firstShiftTimeIn1OfEmployeeNextWeek - nextLogDateTimeOfEmployee).TotalHours;

                                                if (nextLogDateTimeOfEmployeeTofirstShiftTimeIn1OfEmployeeNextWeekInterval > 14)
                                                {
                                                    firstDateOfLogWeeklyPerEmployee = firstDateOfLogWeeklyPerEmployee.AddDays(6);
                                                }
                                                else
                                                {
                                                    firstDateOfLogWeeklyPerEmployee = firstDateOfLogWeeklyPerEmployee.AddDays(7);
                                                }

                                                aWeekIsLapsed = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }                       
                    }
                }
            }

            var empsWithShiftIsTommorow = dtrLines
                        .Where(x => x.IsShiftCodeIsTommorow)
                        .GroupBy(x => x.EmployeeId)
                        .Select(x => x.Key);

            if (empsWithShiftIsTommorow is not null)
            {
                foreach (var employeeId in empsWithShiftIsTommorow)
                {
                    var empDLines = dtrLines.Where(x => x.EmployeeId == employeeId && x.IsShiftCodeIsTommorow && !x.IsSplitted);

                    foreach (var empDLine in empDLines)
                    {
						var empDLineYesterday = dtrLines.FirstOrDefault(x => x.EmployeeId == employeeId && 
                                !x.IsSplitted && 
                                x.Date.Date == empDLine.Date.AddDays(-1).Date) 
                            ?? new TrnDtrLineDto();  

                        if (!empDLine.IsDateMoved && !empDLineYesterday.IsShiftCodeIsTommorow)
                        {
                            empDLine.OldShiftCodeId = empDLine.ShiftCodeId;
                            empDLine.OldTimeIn1 = empDLine.TimeIn1;
                            empDLine.OldTimeOut1 = empDLine.TimeOut1;
                            empDLine.OldTimeIn2 = empDLine.TimeIn2;
                            empDLine.OldTimeOut2 = empDLine.TimeOut2;
                            empDLine.OldShiftDates = empDLine.ShiftDates;

                            empDLine.TimeIn1 = null;
                            empDLine.TimeOut1 = null;
                            empDLine.TimeIn2 = null;
                            empDLine.TimeOut2 = null;
                            empDLine.ShiftDates = null;
                        }

                        var empDLineTommorow = dtrLines.FirstOrDefault(x => x.EmployeeId == employeeId && x.Date.Date == empDLine.Date.AddDays(1).Date);

                        if (empDLineTommorow is not null)
                        {
                            empDLineTommorow.OldShiftCodeId = empDLineTommorow.ShiftCodeId;
                            empDLineTommorow.OldTimeIn1 = empDLineTommorow.TimeIn1;
                            empDLineTommorow.OldTimeOut1 = empDLineTommorow.TimeOut1;
                            empDLineTommorow.OldTimeIn2 = empDLineTommorow.TimeIn2;
                            empDLineTommorow.OldTimeOut2 = empDLineTommorow.TimeOut2;
                            empDLineTommorow.OldShiftDates = empDLineTommorow.ShiftDates;

                            empDLineTommorow.ShiftCodeId = empDLine.OldShiftCodeId;
                            empDLineTommorow.TimeIn1 = empDLine.OldTimeIn1;
                            empDLineTommorow.TimeOut1 = empDLine.OldTimeOut1;
                            empDLineTommorow.TimeIn2 = empDLine.OldTimeIn2;
                            empDLineTommorow.TimeOut2 = empDLine.OldTimeOut2;
                            empDLineTommorow.ShiftDates = empDLine.OldShiftDates;

                            empDLineTommorow.IsDateMoved = true;
                        }
					}
                }
            }

            var defaultShiftCode = _context.MstShiftCodes.FirstOrDefault(x => x.ShiftCode == "DEFAULT");
            var dtrLinesWithNoShiftCodeIds = dtrLines.Where(x => x.ShiftCodeId == 0);

            foreach (var line in dtrLinesWithNoShiftCodeIds) 
            {
                line.ShiftCodeId = defaultShiftCode?.Id ?? 0;
            }

            //if (employeesInLog != null) 
            //{
            //    foreach (var empId in employeesInLog)
            //    {
            //        var dtrLineArray = dtrLines
            //        .Where(x => x.EmployeeId == empId && 
            //            (DateOnly.FromDateTime(x.Date) == DateOnly.FromDateTime(dateStart) || 
            //            DateOnly.FromDateTime(x.Date) == DateOnly.FromDateTime(dateEnd)))
            //        .ToArray();

            //        foreach (var line in dtrLineArray)
            //        {
            //            dtrLines.Remove(line);
            //        }
            //    }
            //}

            employees = null;
            shiftCodeDays = null;
            employeeShiftCodes = null;
        }
        
        internal static void ComputeDtrLines(TrnDtr dtr, EditDtrLinesByComputeDtr command, HRISContext _context)
        {
            var empId = command?.EmployeeId;
            var dtrLines = dtr.TrnDtrlines;

            if (empId is not null)
            {
                dtrLines = dtr.TrnDtrlines
                    .Where(x => x.EmployeeId == empId)
                    .ToList();
            }

            var employees = _context.MstEmployees.ToArray();
            var shiftCodeDays = _context.MstShiftCodeDays.ToArray();
            var dayTypeDays = _context.MstDayTypeDays.ToArray();

            foreach (var line in dtrLines)
            {
                try
                {
                    if (command is not null && line is not null)
                    {
                        var empLines = dtrLines?.Where(x => x.EmployeeId == empId)?.OrderBy(x => x.Date)?.ToArray();

                        if (empLines is not null) 
                        {
                            var lineDate = line?.Date;
                            var empStartDate = empLines.FirstOrDefault()?.Date;                            

                            if (lineDate == empStartDate && lineDate != command.DateStart) 
                            {
                                if (string.IsNullOrEmpty(line?.Dtrremarks?.Trim())) 
                                {
                                    continue;
                                }                               
                            }
                        }

                        var dateOnTimeIn1 = line?.Date ?? DateTime.Now;
                        var dateOnTimeOut1 = line?.TimeOut1 ?? line?.Date ?? DateTime.Now.Date;
                        var dateOnTimeIn2 = line?.TimeIn2 ?? line?.Date ?? DateTime.Now.Date;
                        var dateOnTimeOut2 = line?.TimeOut2 ?? line?.Date ?? DateTime.Now.Date;

                        var shiftCodeId = line?.ShiftCodeId ?? 0;

                        var lineDay = line?.Date.ToString("dddd").ToUpper() ?? "NA";

                        if ((line?.TimeIn1 is null && line?.TimeOut2 is not null) ||
                            (line?.TimeIn1 is not null && line?.TimeOut2 is null))
                        {
                            if (line?.TimeOut2 is null && !(line?.HalfdayAbsent ?? false))
                            {
                                var tOut2 = (shiftCodeDays
                                    .FirstOrDefault(x => x.ShiftCodeId == shiftCodeId &&
                                        x.Day.ToUpper() == lineDay)
                                    ?.TimeOut2 ?? DateTime.Parse($"{DateTime.Now.Date.ToString("MM/dd/yyyy")} 12:00 AM"))
                                    .ToString("HH:mm tt");

                                var shiftTimeOut2 = DateTime.Parse($"{dateOnTimeOut2.ToString("MM/dd/yyyy")} {tOut2}");

                                if (line is not null) line.TimeOut2 = shiftTimeOut2;
                            }
                        }

                        if (line is not null)
                        {
                            line.OfficialBusiness = line.OfficialBusiness;

                            if (!dtr.IsComputeRestDay)
                            {
                                line.RestDay = ComputeRestDay(line, shiftCodeDays);
                                line.OnLeave = ComputeOnLeave(line, _context);
                                line.Absent = ComputeAbsent(line, _context);
                                line.HalfdayAbsent = false;
                            }

                            var isEligibleForHolidayPay = true;
                            var isEmpProjectBased = employees.FirstOrDefault(x => x.Id == empId)?.EmploymentType ?? 0;

                            if (isEmpProjectBased == 3)
                            {
                                isEligibleForHolidayPay = false;
                            }

                            if (line.DayTypeId != 1)
                            {
                                var dateBefore = dayTypeDays.FirstOrDefault(x => x.Date.Date == line.Date.Date)?.DateBefore ?? DefaultDate;
                                var dateAfter = dayTypeDays.FirstOrDefault(x => x.Date.Date == line.Date.Date)?.DateAfter ?? DefaultDate;

                                var lineAbsentOnDateAfter = dtrLines.FirstOrDefault(x => x.EmployeeId == line.EmployeeId && x.Date == dateAfter);

                                var isAbsentOnDateBefore = dtrLines.FirstOrDefault(x => x.EmployeeId == line.EmployeeId && x.Date == dateBefore)?.Absent ?? true;
                                var isAbsentOnDateAfter = lineAbsentOnDateAfter?.TimeIn1 is null && lineAbsentOnDateAfter?.TimeIn2 is null;

                                var isLeaveDateBefore = dtrLines.FirstOrDefault(x => x.EmployeeId == line.EmployeeId && x.Date == dateBefore)?.OnLeave ?? false;
                                var isLeaveDateAfter = dtrLines.FirstOrDefault(x => x.EmployeeId == line.EmployeeId && x.Date == dateAfter)?.OnLeave ?? false;

                                if (isLeaveDateBefore)
                                {
                                    isAbsentOnDateBefore = false;
                                }

                                if (isLeaveDateAfter)
                                {
                                    isAbsentOnDateAfter = false;
                                }

                                var isDateBeforeRestDay = dtrLines.FirstOrDefault(x => x.EmployeeId == line.EmployeeId && x.Date == dateBefore)?.RestDay ?? false;
                                var isDateAfterRestDay = dtrLines.FirstOrDefault(x => x.EmployeeId == line.EmployeeId && x.Date == dateAfter)?.RestDay ?? false;

                                if (isDateBeforeRestDay)
                                {
                                    isAbsentOnDateBefore = false;
                                }

                                if (isDateAfterRestDay)
                                {
                                    isAbsentOnDateAfter = false;
                                }

                                if (isAbsentOnDateBefore || isAbsentOnDateAfter)
                                {
                                    isEligibleForHolidayPay = false;
                                }
                            }

                            line.RegularHours = ComputeRegularHours(line, shiftCodeDays, isEligibleForHolidayPay, _context);
                            line.NightHours = ComputeNightHours(line, shiftCodeDays);
                            line.OvertimeHours = ComputeOverTimeHours(line, _context);
                            line.OvertimeNightHours = ComputeOvertimeNightHours(line, _context);
                            line.GrossTotalHours = ComputeGrossTotalHours(line);
                            line.TardyLateHours = ComputeTardyLateHours(line, shiftCodeDays, employees, _context);
                            line.TardyUndertimeHours = string.IsNullOrEmpty(line.ShiftDates) ? ComputeTardyUndertimeHours(line, shiftCodeDays, _context) : ComputeTardyUndertimeHoursv2(line, shiftCodeDays, _context);

                            if (Math.Abs(line.TardyUndertimeHours) > 4) 
                            {
                                line.TardyUndertimeHours = ComputeTardyUndertimeHours(line, shiftCodeDays, _context);
                            }

                            line.NetTotalHours = ComputeNetTotalHours(line);
                            line.DayMultiplier = ComputeDayMultiplier(line, employees, dayTypeDays, _context);
                            line.RatePerHour = ComputeRatePerHour(line, employees);
                            line.RatePerNightHour = ComputeRatePerNightHour(line, employees);
                            line.RatePerOvertimeHour = ComputeRatePerOvertimeHour(line, employees);
                            line.RatePerOvertimeNightHour = ComputeRatePerOvertimeNightHour(line, employees);
                            line.RegularAmount = ComputeRegularAmount(line);
                            line.NightAmount = ComputeNightAmount(line, isEligibleForHolidayPay);                                                     

                            var emp = employees.FirstOrDefault(x => x.Id == line.EmployeeId);
                            var empPayrollTypeId = emp?.PayrollTypeId ?? 1;

                            if (emp is not null && emp.PayrollTypeId == 3 && line.DayTypeId > 1) 
                            {
                                line.Absent = false;
                            }

                            if (empPayrollTypeId != 3 && line.DayTypeId > 1 && !isEligibleForHolidayPay) 
                            {
                                if (line is not null && line.TimeIn1 is null && line.TimeOut1 is null && line.TimeIn2 is null && line.TimeOut2 is null) 
                                {
                                    line.Absent = true;
                                    line.RegularHours = 0;
                                    line.RegularAmount = 0;
                                }
                            }

                            if (DateOnly.FromDateTime(line.Date) == DateOnly.Parse("12/24/2024"))
                            {

                            }

                            line.OvertimeAmount = ComputeOverTimeAmount(line, employees, dayTypeDays, isEligibleForHolidayPay);
                            line.OvertimeNightAmount = ComputeOvertimeNightAmount(line, employees, dayTypeDays);

                            line.TotalAmount = ComputeTotalAmount(line, shiftCodeDays, isEligibleForHolidayPay, _context); //ComputeTotalAmount(line);
                            line.RatePerHourTardy = ComputeRatePerHourTardy(line, employees);
                            line.RatePerAbsentDay = ComputeRatePerAbsentDay(line, employees);
                            line.TardyAmount = ComputeTardyAmount(line, _context);
                            line.AbsentAmount = ComputeAbsentAmount(line);
                            line.NetAmount = ComputeNetAmount(line);

                            if (line.HalfdayAbsent)
                            {
                                if (line.TardyLateHours > line.TardyUndertimeHours)
                                {
                                    var tardyLateHours = line.TardyLateHours;
                                    line.TardyLateHours = 0;

                                    if (line.TimeOut1 == null && line.TimeIn2 == null)
                                    {
                                        line.TardyLateHours = tardyLateHours > (line.RegularHours / 2) ? (line.RegularHours / 2) : tardyLateHours;

                                        if (tardyLateHours > ((line.RegularHours / 2) + 1))
                                        {
                                            line.TardyLateHours = tardyLateHours - 1;
                                        }

                                        line.TardyAmount = ComputeTardyAmount(line, _context);

                                        //line.NetTotalHours++;
                                        line.RegularHours = line.RegularHours / 2;
                                        line.GrossTotalHours = line.NetTotalHours;
                                        line.NetAmount = ComputeNetAmount(line);

                                        line.TardyLateHours = 0;
                                        line.TardyAmount = 0;

                                        if (line.NightHours > line.RegularHours)
                                        {
                                            line.NightHours = line.RegularHours;
                                            line.NightAmount = ComputeNightAmount(line, isEligibleForHolidayPay);
                                        }
                                    }
                                    else
                                    {
                                        if (tardyLateHours > (line.RegularHours / 2))
                                        {
                                            line.TardyLateHours = tardyLateHours > (line.RegularHours / 2) ? (line.RegularHours / 2) : tardyLateHours;

                                            if (tardyLateHours > ((line.RegularHours / 2) + 1))
                                            {
                                                line.TardyLateHours = tardyLateHours - 1;
                                            }

                                            line.TardyAmount = ComputeTardyAmount(line, _context);

                                            //line.NetTotalHours++;
                                            line.RegularHours = line.RegularHours / 2;
                                            line.GrossTotalHours = line.NetTotalHours;
                                            line.NetAmount = ComputeNetAmount(line);

                                            line.TardyLateHours = 0;
                                            line.TardyAmount = 0;
                                        }
                                    }                                        
                                }

                                if (line.TardyUndertimeHours > line.TardyLateHours)
                                {
                                    var tardyUndertimeHours = line.TardyUndertimeHours;
                                    line.TardyUndertimeHours = 0;

                                    if (line.TimeOut1 == null && line.TimeIn2 == null)
                                    {
                                        line.TardyUndertimeHours = (line.RegularHours / 2);//tardyUndertimeHours > (line.RegularHours / 2) ? (line.RegularHours / 2) : tardyUndertimeHours;

                                        if (tardyUndertimeHours > ((line.RegularHours / 2) + 1))
                                        {
                                            line.TardyUndertimeHours = tardyUndertimeHours - 1;
                                        }

                                        line.TardyAmount = ComputeTardyAmount(line, _context);

                                        line.NetTotalHours = ComputeNetTotalHours(line);
                                        //line.NetTotalHours++;
                                        line.RegularHours = line.RegularHours / 2;
                                        line.GrossTotalHours = line.NetTotalHours;
                                        line.NetAmount = ComputeNetAmount(line);

                                        line.TardyUndertimeHours = 0;
                                        line.TardyAmount = 0;

                                        if (line.NightHours > line.RegularHours)
                                        {
                                            line.NightHours = line.RegularHours;
                                            line.NightAmount = ComputeNightAmount(line, isEligibleForHolidayPay);
                                        }
                                    }
                                    else 
                                    {
                                        if (tardyUndertimeHours > (line.RegularHours / 2))
                                        {
                                            line.TardyUndertimeHours = tardyUndertimeHours > (line.RegularHours / 2) ? (line.RegularHours / 2) : tardyUndertimeHours;

                                            if (tardyUndertimeHours > ((line.RegularHours / 2) + 1))
                                            {
                                                line.TardyUndertimeHours = tardyUndertimeHours - 1;
                                            }

                                            line.TardyAmount = ComputeTardyAmount(line, _context);

                                            line.NetTotalHours = ComputeNetTotalHours(line);
                                            //line.NetTotalHours++;
                                            line.RegularHours = line.RegularHours / 2;
                                            line.GrossTotalHours = line.NetTotalHours;
                                            line.NetAmount = ComputeNetAmount(line);

                                            line.TardyUndertimeHours = 0;
                                            line.TardyAmount = 0;
                                        }
                                    }                                        
                                }

                                line.TardyAmount = ComputeTardyAmount(line, _context);
                                line.TotalAmount = line.NetAmount;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Message:" + ex.Message);
                }
            }

            employees = null;
            shiftCodeDays = null;
            dayTypeDays = null;                      
        }

        internal static void QuickChangeLines(TrnDtr dtr, HRISContext _context)
        {
			var shiftCodeDays = _context.MstShiftCodeDays.ToArray();

			foreach (var line in dtr.TrnDtrlines)
            {
                if (line.EmployeeId == 361) 
                {
                
                }

                line.ShiftCodeId = QuickChangeShift(_context, line?.TimeIn1 ?? DefaultDate, 
                    line?.EmployeeId ?? 0, 
                    line?.Date ?? DefaultDate, 
                    line?.Dtr.ChangeShiftId ?? 0, 
                    line?.ShiftCodeId ?? 0);

                if (line is not null) 
                {
					var changeShiftId = line.Dtr.ChangeShiftId ?? 0;
					var employeeId = line.EmployeeId;
					var lineDate = line.Date;

					var changeShiftCodeId = _context.TrnChangeShiftLines
								?.FirstOrDefault(x => x.ChangeShiftId == changeShiftId
									&& x.EmployeeId == employeeId
									&& x.Date.Date == lineDate)
								?.ShiftCodeId ?? 0;

					if (changeShiftCodeId > 0)
					{
						var shiftCodeDay = shiftCodeDays.FirstOrDefault(x => x.ShiftCodeId == changeShiftCodeId && x.Day == line.Date.DayOfWeek.ToString());

						line.ShiftCodeId = changeShiftCodeId;

						if (shiftCodeDay is not null)
						{
							if (shiftCodeDay.TimeIn1 is not null && shiftCodeDay.TimeOut1 is null && shiftCodeDay.TimeIn2 is null && shiftCodeDay.TimeOut2 is not null)
							{
								shiftCodeDay.TimeIn1 = shiftCodeDay.TimeIn1.HasValue
									? line.Date.Add(shiftCodeDay.TimeIn1.Value.TimeOfDay)
									: (DateTime?)null;
								shiftCodeDay.TimeOut1 = null;
								shiftCodeDay.TimeIn2 = null;
								shiftCodeDay.TimeOut2 = shiftCodeDay.TimeOut2.HasValue
									? line.Date.Add(shiftCodeDay.TimeOut2.Value.TimeOfDay)
									: (DateTime?)null;

								//! Normal
								if (shiftCodeDay.TimeOut2 < shiftCodeDay.TimeIn1)
								{
									shiftCodeDay.TimeIn1 = shiftCodeDay.TimeIn1?.AddDays(-1);
								}

								//! IsTommorow
								if (shiftCodeDay.TimeOut2 < shiftCodeDay.TimeIn1 && shiftCodeDay.IsTommorow) 
                                {
									shiftCodeDay.TimeOut2 = shiftCodeDay.TimeOut2?.AddDays(1);
								}
							}
							else
							{
								shiftCodeDay.TimeIn1 = shiftCodeDay.TimeIn1.HasValue
									? line.Date.Add(shiftCodeDay.TimeIn1.Value.TimeOfDay)
									: (DateTime?)null;
								shiftCodeDay.TimeOut1 = shiftCodeDay.TimeOut1.HasValue
									? line.Date.Add(shiftCodeDay.TimeOut1.Value.TimeOfDay)
									: (DateTime?)null;
								shiftCodeDay.TimeIn2 = shiftCodeDay.TimeIn2.HasValue
									? line.Date.Add(shiftCodeDay.TimeIn2.Value.TimeOfDay)
									: (DateTime?)null;
								shiftCodeDay.TimeOut2 = shiftCodeDay.TimeOut2.HasValue
									? line.Date.Add(shiftCodeDay.TimeOut2.Value.TimeOfDay)
									: (DateTime?)null;

                                //! Normal
								if (shiftCodeDay.TimeOut1 < shiftCodeDay.TimeIn1)
								{
									shiftCodeDay.TimeIn1 = shiftCodeDay.TimeIn1?.AddDays(-1);
								}

								if (shiftCodeDay.TimeIn2 < shiftCodeDay.TimeOut1)
								{
									shiftCodeDay.TimeOut1 = shiftCodeDay.TimeOut1?.AddDays(-1);
								}

								if (shiftCodeDay.TimeOut2 < shiftCodeDay.TimeIn2)
								{
									shiftCodeDay.TimeIn2 = shiftCodeDay.TimeIn2?.AddDays(-1);
								}

								//! IsTommorow
								if (shiftCodeDay.TimeOut1 < shiftCodeDay.TimeIn1)
								{
									shiftCodeDay.TimeOut1 = shiftCodeDay.TimeOut1?.AddDays(1);
								}

								if (shiftCodeDay.TimeIn2 < shiftCodeDay.TimeOut1)
								{
									shiftCodeDay.TimeIn2 = shiftCodeDay.TimeIn2?.AddDays(1);
								}

								if (shiftCodeDay.TimeOut2 < shiftCodeDay.TimeIn2)
								{
									shiftCodeDay.TimeOut2 = shiftCodeDay.TimeOut2?.AddDays(1);
								}
							}

							line.ShiftDates = string.Join(",", shiftCodeDay.TimeIn1, shiftCodeDay.TimeOut1, shiftCodeDay.TimeIn2, shiftCodeDay.TimeOut2);
						}
					}
				}
			}

            shiftCodeDays = null;
        }

        internal static void QuickEditLines(TrnDtr dtr, EditDtrLinesByQuickEdit command, HRISContext _context)
        {
            var listOfEmployeeIds = new List<int>();
            var filteredLines = new List<TrnDtrline>();

            if (command is not null && dtr is not null && dtr?.TrnDtrlines is not null) 
            {
                listOfEmployeeIds = _context.MstEmployees
                    .Where(x => x.IsLocked)
                    .Select(x => x.Id)
                    .ToList();

                if (command.DepartmentId is not null)
                {
                    listOfEmployeeIds = _context
                        .MstEmployees
                        .Where(x => x.DepartmentId == command.DepartmentId)
                        .Select(x => x.Id).ToList();
                }

                if (command.EmployeeId is not null)
                {
                    listOfEmployeeIds.Clear();
                    listOfEmployeeIds.Add(command?.EmployeeId ?? 0);
                }

                foreach (var employeeId in listOfEmployeeIds)
                {
                    if (command is not null) 
                    {
                        for (var dtrDate = command.DateStart; dtrDate <= command?.DateEnd; dtrDate = dtrDate.AddDays(1))
                        {
                            var line = new TrnDtrline();

                            line.Dtrid = command?.DTRId ?? 0;
                            line.EmployeeId = employeeId;
                            line.Date = dtrDate;
                            line.ShiftCodeId = ComputeShiftCode(null, employeeId, dtrDate, _context);
                            line.TimeIn1 = command?.TimeIn1 == null ? null : DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {command?.TimeIn1?.ToString("hh:mm tt")}");
                            line.TimeOut1 = command?.TimeOut1 == null ? null: DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {command?.TimeOut1?.ToString("hh:mm tt")}");
                            line.TimeIn2 = command?.TimeIn2 == null ? null : DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {command?.TimeIn2?.ToString("hh:mm tt")}");
                            line.TimeOut2 = command?.TimeOut2 == null ? null : DateTime.Parse($"{line.Date.ToString("MM/dd/yyyy")} {command?.TimeOut2?.ToString("hh:mm tt")}");
                            line.OfficialBusiness = false;
                            line.OnLeave = false;
                            line.Absent = false;
                            line.HalfdayAbsent = false;
                            line.RegularHours = 0;
                            line.NightHours = 0;
                            line.OvertimeHours = 0;
                            line.OvertimeNightHours = 0;
                            line.GrossTotalHours = 0;
                            line.TardyLateHours = 0;
                            line.TardyUndertimeHours = 0;
                            line.NetTotalHours = 0;
                            line.DayTypeId = ComputeDayType(employeeId, dtrDate, _context);
                            line.RestDay = false;
                            line.DayMultiplier = 1;
                            line.RatePerHour = 0;
                            line.RatePerNightHour = 0;
                            line.RatePerOvertimeHour = 0;
                            line.RatePerOvertimeNightHour = 0;
                            line.RegularAmount = 0;
                            line.NightAmount = 0;
                            line.OvertimeAmount = 0;
                            line.OvertimeNightAmount = 0;
                            line.TotalAmount = 0;
                            line.RatePerHourTardy = 0;
                            line.RatePerAbsentDay = 0;
                            line.TardyAmount = 0;
                            line.AbsentAmount = 0;
                            line.NetAmount = 0;
                             
                            dtr.TrnDtrlines.Add(line);
                        }
                    }
                }
            }
        }
        #endregion

        public static List<TmpDtrLogs>? ProcessLogsFromDb(int? departmentId, int? employeeId, DateTime startDate, DateTime endDate, HRISContext _context) 
        {
            var logs = new List<TmpDtrLogs>();
            var employees = GetEmployees(_context);
            var shiftCodeDays = GetShiftCodeDays(_context);

            var empIds = new List<int>();

            empIds = GetEmployeeIds(departmentId, _context);

            if (employeeId is not null)
            {
                empIds = new List<int> { employeeId ?? 0 };
            }

            foreach (var empId in empIds)
            {
                var companyId = Common.Lookup.GetCompanyIdByEmployeeId(empId);

                var noField = Common.Lookup.GetDTRNoFieldByCompanyId(companyId);
                var dateTimeField = Common.Lookup.GetDTRDateTimeFieldByCompanyId(companyId);
                var logTypeField = Common.Lookup.GetDTRLogTypeFieldByCompanyId(companyId);

                var bioId = Common.Lookup.GetBioIdByEmployeeId(empId);

                var table = _context.TrnLogs
                    .Where(x => x.BiometricIdNumber == bioId && x.LogType != "X");

                if (table.Count() == 0)
                {
                    continue;
                }

                var dateOnlyStart = DateOnly.FromDateTime(startDate);
                var dateOnlyEnd = DateOnly.FromDateTime(endDate);

                try
                {
                    var table1 = table
                        .Where(row => row.LogDateTime.HasValue &&
                            row.LogDateTime.Value.Date >= startDate.Date &&
                            row.LogDateTime.Value.Date <= endDate.Date);

                    foreach (var row in table1)
                    {
                        logs.Add(new TmpDtrLogs()
                        {
                            EmployeeId = Common.Lookup.GetEmployeeIdByBioId(row.BiometricIdNumber ?? ""),
                            Date = DateOnly.FromDateTime(row.LogDateTime ?? new DateTime(1990, 09, 15)),
                            Time = TimeOnly.FromDateTime(row.LogDateTime ?? new DateTime(1990, 09, 15)),
                            LogType = row.LogType?.ToString()
                        });
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }

            return logs;
        }
    }
}
