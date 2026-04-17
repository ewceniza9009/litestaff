// =============================================================================
// DtrEngine.cs  —  Industry-standard refactor of DTR.cs
//
// MIGRATION GUIDE:
//   Step 1: In DTR.cs, delete the entire `public class DTR { ... }` block only.
//           Leave the `DtrBatchProcessor` class in DTR.cs — it is shared by both.
//   Step 2: This file adds `DtrEngine` to the same namespace.
//   Step 3: Update all callers: rename `DTR.Xxx(...)` → `DtrEngine.Xxx(...)`.
//
// CHANGES (no business logic was altered):
//   ✓ Named constants replace magic numbers (DayTypeId, PayrollTypeId)
//   ✓ Dead debug breakpoint blocks removed
//   ✓ ~700 lines of commented-out code removed
//   ✓ Duplicate ComputeOverTimeAmount / ComputeOvertimeNightAmount calls fixed
//   ✓ `goto` replaced with a bounded while-loop (ProcessDtrLines)
//   ✓ `if (true)` wrapper removed (ComputeRegularHours)
//   ✓ Repeated ShiftCodeDay lookups consolidated into local variables
//   ✓ DateTime.Parse string formatting replaced with $"{dt:hh:mm tt}" interpolation
//   ✓ Unused `using` directives removed
//   ✓ ComputeRestDay simplified (redundant .Any() guard removed)
//   ✓ GetLeaveDetails() helper extracts duplicated leave-lookup pattern
//   ✓ HasAnyTimeSwipe() / HasNoTimeSwipes() helpers reduce repetition
//   ✓ XML doc comments added on the public surface
// =============================================================================

using Microsoft.EntityFrameworkCore;
using whris.Application.CQRS.TrnDtr.Commands;
using whris.Application.Dtos;
using whris.Application.Queries.TrnDtr;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Library
{
    // NOTE: DtrBatchProcessor is defined in DTR.cs and shared by both classes.
    //       It does NOT need to be duplicated here.

    // =========================================================================
    /// <summary>
    /// DtrEngine — refactored replacement for the <c>DTR</c> class.
    /// All static methods compute DTR line values (hours, amounts, tardy deductions).
    /// </summary>
    // =========================================================================
    public class DtrEngine
    {
        // ── Day Type IDs (MstDayType) ─────────────────────────────────────────
        public const int DayTypeWorking = 1;
        public const int DayTypeRegularHoliday = 2;
        public const int DayTypeSpecialHoliday = 3;

        // ── Payroll Type IDs (MstPayrollType) ────────────────────────────────
        private const int PayrollTypeVariable = 1;
        private const int PayrollTypeFixed = 2;
        private const int PayrollTypeProjectBased = 3;

        // ── Employment Type IDs ───────────────────────────────────────────────
        private const int EmploymentTypeProjectBased = 3;

        // ── Sentinel "no date" values ─────────────────────────────────────────
        private static readonly DateTime DefaultDate = new(1990, 09, 15);
        private static readonly DateOnly DefaultDateOnly = new(1990, 09, 15);

        // ── Night Differential Window ─────────────────────────────────────────
        private const string NightShiftStartTime = "10:00 PM";
        private const string NightShiftEndTime = "06:00 AM";

        // ── Time Boundary Sentinels ───────────────────────────────────────────
        private const string MidnightTimeStr = "12:00 AM";
        private const string NoonTimeStr = "12:00 PM";
        private const string MidnightPlusOneAmStr = "12:01 AM"; // "unset" sentinel for shift start detection
        private const string MidnightPlusOnePmStr = "12:01 PM"; // "unset" sentinel for shift start detection

        // ── Shift / Tardy Hour Thresholds ─────────────────────────────────────
        private const int OvernightSpanHours = 20; // shift span ≥ 20 h → overnight boundary
        private const int TardyDiffBoundaryHours = 20; // |tardy diff| > 20 h → apply AddDays adj.
        private const int LongShiftGapHours = 12; // gap > 12 h → implied day/shift boundary
        private const int LongShiftMaxHours = 36; // gap > 36 h → flag NoTimeOut2
        private const int FlexWorkWindowHours = 19; // flex shift scan window
        private const int WeekIntervalBoundaryHours = 14; // next-week shift interval threshold
        private const int ShiftOverrideDiscThreshold = 18; // change-shift time-override tolerance (h)

        // ── Tomorrow-shift gap tolerance (hours) ──────────────────────────────
        private const decimal TomorrowShiftGapMin = -4m;
        private const decimal TomorrowShiftGapMax = 4m;

        // ── Shift Resolution Sentinels ────────────────────────────────────────
        private const string UnknownDayKey = "NA";
        private const string StraightShiftRemark = "STRAIGHT";
        private const string DefaultShiftCodeName = "DEFAULT";

        // ── Log Type Codes ────────────────────────────────────────────────────
        private const string LogTypeIn = "I"; // TimeIn / first-in swipe
        private const string LogTypeOut = "O"; // TimeOut / last-out swipe
        private const string LogTypeBreakStart = "0"; // mid-shift out (break start)
        private const string LogTypeBreakEnd = "1"; // mid-shift in  (break end)
        private const string LogTypeExcluded = "X"; // ignored log entry

        // ── Night-Differential Pay Multipliers ────────────────────────────────
        private const decimal NightMultRestRegHolElig = 2.6m; // rest day + regular holiday, eligible
        private const decimal NightMultRestSpcHolElig = 1.5m; // rest day + special holiday, eligible
        private const decimal NightMultRestWorkElig = 1.3m; // rest day + working day, eligible
        private const decimal NightMultRegHolElig = 2.0m; // regular holiday (no rest), eligible
        private const decimal NightMultSpcHolElig = 1.3m; // special holiday (no rest), eligible

        // ── Overtime Pay Multipliers ──────────────────────────────────────────
        private const decimal OtMultRegHolElig = 2.60m; // OT regular holiday, eligible
        private const decimal OtMultSpcHolElig = 1.69m; // OT special holiday, eligible
        private const decimal OtMultWorking = 1.25m; // OT working day (base)
        private const decimal OtMultRestRegHolElig = 3.38m; // OT rest-day + regular holiday, eligible
        private const decimal OtMultRestRegHolNotElig = 1.69m; // OT rest-day + regular holiday, not eligible
        private const decimal OtMultRestSpcHolElig = 1.95m; // OT rest-day + special holiday, eligible
        private const decimal OtMultRestSpcHolNotElig = 1.69m; // OT rest-day + special holiday, not eligible
        private const decimal OtMultRestWorking = 1.69m; // OT rest-day + working day

        // ── Overtime Night Amount Factors ─────────────────────────────────────
        private const decimal OtNightFixedAddendum = 1.0m; // additional multiplier addendum (fixed payroll)
        private const decimal OtNightRegHolAddendum = 0.6m; // regular holiday addendum
        private const decimal OtNightSpcHolFactorFixed = 0.3m; // special holiday factor (fixed, excluded)
        private const decimal OtNightSpcHolFactorVar = 1.3m; // special holiday factor (variable)

        // ── Day-Multiplier Fallbacks ──────────────────────────────────────────
        private const decimal DayMultRestDayFallback = 1.3m; // rest-day multiplier when holiday ineligible
        private const decimal DayMultHolidayWorkFactor = 1.6m; // variable bonus for working before reg. holiday

        // ── Shift Window Scan Buffers (hours) ─────────────────────────────────
        private const double ShiftWindowLookBackHours = -4; // look-back buffer before shift TimeIn1
        private const double ShiftWindowLookAheadHours = 8; // look-ahead buffer after shift TimeOut2
        private const double ShiftScanBufferBefore = -4; // shift code resolution: pre-window
        private const double ShiftScanBufferAfter = 7; // shift code resolution: post-window
        private const double WeekRolloverCheckHours = 2; // next-log gap that triggers week rollover
        private const double FlexBreakGapEstimateHours = 1; // estimated break gap for missing flex swipes

        // ── Private helpers ───────────────────────────────────────────────────
        private static bool HasAnyTimeSwipe(TrnDtrline line) =>
            line.TimeIn1 is not null
            || line.TimeOut1 is not null
            || line.TimeIn2 is not null
            || line.TimeOut2 is not null;

        private static bool HasNoTimeSwipes(TrnDtrline line) =>
            line.TimeIn1 is null
            && line.TimeOut1 is null
            && line.TimeIn2 is null
            && line.TimeOut2 is null;

        /// <summary>
        /// Fetches leave pay status and hours for a DTR line from its linked leave application.
        /// Returns (false, 0) when no matching leave application line exists.
        /// </summary>
        private static (bool WithPay, decimal Hours) GetLeaveDetails(
            TrnDtrline line,
            HRISContext context
        )
        {
            var leaveApplicationId =
                context.TrnDtrs.FirstOrDefault(x => x.Id == line.Dtrid)?.LeaveApplicationId ?? 0;

            if (leaveApplicationId == 0 || line.EmployeeId == 0)
                return (false, 0);

            var leaveApp = context.TrnLeaveApplicationLines.FirstOrDefault(
                x =>
                    x.LeaveApplicationId == leaveApplicationId
                    && x.EmployeeId == line.EmployeeId
                    && x.Date.Date == line.Date.Date
            );

            return leaveApp is not null ? (leaveApp.WithPay, leaveApp.NumberOfHours) : (false, 0);
        }

        #region Shift & Status Resolution

        /// <summary>
        /// Resolves the effective shift code. Priority: ChangeShift → MstEmployeeShiftCodes → MstEmployee.
        /// </summary>
        public static int ComputeShiftCode(
            int? changeShiftId,
            int? employeeId,
            DateTime dtrDate,
            HRISContext context
        )
        {
            if (changeShiftId is not null)
            {
                var fromChangeShift =
                    context.TrnChangeShiftLines?.FirstOrDefault(
                        x =>
                            x.ChangeShiftId == changeShiftId
                            && x.EmployeeId == employeeId
                            && x.Date.Date == dtrDate.Date
                    )?.ShiftCodeId ?? 0;

                if (fromChangeShift > 0)
                    return fromChangeShift;

                return context.MstEmployeeShiftCodes?.FirstOrDefault(
                        x => x.EmployeeId == employeeId
                    )?.ShiftCodeId ?? 0;
            }

            return context.MstEmployees?.FirstOrDefault(x => x.Id == employeeId)?.ShiftCodeId ?? 0;
        }

        /// <summary>Returns true when the line's day-of-week is marked as a rest day in the shift code.</summary>
        public static bool ComputeRestDay(
            TrnDtrline line,
            IEnumerable<MstShiftCodeDay> shiftCodeDays
        )
        {
            var dayOfWeek = line.Date.ToString("dddd").ToUpperInvariant();
            return shiftCodeDays.FirstOrDefault(
                    x => x.ShiftCodeId == line.ShiftCodeId && x.Day.ToUpperInvariant() == dayOfWeek
                )?.RestDay ?? false;
        }

        /// <summary>Returns true when the employee has an approved leave on the DTR date.</summary>
        public static bool ComputeOnLeave(TrnDtrline line, HRISContext context)
        {
            var leaveApplicationId =
                context.TrnDtrs.FirstOrDefault(x => x.Id == line.Dtrid)?.LeaveApplicationId ?? 0;

            if (leaveApplicationId == 0 || line.EmployeeId == 0)
                return false;

            return context.TrnLeaveApplicationLines.Any(
                x =>
                    x.LeaveApplicationId == leaveApplicationId
                    && x.EmployeeId == line.EmployeeId
                    && x.Date.Date == line.Date.Date
            );
        }

        /// <summary>
        /// Returns true when the employee is absent. Handles regular absent, leave-without-pay,
        /// holiday absent (with WithAbsentInFixed check for fixed-rate employees).
        /// </summary>
        public static bool ComputeAbsent(TrnDtrline line, HRISContext context)
        {
            var noSwipes = HasNoTimeSwipes(line);

            // Regular absent on a working day
            if (
                noSwipes
                && !line.OfficialBusiness
                && !line.OnLeave
                && !line.RestDay
                && line.DayTypeId == DayTypeWorking
            )
                return true;

            // On leave with no swipes on a working day
            if (
                noSwipes
                && !line.OfficialBusiness
                && line.OnLeave
                && !line.RestDay
                && line.DayTypeId == DayTypeWorking
            )
            {
                var isWithPay =
                    context.TrnLeaveApplicationLines.FirstOrDefault(
                        x => x.EmployeeId == line.EmployeeId && x.Date.Date == line.Date.Date
                    )?.WithPay ?? false;

                if (!isWithPay)
                {
                    var payrollTypeId =
                        context.MstEmployees?.FirstOrDefault(
                            x => x.Id == line.EmployeeId
                        )?.PayrollTypeId ?? 0;

                    return payrollTypeId != PayrollTypeFixed;
                }
                return false;
            }

            // No swipes on a holiday
            if (
                noSwipes
                && !line.OfficialBusiness
                && !line.OnLeave
                && !line.RestDay
                && line.DayTypeId != DayTypeWorking
            )
            {
                var payrollTypeId =
                    context.MstEmployees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId
                    ?? 0;

                if (payrollTypeId == PayrollTypeFixed)
                {
                    return context.MstDayTypeDays.FirstOrDefault(
                            x => x.Date.Date == line.Date.Date
                        )?.WithAbsentInFixed ?? false;
                }
            }

            return false;
        }

        /// <summary>Returns the day type ID for the employee's branch on the given date (default: 1).</summary>
        public static int ComputeDayType(int? employeeId, DateTime dtrDate, HRISContext context)
        {
            var branchId =
                context.MstEmployees.FirstOrDefault(x => x.Id == employeeId)?.BranchId ?? 0;

            return context.MstDayTypeDays?.FirstOrDefault(
                    x => x.Date.Date == dtrDate.Date && x.BranchId == branchId
                )?.DayTypeId ?? DayTypeWorking;
        }

        #endregion

        #region Hours Computation

        /// <summary>
        /// Computes regular (non-overtime) hours. Factors in swipes, holiday eligibility,
        /// leave applications, and payroll type.
        /// </summary>
        public static decimal ComputeRegularHours(
            TrnDtrline line,
            IEnumerable<MstShiftCodeDay> shiftCodeDays,
            bool isEligibleForHolidayPay,
            HRISContext context
        )
        {
            var lineDay = line.Date.ToString("dddd").ToUpperInvariant();
            var shiftCodeDay = shiftCodeDays?.FirstOrDefault(
                x => x.ShiftCodeId == line.ShiftCodeId && x.Day.ToUpperInvariant() == lineDay
            );

            if (shiftCodeDay is null)
                return 0;

            // ── Has at least one swipe ────────────────────────────────────────
            if (HasAnyTimeSwipe(line))
            {
                if (line.TimeIn1 is null && line.TimeOut1 is null)
                {
                    if (line.Employee.IsFlexBreak == false)
                    {
                        line.HalfdayAbsent = false;
                        return 0;
                    }
                    line.HalfdayAbsent = true;
                    return shiftCodeDay.NumberOfHours / 2;
                }

                if (line.TimeIn2 is null && line.TimeOut2 is null)
                {
                    if (line.Employee.IsFlexBreak == false)
                    {
                        line.HalfdayAbsent = false;
                        return 0;
                    }
                    line.HalfdayAbsent = true;
                    return shiftCodeDay.NumberOfHours / 2;
                }

                return shiftCodeDay.NumberOfHours;
            }

            // ── No swipes — holiday rest-day shortcut ─────────────────────────
            if (isEligibleForHolidayPay && line.DayTypeId > DayTypeWorking && line.RestDay)
            {
                var dayTypeId = line?.DayTypeId ?? DayTypeWorking;
                if (
                    !(line?.Absent ?? true)
                    && dayTypeId == DayTypeSpecialHoliday
                    && HasNoTimeSwipes(line!)
                )
                    return 0;
                return shiftCodeDay.NumberOfHours;
            }

            // ── No swipes — check leave application ───────────────────────────
            var leaveApplicationId =
                context.TrnDtrs.FirstOrDefault(x => x.Id == line.Dtrid)?.LeaveApplicationId ?? 0;

            if (leaveApplicationId != 0 && line.EmployeeId != 0)
            {
                var leaveApplication = context.TrnLeaveApplicationLines.FirstOrDefault(
                    x =>
                        x.LeaveApplicationId == leaveApplicationId
                        && x.EmployeeId == line.EmployeeId
                        && x.Date.Date == line.Date.Date
                );

                if (leaveApplication is not null)
                {
                    if (leaveApplication.WithPay)
                        return leaveApplication.NumberOfHours;
                }
            }

            // ── No swipes — payroll type & day type checks ────────────────────
            var payrollTypeId =
                context.MstEmployees?.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId
                ?? 0;

            var branchId =
                context.MstEmployees?.FirstOrDefault(x => x.Id == line.EmployeeId)?.BranchId ?? 0;

            var dayTypeDay = context.MstDayTypeDays.FirstOrDefault(
                x => x.Date.Date == line.Date.Date && x.BranchId == branchId
            );

            if (payrollTypeId == PayrollTypeProjectBased)
            {
                var dayTypeIdForProject = dayTypeDay?.DayTypeId ?? DayTypeWorking;
                if (
                    line is not null
                    && HasNoTimeSwipes(line)
                    && dayTypeIdForProject > DayTypeWorking
                )
                    return 0;
            }

            // ── Holiday pay eligibility (previously wrapped in `if (true)`) ──
            var dayType = 0;
            if (dayTypeDay is not null)
                dayType = dayTypeDay.DayTypeId;

            if (dayType == DayTypeSpecialHoliday && HasNoTimeSwipes(line))
                return 0;

            if (line.DayTypeId > DayTypeWorking)
            {
                if (isEligibleForHolidayPay)
                {
                    if (line is not null && line.RestDay && HasNoTimeSwipes(line))
                        return 0;
                    return shiftCodeDay.NumberOfHours;
                }
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// Computes night differential hours (10 PM – 6 AM window),
        /// capped by the shift's configured NightHours.
        /// </summary>
        public static decimal ComputeNightHours(
            TrnDtrline line,
            IEnumerable<MstShiftCodeDay> shiftCodeDays
        )
        {
            if (line.Absent)
                return 0;

            var nightTimeStart = DateTime.Parse($"{line.Date:MM/dd/yyyy} {NightShiftStartTime}");
            var nightTimeEnd = DateTime.Parse(
                $"{line.Date.AddDays(1):MM/dd/yyyy} {NightShiftEndTime}"
            );

            var tIn1Date = (line.TimeIn1 ?? DefaultDate).Date;
            var is2Swipes = line.TimeIn1 is not null && line.TimeOut2 is not null;
            var is4Swipes = is2Swipes && line.TimeOut1 is not null && line.TimeIn2 is not null;

            if (!is2Swipes && !is4Swipes)
                return 0;

            if (tIn1Date < line.Date)
            {
                nightTimeStart = nightTimeStart.AddDays(-1);
                nightTimeEnd = nightTimeEnd.AddDays(-1);
            }
            if (tIn1Date > line.Date)
            {
                nightTimeStart = nightTimeStart.AddDays(1);
                nightTimeEnd = nightTimeEnd.AddDays(1);
            }

            var shiftDay = shiftCodeDays.FirstOrDefault(
                x => x.ShiftCodeId == line?.ShiftCodeId && x.Day == line.Date.ToString("dddd")
            );
            var stOut1Date = line?.TimeOut1 ?? line?.Date ?? DefaultDate;
            var stIn2Date = line?.TimeIn2 ?? line?.Date ?? DefaultDate;
            var shiftTimeOut1 = DateTime.Parse(
                $"{stOut1Date:MM/dd/yyyy} {shiftDay?.TimeOut1 ?? DefaultDate:hh:mm tt}"
            );
            var shiftTimeIn2 = DateTime.Parse(
                $"{stIn2Date:MM/dd/yyyy} {shiftDay?.TimeIn2 ?? DefaultDate:hh:mm tt}"
            );

            var actualNightStart = line?.TimeIn1 ?? DefaultDate;
            var actualNightEnd = line?.TimeOut2 ?? DefaultDate;

            if (line is not null && line.TimeIn1 >= nightTimeStart && line.TimeIn1 > nightTimeEnd)
            {
                actualNightStart = line.TimeIn1 ?? DefaultDate;
            }
            else
            {
                if (line is not null && line.TimeIn1 < nightTimeStart)
                {
                    actualNightStart = nightTimeStart;
                    var shiftSpan = (decimal)(
                        (line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)
                    ).TotalHours;
                    if (shiftSpan > OvernightSpanHours)
                        actualNightStart = line.TimeIn1 ?? DefaultDate;
                }
                else
                {
                    if (line?.TimeIn2 is not null && line.TimeIn2 > nightTimeStart)
                        actualNightStart =
                            line.TimeIn2 > shiftTimeIn2
                                ? line.TimeIn2 ?? DefaultDate
                                : shiftTimeIn2;
                }
            }

            if (
                line?.TimeOut2 is not null
                && line.TimeOut2 <= nightTimeEnd
                && line.TimeOut2 > nightTimeStart
            )
                actualNightEnd = line.TimeOut2 ?? DefaultDate;
            else
                actualNightEnd = nightTimeEnd;

            var numberOfHours = (decimal)(actualNightEnd - actualNightStart).TotalHours;

            var nightHoursCap =
                shiftCodeDays.FirstOrDefault(
                    x =>
                        x.ShiftCodeId == (line?.ShiftCodeId ?? 0)
                        && x.Day.ToUpperInvariant()
                            == (line?.Date.ToString("dddd") ?? UnknownDayKey).ToUpperInvariant()
                )?.NightHours ?? 0;

            if (numberOfHours > nightHoursCap)
                numberOfHours = nightHoursCap;
            if (numberOfHours < 0)
                numberOfHours = 0;

            return Math.Round(numberOfHours, 5);
        }

        /// <summary>
        /// Computes approved overtime hours, capped by the OvertimeLimitHours on the OT application.
        /// </summary>
        public static decimal ComputeOverTimeHours(TrnDtrline line, HRISContext context)
        {
            var overTimeId = line.Dtr?.OvertTimeId ?? 0;
            var overtimeLine = context.TrnOverTimeLines.FirstOrDefault(
                x =>
                    x.EmployeeId == line.EmployeeId
                    && x.OverTimeId == overTimeId
                    && x.Date.Date == line.Date.Date
            );

            var otHours = overtimeLine?.OvertimeHours ?? 0;
            var otLimitHours = overtimeLine?.OvertimeLimitHours ?? 0;

            if (otHours > 0 && otHours > otLimitHours)
                otHours = otLimitHours;

            return Math.Round(otHours, 5);
        }

        /// <summary>Computes overtime night hours, capped by total actual hours worked.</summary>
        public static decimal ComputeOvertimeNightHours(TrnDtrline line, HRISContext context)
        {
            var overTimeId = line.Dtr?.OvertTimeId ?? 0;
            var otHours =
                context.TrnOverTimeLines.FirstOrDefault(
                    x =>
                        x.EmployeeId == line.EmployeeId
                        && x.OverTimeId == overTimeId
                        && x.Date.Date == line.Date.Date
                )?.OvertimeNightHours ?? 0;

            if (otHours > 0)
            {
                var totalWorkHours = (decimal)(
                    (line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)
                ).TotalHours;
                totalWorkHours = totalWorkHours < 0 ? 0 : totalWorkHours;
                if (otHours > totalWorkHours)
                    otHours = totalWorkHours;
            }

            return Math.Round(otHours, 5);
        }

        /// <summary>Gross total = Regular + Overtime + OvertimeNight hours.</summary>
        public static decimal ComputeGrossTotalHours(TrnDtrline line) =>
            line.RegularHours + line.OvertimeHours + line.OvertimeNightHours;

        /// <summary>Net total = Gross total minus both tardy deduction buckets.</summary>
        public static decimal ComputeNetTotalHours(TrnDtrline line) =>
            line.RegularHours
            + line.OvertimeHours
            + line.OvertimeNightHours
            - line.TardyLateHours
            - line.TardyUndertimeHours;

        #endregion

        #region Tardy Computation

        /// <summary>
        /// Computes late-arrival (tardy) hours by comparing the actual TimeIn1
        /// against the scheduled shift start, after applying the grace period.
        /// </summary>
        public static decimal ComputeTardyLateHours(
            TrnDtrline line,
            IEnumerable<MstShiftCodeDay> shiftCodeDays,
            IEnumerable<MstEmployee> employees,
            HRISContext context
        )
        {
            var numberOfHours = 0m;
            var lineDay = line.Date.ToString("dddd").ToUpperInvariant();
            var dateMidnight = DateTime.Parse($"{DateTime.Now.Date:MM/dd/yyyy} {MidnightTimeStr}");

            // Resolve single shift-day entry once
            var shiftDay = shiftCodeDays.FirstOrDefault(
                x => x.ShiftCodeId == line.ShiftCodeId && x.Day.ToUpperInvariant() == lineDay
            );

            var shiftTimeIn1 = DateTime.Parse(
                $"{line.Date:MM/dd/yyyy} {shiftDay?.TimeIn1?.ToString("HH:mm tt") ?? dateMidnight.ToString("HH:mm tt")}"
            );
            var shiftTimeIn2 = DateTime.Parse(
                $"{line.Date:MM/dd/yyyy} {shiftDay?.TimeIn2?.ToString("HH:mm tt") ?? dateMidnight.ToString("HH:mm tt")}"
            );
            var shiftTimeOut1 = DateTime.Parse(
                $"{line.Date:MM/dd/yyyy} {shiftDay?.TimeOut1?.ToString("HH:mm tt") ?? dateMidnight.ToString("HH:mm tt")}"
            );
            var shiftTimeOut2 = DateTime.Parse(
                $"{line.Date:MM/dd/yyyy} {shiftDay?.TimeOut2?.ToString("HH:mm tt") ?? dateMidnight.ToString("HH:mm tt")}"
            );

            var graceMinutes = shiftDay?.LateGraceMinute ?? 0;
            var flexHours = shiftDay?.LateFlexibility ?? 0;
            var payrollTypeId =
                employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

            shiftTimeIn1 = shiftTimeIn1.AddMinutes((double)graceMinutes);
            shiftTimeIn2 = shiftTimeIn2.AddMinutes((double)graceMinutes);

            // Overnight shift adjustment
            var isTomorrowShift = shiftCodeDays.Any(
                x => x.ShiftCodeId == line.ShiftCodeId && x.IsTommorow
            );
            if (isTomorrowShift)
            {
                var shiftTimeIn1InShiftDates = DefaultDate;
                if (line.ShiftDates is not null && line.ShiftDates.Length > 0)
                    shiftTimeIn1InShiftDates = DateTime.Parse(line.ShiftDates.Split(",")[0]);

                if (
                    shiftTimeIn1.Date != line.Date.Date
                    || (
                        line.ShiftDates is not null
                        && shiftTimeIn1InShiftDates.Date != line.Date.Date
                    )
                )
                    shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
            }

            if (line?.TimeIn2 is not null)
            {
                var tIn1 = TimeOnly.FromDateTime(
                    DateTime.Parse($"{line.Date:MM/dd/yyyy} {line.TimeIn1:hh:mm tt}")
                );
                var tIn2 = TimeOnly.FromDateTime(
                    DateTime.Parse($"{line.Date:MM/dd/yyyy} {line.TimeIn2:hh:mm tt}")
                );
                if (tIn1 > tIn2)
                    shiftTimeIn2 = shiftTimeIn2.AddDays(1);
            }

            if (
                line?.TimeIn1 is not null
                && shiftTimeIn1 != DateTime.Parse($"{line.Date:MM/dd/yyyy} {MidnightTimeStr}")
            )
            {
                var diff = (decimal)((line.TimeIn1 ?? DefaultDate) - shiftTimeIn1).TotalHours;
                if (diff < -TardyDiffBoundaryHours)
                {
                    shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
                    numberOfHours += (decimal)(
                        (line.TimeIn1 ?? DefaultDate) - shiftTimeIn1
                    ).TotalHours;
                }
                else if (diff > TardyDiffBoundaryHours)
                {
                    shiftTimeIn1 = shiftTimeIn1.AddDays(1);
                    numberOfHours += (decimal)(
                        (line.TimeIn1 ?? DefaultDate) - shiftTimeIn1
                    ).TotalHours;
                }
                else
                {
                    numberOfHours += diff;
                }
            }
            else
            {
                if (
                    line?.TimeIn1 is not null
                    && line.TimeOut1 is not null
                    && line.TimeIn2 is not null
                    && line.TimeOut2 is not null
                )
                {
                    if (
                        shiftTimeOut1 != DateTime.Parse($"{line.Date:MM/dd/yyyy} {MidnightTimeStr}")
                    )
                    {
                        shiftTimeOut1 = shiftTimeOut1.AddMinutes((double)graceMinutes);
                        numberOfHours += (decimal)(
                            (line?.TimeIn1 ?? DefaultDate) - shiftTimeOut1
                        ).TotalHours;
                    }
                }
                else if (
                    line?.TimeIn1 is null
                    && line?.TimeOut1 is null
                    && line?.TimeIn2 is not null
                    && line?.TimeOut2 is not null
                )
                {
                    shiftTimeOut2 = shiftTimeOut2.AddMinutes((double)graceMinutes);
                    numberOfHours += (decimal)(
                        shiftTimeOut2 - (line?.TimeIn2 ?? DefaultDate)
                    ).TotalHours;
                }
            }

            if (numberOfHours < 0)
                numberOfHours = 0;
            if (payrollTypeId == PayrollTypeFixed)
                numberOfHours = 0;

            if (line?.OnLeave ?? false)
            {
                var (leaveWithPay, leaveWithHours) = GetLeaveDetails(line, context);
                if (leaveWithPay && !line.Absent)
                    numberOfHours =
                        numberOfHours > leaveWithHours ? numberOfHours - leaveWithHours : 0;
                else
                    numberOfHours =
                        numberOfHours < 0
                            ? Math.Round(numberOfHours, 0)
                            : Math.Abs(Math.Round(numberOfHours, 5));
            }
            else
            {
                if (numberOfHours > 0 && !(line?.Absent ?? false))
                    numberOfHours = flexHours > 0 ? 0 : Math.Round(numberOfHours, 5);
                else
                    numberOfHours = 0;
            }

            if (numberOfHours > 0)
            {
                var empId = line?.EmployeeId ?? 0;
                if (employees.FirstOrDefault(x => x.Id == empId)?.IsFlex ?? false)
                    return 0;
                if (employees.FirstOrDefault(x => x.Id == empId)?.IsFlexBreak ?? false)
                    return 0;
                return Math.Round(numberOfHours, 5);
            }

            return 0;
        }

        /// <summary>
        /// Computes undertime hours from the shift schedule.
        /// Used when <see cref="TrnDtrline.ShiftDates"/> is <c>null</c> or empty.
        /// </summary>
        public static decimal ComputeTardyUndertimeHours(
            TrnDtrline line,
            IEnumerable<MstShiftCodeDay> shiftCodeDays,
            HRISContext context
        )
        {
            var numberOfHours = 0m;
            var actualHours = 0m;
            var shiftTimeIn1 = DefaultDate;
            var shiftTimeIn2 = DefaultDate;
            var shiftTimeOut2 = DefaultDate;
            var shiftNumberOfHours = 0m;
            var shiftCodeId = 0;
            var isFlexBreak =
                context.MstEmployees?.FirstOrDefault(x => x.Id == line.EmployeeId)?.IsFlexBreak
                ?? false;

            if (!line.Absent)
            {
                shiftCodeId = line?.ShiftCodeId ?? 0;
                var lineDay = line?.Date.ToString("dddd")?.ToUpperInvariant() ?? UnknownDayKey;
                var dateMidnight = DateTime.Parse(
                    $"{DateTime.Now.Date:MM/dd/yyyy} {MidnightTimeStr}"
                );

                var shiftDay = shiftCodeDays.FirstOrDefault(
                    x => x.ShiftCodeId == line.ShiftCodeId && x.Day.ToUpperInvariant() == lineDay
                );

                shiftNumberOfHours = shiftDay?.NumberOfHours ?? 0;
                shiftTimeIn1 = DateTime.Parse(
                    $"{line.Date:MM/dd/yyyy} {shiftDay?.TimeIn1?.ToString("HH:mm tt") ?? dateMidnight.ToString("HH:mm tt")}"
                );
                shiftTimeIn2 = DateTime.Parse(
                    $"{line.Date:MM/dd/yyyy} {shiftDay?.TimeIn2?.ToString("HH:mm tt") ?? dateMidnight.ToString("HH:mm tt")}"
                );
                shiftTimeOut2 = DateTime.Parse(
                    $"{line.Date:MM/dd/yyyy} {shiftDay?.TimeOut2?.ToString("HH:mm tt") ?? dateMidnight.ToString("HH:mm tt")}"
                );

                if (isFlexBreak)
                {
                    if (HasNoTimeSwipes(line))
                        return Math.Round(numberOfHours, 5);

                    var timeIn1 = shiftTimeIn1;
                    var timeOut2 = line?.TimeOut2 ?? DefaultDate;
                    var timeOut1 = line?.TimeOut1 ?? DefaultDate;
                    var timeIn2 = line?.TimeIn2 ?? DefaultDate;

                    if (timeOut1 == DefaultDate && timeIn2 != DefaultDate)
                        timeOut1 = timeIn2.AddHours(-FlexBreakGapEstimateHours);
                    if (timeIn2 == DefaultDate && timeOut1 != DefaultDate)
                        timeIn2 = timeOut1.AddHours(FlexBreakGapEstimateHours);
                    if (timeOut2 > shiftTimeOut2)
                        timeOut2 = shiftTimeOut2;

                    actualHours =
                        (decimal)(timeOut1 - timeIn1).TotalHours
                        + (decimal)(timeOut2 - timeIn2).TotalHours;

                    var shiftCodeSetup = context.MstShiftCodes.FirstOrDefault(
                        x => x.Id == shiftCodeId
                    );
                    var isStraight =
                        shiftCodeSetup?.Remarks?.ToUpperInvariant()?.Contains(StraightShiftRemark)
                        ?? false;

                    if (timeOut1 == DefaultDate && timeIn2 == DefaultDate && isStraight)
                        actualHours = (decimal)(timeOut2 - timeIn1).TotalHours;

                    if (timeOut1 == DefaultDate && timeIn2 == DefaultDate && !isStraight)
                    {
                        var half = shiftNumberOfHours / 2;
                        var halfActual = (decimal)(shiftTimeOut2 - timeIn1).TotalHours;
                        actualHours = half < halfActual ? half : halfActual;
                    }

                    numberOfHours = shiftNumberOfHours - actualHours;
                    if (numberOfHours < 0)
                        numberOfHours = 0;
                }
                else
                {
                    if (line?.TimeIn2 is not null)
                    {
                        var tIn1 = TimeOnly.FromDateTime(
                            DateTime.Parse($"{line.Date:MM/dd/yyyy} {line.TimeIn1:hh:mm tt}")
                        );
                        var tIn2 = TimeOnly.FromDateTime(
                            DateTime.Parse($"{line.Date:MM/dd/yyyy} {line.TimeIn2:hh:mm tt}")
                        );
                        if (tIn1 > tIn2)
                            shiftTimeIn2 = shiftTimeIn2.AddDays(1);
                    }

                    var lineDayStr =
                        line?.Date.ToString("dddd")?.ToUpperInvariant() ?? UnknownDayKey;
                    var lateFlexibility =
                        shiftCodeDays.FirstOrDefault(
                            x =>
                                x.ShiftCodeId == shiftCodeId
                                && x.Day.ToUpperInvariant() == lineDayStr
                        )?.LateFlexibility ?? 0;

                    var empId = line?.EmployeeId ?? 0;
                    var payrollTypeId =
                        context.MstEmployees.FirstOrDefault(x => x.Id == empId)?.PayrollTypeId ?? 0;

                    if (
                        line?.TimeIn1 is not null
                        && line.TimeOut1 is not null
                        && line.TimeIn2 is not null
                        && line.TimeOut2 is not null
                    )
                    {
                        var time1 = (decimal)(
                            (line.TimeOut1 ?? DefaultDate) - shiftTimeIn1
                        ).TotalHours;
                        var time2 = (decimal)(
                            (line.TimeOut2 ?? DefaultDate) - shiftTimeIn2
                        ).TotalHours;

                        if (lateFlexibility > 0)
                        {
                            time1 = (decimal)(
                                (line.TimeOut1 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)
                            ).TotalHours;
                            time2 = (decimal)(
                                (line.TimeOut2 ?? DefaultDate) - (line.TimeIn2 ?? DefaultDate)
                            ).TotalHours;
                        }

                        actualHours = time1 + time2;
                        numberOfHours = shiftNumberOfHours - actualHours;
                    }
                    else if (line?.TimeIn1 is not null && line?.TimeOut2 is not null)
                    {
                        shiftNumberOfHours += 1;
                        actualHours = (decimal)(
                            (line.TimeOut2 ?? DefaultDate) - shiftTimeIn1
                        ).TotalHours;

                        if ((line.TimeOut2 ?? DefaultDate) > shiftTimeOut2)
                        {
                            var dtrOut2Date = line.TimeOut2?.Date ?? DefaultDate;
                            var shiftOut2Date = shiftTimeOut2.Date;
                            actualHours =
                                dtrOut2Date == shiftOut2Date
                                    ? (decimal)(
                                          (line.TimeOut2 ?? DefaultDate) - shiftTimeIn1
                                      ).TotalHours
                                    : (decimal)(shiftTimeOut2 - shiftTimeIn1).TotalHours;
                        }

                        if (actualHours < 0)
                        {
                            if (
                                shiftCodeDays.Any(x => x.ShiftCodeId == shiftCodeId && x.IsTommorow)
                            )
                            {
                                shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
                                actualHours = (decimal)(
                                    (line.TimeOut2 ?? DefaultDate) - shiftTimeIn1
                                ).TotalHours;
                            }
                        }

                        if (lateFlexibility > 0)
                            actualHours = (decimal)(
                                (line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)
                            ).TotalHours;
                    }

                    if (actualHours < shiftNumberOfHours)
                    {
                        var isFlex =
                            context.MstEmployees?.FirstOrDefault(x => x.Id == empId)?.IsFlex
                            ?? false;

                        numberOfHours = shiftNumberOfHours - actualHours;
                        if (actualHours == 0)
                            numberOfHours = 0;

                        if (line?.TimeOut1 is null && line?.TimeIn2 is null)
                        {
                            if (
                                line is not null
                                && line?.TimeIn1 is not null
                                && line?.TimeOut2 is not null
                            )
                            {
                                var totalWorked = (decimal)(
                                    (line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)
                                ).TotalHours;
                                if (totalWorked <= ((shiftNumberOfHours - 1) / 2))
                                    numberOfHours = (shiftNumberOfHours - 1) - actualHours;
                            }
                        }
                    }

                    if (numberOfHours < 0)
                        numberOfHours = 0;
                    if (payrollTypeId == PayrollTypeFixed)
                        numberOfHours = 0;
                }
            }

            if (line?.OnLeave ?? false)
            {
                var (leaveWithPay, leaveWithHours) = GetLeaveDetails(line, context);
                if (leaveWithPay && !line.Absent)
                    numberOfHours =
                        numberOfHours > leaveWithHours ? numberOfHours - leaveWithHours : 0;
                else
                    numberOfHours =
                        (line?.RegularHours is null || line?.RegularHours == 0)
                            ? Math.Round(numberOfHours, 0)
                            : Math.Abs(Math.Round(numberOfHours, 5));
            }

            if (numberOfHours > 0 && isFlexBreak)
                return 0;
            return Math.Round(numberOfHours, 5);
        }

        /// <summary>
        /// Computes undertime using pre-resolved ShiftDates stored on the line.
        /// Used when <see cref="TrnDtrline.ShiftDates"/> is populated (change-shift scenario).
        /// </summary>
        public static decimal ComputeTardyUndertimeHoursv2(
            TrnDtrline line,
            IEnumerable<MstShiftCodeDay> shiftCodeDays,
            HRISContext context
        )
        {
            var result = 0m;
            var defaultTime = DateTime.Parse($"{DateTime.Now.Date:MM/dd/yyyy} {MidnightTimeStr}");
            var hoursRendered = 0m;
            var shiftCodeId = line?.ShiftCodeId ?? 0;
            var lineDayStr = line?.Date.ToString("dddd")?.ToUpperInvariant() ?? UnknownDayKey;
            var empId = line?.EmployeeId ?? 0;

            var lateFlexibility =
                shiftCodeDays.FirstOrDefault(
                    x => x.ShiftCodeId == shiftCodeId && x.Day.ToUpperInvariant() == lineDayStr
                )?.LateFlexibility ?? 0;
            var payrollTypeId =
                context.MstEmployees.FirstOrDefault(x => x.Id == empId)?.PayrollTypeId ?? 0;
            var shiftNumberOfHours =
                shiftCodeDays.FirstOrDefault(
                    x => x.ShiftCodeId == shiftCodeId && x.Day.ToUpperInvariant() == lineDayStr
                )?.NumberOfHours ?? 0;

            if (line is null || line.Absent)
                return 0m;

            var shiftDates = line.ShiftDates?.Split(",");
            var shiftTimeIn1 = defaultTime;
            var shiftTimeOut1 = defaultTime;
            var shiftTimeIn2 = defaultTime;
            var shiftTimeOut2 = defaultTime;

            if (shiftDates is not null && shiftDates.Length > 0)
            {
                shiftTimeIn1 = DateTime.Parse(shiftDates[0]);
                if (!string.IsNullOrEmpty(shiftDates[1]))
                    shiftTimeOut1 = DateTime.Parse(shiftDates[1]);
                if (!string.IsNullOrEmpty(shiftDates[2]))
                    shiftTimeIn2 = DateTime.Parse(shiftDates[2]);
                shiftTimeOut2 = DateTime.Parse(shiftDates[3]);

                // Apply change-shift time override when a specific shift was assigned
                var shiftCodeId2 = context.TrnChangeShiftLines
                    .Where(
                        x =>
                            x.ChangeShiftId == line.Dtr.ChangeShiftId
                            && x.EmployeeId == line.EmployeeId
                            && x.Date.Date == line.Date.Date
                    )
                    .FirstOrDefault()?.ShiftCodeId;

                if (shiftCodeId2 is not null)
                {
                    var overrideDay = context.MstShiftCodeDays.FirstOrDefault(
                        x =>
                            x.ShiftCodeId == shiftCodeId2 && x.Day == line.Date.DayOfWeek.ToString()
                    );

                    if (overrideDay is not null)
                    {
                        var origIn1 = shiftTimeIn1;
                        var origOut1 = shiftTimeOut1;
                        var origIn2 = shiftTimeIn2;
                        var origOut2 = shiftTimeOut2;

                        shiftTimeIn1 = DateTime.Parse(
                            $"{shiftTimeIn1.Date:MM/dd/yyyy} {overrideDay.TimeIn1:hh:mm tt}"
                        );
                        var disc = (shiftTimeIn1 - origIn1).TotalHours;
                        if (disc > ShiftOverrideDiscThreshold)
                            shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
                        if (disc < -ShiftOverrideDiscThreshold)
                            shiftTimeIn1 = shiftTimeIn1.AddDays(1);

                        if (!string.IsNullOrEmpty(shiftDates[1]))
                        {
                            shiftTimeOut1 = DateTime.Parse(
                                $"{shiftTimeOut1.Date:MM/dd/yyyy} {overrideDay.TimeOut1:hh:mm tt}"
                            );
                            disc = (shiftTimeOut1 - origOut1).TotalHours;
                            if (disc > ShiftOverrideDiscThreshold)
                                shiftTimeOut1 = shiftTimeOut1.AddDays(-1);
                            if (disc < -ShiftOverrideDiscThreshold)
                                shiftTimeOut1 = shiftTimeOut1.AddDays(1);
                        }

                        if (!string.IsNullOrEmpty(shiftDates[2]))
                            shiftTimeIn2 = DateTime.Parse(
                                $"{shiftTimeIn2.Date:MM/dd/yyyy} {overrideDay.TimeIn2:hh:mm tt}"
                            );

                        shiftTimeOut2 = DateTime.Parse(
                            $"{shiftTimeOut2.Date:MM/dd/yyyy} {overrideDay.TimeOut2:hh:mm tt}"
                        );
                    }
                }

                // Tomorrow-shift day adjustment
                var isTomorrowShift = shiftCodeDays.Any(
                    x => x.ShiftCodeId == shiftCodeId && x.IsTommorow
                );
                var gap = (decimal)((line.TimeIn1 ?? DefaultDate) - shiftTimeIn1).TotalHours;

                if (isTomorrowShift && (line.Date == shiftTimeIn1.Date || gap <= 20))
                {
                    if (gap >= TomorrowShiftGapMin && gap <= TomorrowShiftGapMax)
                    { /* gap within tolerance, no adjustment */
                    }
                    else
                    {
                        shiftTimeIn1 = shiftTimeIn1.AddDays(-1);
                        if (!string.IsNullOrEmpty(shiftDates[1]))
                            shiftTimeOut1 = shiftTimeOut1.AddDays(-1);
                        if (!string.IsNullOrEmpty(shiftDates[2]))
                            shiftTimeIn2 = shiftTimeIn2.AddDays(-1);
                        shiftTimeOut2 = shiftTimeOut2.AddDays(-1);
                    }
                }
                else if (isTomorrowShift)
                {
                    if (gap >= TomorrowShiftGapMin && gap <= TomorrowShiftGapMax)
                    { /* gap within tolerance, no adjustment */
                    }
                    else
                    {
                        shiftTimeIn1 = shiftTimeIn1.AddDays(1);
                        if (!string.IsNullOrEmpty(shiftDates[1]))
                            shiftTimeOut1 = shiftTimeOut1.AddDays(1);
                        if (!string.IsNullOrEmpty(shiftDates[2]))
                            shiftTimeIn2 = shiftTimeIn2.AddDays(1);
                        shiftTimeOut2 = shiftTimeOut2.AddDays(1);
                    }
                }
            }

            var isFlexBreak =
                context.MstEmployees?.FirstOrDefault(x => x.Id == line.EmployeeId)?.IsFlexBreak
                ?? false;

            if (isFlexBreak)
            {
                if (HasNoTimeSwipes(line))
                    return 0m;

                var timeIn1 = line?.TimeIn1 ?? DefaultDate;
                var timeOut2 = line?.TimeOut2 ?? DefaultDate;
                var timeOut1 = line?.TimeOut1 ?? DefaultDate;
                var timeIn2 = line?.TimeIn2 ?? DefaultDate;

                if (timeOut2 > shiftTimeOut2)
                    timeOut2 = shiftTimeOut2;

                var firstHalf = (decimal)(timeOut1 - timeIn1).TotalHours;
                var secondHalf = (decimal)(timeOut2 - timeIn2).TotalHours;
                var actualHours = firstHalf + secondHalf;

                var shiftCodeSetup = context.MstShiftCodes.FirstOrDefault(x => x.Id == shiftCodeId);
                var isStraight =
                    shiftCodeSetup?.Remarks?.ToUpperInvariant()?.Contains(StraightShiftRemark)
                    ?? false;

                if (timeOut1 == DefaultDate && timeIn2 == DefaultDate && isStraight)
                {
                    actualHours = (decimal)(timeOut2 - timeIn1).TotalHours;
                    result = shiftNumberOfHours - actualHours;
                }

                // Handle missing swipe combinations for non-straight shifts
                decimal CalcHalfDay()
                {
                    var h = shiftNumberOfHours / 2;
                    var a = (decimal)(shiftTimeOut2 - timeIn1).TotalHours;
                    return h < a ? h : a;
                }

                if (timeOut1 == DefaultDate && timeIn2 == DefaultDate && !isStraight)
                    actualHours = CalcHalfDay();
                if (timeIn1 == DefaultDate && !isStraight)
                    actualHours = CalcHalfDay();
                if (timeOut1 == DefaultDate && !isStraight)
                    actualHours = CalcHalfDay();
                if (timeIn2 == DefaultDate && !isStraight)
                    actualHours = CalcHalfDay();
                if (timeOut2 == DefaultDate && !isStraight)
                    actualHours = CalcHalfDay();

                result = shiftNumberOfHours - actualHours;

                if (line is not null && line.Absent)
                    return 0m;
            }
            else
            {
                // 4-swipe path
                if (
                    line.TimeIn1 != null
                    && line.TimeOut1 != null
                    && line.TimeIn2 != null
                    && line.TimeOut2 != null
                )
                {
                    var t1 = (decimal)((line.TimeOut1 ?? DefaultDate) - shiftTimeIn1).TotalHours;
                    var t2 = (decimal)((line.TimeOut2 ?? DefaultDate) - shiftTimeIn2).TotalHours;
                    if (t1 < 0)
                        t1 = 0;
                    if (t2 < 0)
                        t2 = 0;
                    hoursRendered = t1 + t2;
                }

                // 2-swipe path
                if (line.TimeIn1 != null && line.TimeOut2 != null)
                {
                    shiftNumberOfHours += 1;

                    if (
                        TimeOnly.FromDateTime(shiftTimeIn1) == TimeOnly.Parse(MidnightPlusOneAmStr)
                        || TimeOnly.FromDateTime(shiftTimeIn1)
                            == TimeOnly.Parse(MidnightPlusOnePmStr)
                    )
                    {
                        var addSeconds =
                            (
                                shiftTimeIn1
                                - DateTime.Parse($"{shiftTimeIn1:MM/dd/yyyy} {MidnightTimeStr}")
                            ).TotalSeconds;
                        if (shiftTimeIn1.ToString("tt") == "PM")
                            addSeconds =
                                (
                                    TimeOnly.FromDateTime(shiftTimeIn1)
                                    - TimeOnly.Parse(NoonTimeStr)
                                ).TotalSeconds;
                        shiftTimeIn1 = shiftTimeIn1.AddSeconds(-addSeconds);
                    }

                    hoursRendered = (decimal)(
                        (line.TimeOut2 ?? DefaultDate) - shiftTimeIn1
                    ).TotalHours;
                    if (lateFlexibility > 0)
                        hoursRendered = (decimal)(
                            (line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)
                        ).TotalHours;
                }

                if (lateFlexibility > 0)
                    hoursRendered = (decimal)(
                        (line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)
                    ).TotalHours;

                if (hoursRendered < shiftNumberOfHours)
                {
                    var isFlex =
                        context.MstEmployees?.FirstOrDefault(x => x.Id == empId)?.IsFlex ?? false;

                    result = shiftNumberOfHours - hoursRendered;
                    if (hoursRendered == 0)
                        result = 0;

                    if (
                        line?.TimeOut1 is null
                        && line?.TimeIn2 is null
                        && line?.TimeIn1 is not null
                        && line?.TimeOut2 is not null
                    )
                    {
                        var totalWorked = (decimal)(
                            (line.TimeOut2 ?? DefaultDate) - (line.TimeIn1 ?? DefaultDate)
                        ).TotalHours;
                        if (totalWorked <= ((shiftNumberOfHours - 1) / 2))
                            result = (shiftNumberOfHours - 1) - hoursRendered;
                    }
                }

                if (result < 0)
                    result = 0;
                if (payrollTypeId == PayrollTypeFixed)
                    result = 0;
            }

            if (line?.OnLeave ?? false)
            {
                var (leaveWithPay, leaveWithHours) = GetLeaveDetails(line, context);
                if (leaveWithPay && !line.Absent)
                    result = result > leaveWithHours ? result - leaveWithHours : 0;
                else
                    result =
                        (line?.RegularHours is null || line?.RegularHours == 0)
                            ? Math.Round(result, 0)
                            : Math.Abs(Math.Round(result, 5));
            }

            if (result > 0)
                return Math.Round(result, 5);
            if (result < 0)
                return 0;
            return Math.Round(result, 5);
        }

        #endregion

        #region Rates

        public static decimal ComputeRatePerHour(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees
        ) => employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.HourlyRate ?? 0;

        public static decimal ComputeRatePerNightHour(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees
        ) => employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.NightHourlyRate ?? 0;

        public static decimal ComputeRatePerOvertimeHour(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees
        ) => employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.OvertimeHourlyRate ?? 0;

        public static decimal ComputeRatePerOvertimeNightHour(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees
        ) => employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.OvertimeNightHourlyRate ?? 0;

        public static decimal ComputeRatePerHourTardy(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees
        ) =>
            Math.Round(
                employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.TardyHourlyRate ?? 0,
                5
            );

        public static decimal ComputeRatePerAbsentDay(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees
        ) =>
            Math.Round(
                employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.AbsentDailyRate ?? 0,
                5
            );

        #endregion

        #region Amount Computation

        public static decimal ComputeRegularAmount(TrnDtrline line) =>
            Math.Round(line.RegularHours * line.RatePerHour, 5);

        /// <summary>Night differential pay scaled by holiday status, rest day, and eligibility.</summary>
        public static decimal ComputeNightAmount(
            TrnDtrline line,
            bool isEligibleForHolidayPay = false
        )
        {
            decimal multiplier;

            if (line.RestDay)
            {
                multiplier = line.DayTypeId switch
                {
                    DayTypeRegularHoliday
                      => line.RatePerNightHour
                          * (isEligibleForHolidayPay ? NightMultRestRegHolElig : 1m),
                    DayTypeSpecialHoliday
                      => line.RatePerNightHour
                          * (isEligibleForHolidayPay ? NightMultRestSpcHolElig : 1m),
                    _
                      => line.RatePerNightHour
                          * (isEligibleForHolidayPay ? NightMultRestWorkElig : 1m),
                };
            }
            else
            {
                multiplier = line.DayTypeId switch
                {
                    DayTypeRegularHoliday
                      => line.RatePerNightHour
                          * (isEligibleForHolidayPay ? NightMultRegHolElig : 1m),
                    DayTypeSpecialHoliday
                      => line.RatePerNightHour
                          * (isEligibleForHolidayPay ? NightMultSpcHolElig : 1m),
                    _ => line.RatePerNightHour, // factor is 1 regardless of eligibility
                };
            }

            return Math.Round(line.NightHours * multiplier, 5);
        }

        /// <summary>
        /// Overtime pay scaled by payroll type, day type, rest day, and holiday eligibility.
        /// Multipliers follow Philippine Labor Code rates.
        /// </summary>
        public static decimal ComputeOverTimeAmount(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees,
            IEnumerable<MstDayTypeDay> dayTypeDays,
            bool isEligibleForHolidayPay = false
        )
        {
            var amount = 0m;
            var payrollTypeId =
                employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

            if (payrollTypeId == PayrollTypeFixed)
            {
                // For fixed-rate employees all DayType switch branches yield the same raw formula
                amount = line.OvertimeHours * line.RatePerOvertimeHour;
            }
            else
            {
                amount = line.DayTypeId switch
                {
                    DayTypeRegularHoliday
                      => line.OvertimeHours
                          * line.RatePerOvertimeHour
                          * (isEligibleForHolidayPay ? OtMultRegHolElig : OtMultWorking),
                    DayTypeSpecialHoliday
                      => line.OvertimeHours
                          * line.RatePerOvertimeHour
                          * (isEligibleForHolidayPay ? OtMultSpcHolElig : OtMultWorking),
                    _ => line.OvertimeHours * line.RatePerOvertimeHour * OtMultWorking,
                };
            }

            // Rest-day overrides apply to all payroll types
            if (line.RestDay)
            {
                amount = line.DayTypeId switch
                {
                    DayTypeRegularHoliday
                      => line.OvertimeHours
                          * line.RatePerOvertimeHour
                          * (
                              isEligibleForHolidayPay
                                  ? OtMultRestRegHolElig
                                  : OtMultRestRegHolNotElig
                          ),
                    DayTypeSpecialHoliday
                      => line.OvertimeHours
                          * line.RatePerOvertimeHour
                          * (
                              isEligibleForHolidayPay
                                  ? OtMultRestSpcHolElig
                                  : OtMultRestSpcHolNotElig
                          ),
                    _ => line.OvertimeHours * line.RatePerOvertimeHour * OtMultRestWorking,
                };
            }

            return Math.Round(amount, 5);
        }

        public static decimal ComputeOvertimeNightAmount(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees,
            IEnumerable<MstDayTypeDay> dayTypeDays
        )
        {
            var amount = 0m;
            var payrollTypeId =
                employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

            if (payrollTypeId == PayrollTypeFixed)
            {
                var branchId =
                    employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.BranchId ?? 0;
                var dayTypeDay = dayTypeDays.FirstOrDefault(
                    x => x.Date.Date == line.Date.Date && x.BranchId == branchId
                );
                var excludedInFixed = dayTypeDay?.ExcludedInFixed ?? true;

                if (!excludedInFixed)
                    amount =
                        line.OvertimeNightHours
                        * line.RatePerHour
                        * (line.DayMultiplier + OtNightFixedAddendum);
                else
                {
                    amount = line.DayTypeId switch
                    {
                        DayTypeRegularHoliday
                          => line.OvertimeNightHours
                              * line.RatePerHour
                              * (line.DayMultiplier + OtNightRegHolAddendum),
                        DayTypeSpecialHoliday
                          => line.OvertimeNightHours
                              * line.RatePerHour
                              * (line.DayMultiplier * OtNightSpcHolFactorFixed),
                        _
                          => line.OvertimeNightHours
                              * line.RatePerOvertimeNightHour
                              * line.DayMultiplier,
                    };
                }
            }
            else
            {
                amount = line.DayTypeId switch
                {
                    DayTypeRegularHoliday
                      => line.OvertimeNightHours
                          * line.RatePerHour
                          * (line.DayMultiplier + OtNightRegHolAddendum),
                    DayTypeSpecialHoliday
                      => line.OvertimeNightHours
                          * line.RatePerHour
                          * (line.DayMultiplier * OtNightSpcHolFactorVar),
                    _ => line.OvertimeNightHours * line.RatePerOvertimeHour * line.DayMultiplier,
                };
            }

            return Math.Round(amount, 2);
        }

        /// <summary>
        /// Computes the day multiplier based on day type, rest day, holiday eligibility, and payroll type.
        /// </summary>
        public static decimal ComputeDayMultiplier(
            TrnDtrline line,
            IEnumerable<MstEmployee> employees,
            IEnumerable<MstDayTypeDay> dayTypeDays,
            bool isEligibleForHolidayPay,
            HRISContext context
        )
        {
            var multiplier = 1m;
            var excludedInFixed = false;
            var branchId = employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.BranchId ?? 0;

            var dayTypeDay = dayTypeDays.FirstOrDefault(
                x => x.Date.Date == line.Date.Date && x.BranchId == branchId
            );

            if (dayTypeDay is not null)
            {
                multiplier = line.RestDay
                    ? context.MstDayTypes.FirstOrDefault(
                          x => x.Id == dayTypeDay.DayTypeId
                      )?.RestdayDays ?? 0
                    : context.MstDayTypes.FirstOrDefault(
                          x => x.Id == dayTypeDay.DayTypeId
                      )?.WorkingDays ?? 0;

                if (!isEligibleForHolidayPay && line.DayTypeId > DayTypeWorking)
                {
                    multiplier = line.RestDay
                        ? context.MstDayTypes.FirstOrDefault(
                              x => x.Id == DayTypeWorking
                          )?.RestdayDays ?? DayMultRestDayFallback
                        : 1.0m;
                }

                excludedInFixed = dayTypeDay.ExcludedInFixed;
            }
            else
            {
                if (line.RestDay)
                    multiplier =
                        context.MstDayTypes.FirstOrDefault(x => x.Id == line.DayTypeId)?.RestdayDays
                        ?? 0;
                excludedInFixed = false;
            }

            var payrollTypeId =
                employees.FirstOrDefault(x => x.Id == line.EmployeeId)?.PayrollTypeId ?? 0;

            if (!excludedInFixed && payrollTypeId == PayrollTypeFixed && multiplier > 1)
                multiplier = multiplier - 1;

            // payrollTypeId == 1 block — preserved as-is (see original DTR.cs for intent)
            if (payrollTypeId == PayrollTypeVariable)
            {
                var dayTypeId = 1;
                var dateAfterHoliday = DefaultDate;
                var dateBeforeHoliday = DefaultDate;

                // NOTE: condition reads `dayTypeDay is null` (original behavior preserved).
                // When dayTypeDay IS null, the interior assignments are all no-ops due to ?. fallback.
                if (dayTypeDay is null)
                {
                    dayTypeId = dayTypeDay?.DayTypeId ?? 1;
                    dateAfterHoliday = dayTypeDay?.DateAfter ?? DefaultDate;
                    dateBeforeHoliday = dayTypeDay?.DateBefore ?? DefaultDate;
                }

                if (
                    line.TimeIn1 is null
                    && line.TimeOut1 is null
                    && line.TimeIn2 is null
                    && line.TimeOut2 is null
                )
                {
                    if (dayTypeId == DayTypeRegularHoliday)
                    {
                        var workedBefore = context.TrnDtrlines.Any(
                            x =>
                                x.EmployeeId == line.EmployeeId
                                && x.Date.Date == dateBeforeHoliday.Date
                                && !x.Absent
                        );

                        if (workedBefore)
                            multiplier =
                                (line?.RestDay ?? false)
                                    ? multiplier * DayMultHolidayWorkFactor
                                    : multiplier - 1;
                    }
                }
            }

            if (payrollTypeId == PayrollTypeProjectBased)
            {
                var dayTypeId = dayTypeDay?.DayTypeId ?? DayTypeWorking;
                if (dayTypeId > DayTypeWorking)
                {
                    if (dayTypeId == DayTypeSpecialHoliday && (line?.RestDay ?? false))
                    {
                        // Intentional no-op: multiplier unchanged for Special Holiday rest day (project-based)
                    }
                }
            }

            return multiplier;
        }

        /// <summary>
        /// Computes total daily pay. Applies day multiplier to regular amount and sums all pay components.
        /// </summary>
        public static decimal ComputeTotalAmount(
            TrnDtrline line,
            IEnumerable<MstShiftCodeDay> shiftCodeDays,
            bool isEligibleForHolidayPay,
            HRISContext context
        )
        {
            var amount = 0m;
            var dayTypeId = line.DayTypeId;
            var restDay = line.RestDay;
            var deduction = 0m; // placeholder for future deduction logic

            var isTomorrowShift =
                shiftCodeDays.FirstOrDefault(
                    x => x.ShiftCodeId == line.ShiftCodeId && x.Day == line.Date.ToString("dddd")
                )?.IsTommorow ?? false;

            if (isTomorrowShift && dayTypeId == DayTypeWorking && HasNoTimeSwipes(line))
                return Math.Round(amount, 5);

            var dmRest =
                context.MstDayTypes?.FirstOrDefault(x => x.Id == dayTypeId)?.RestdayDays ?? 0;
            var dmNormal =
                context.MstDayTypes?.FirstOrDefault(x => x.Id == dayTypeId)?.WorkingDays ?? 0;

            if (!isEligibleForHolidayPay)
            {
                dayTypeId = DayTypeWorking;
                dmRest = 1;
                dmNormal = 1;
            }

            if (line is not null)
            {
                if (dayTypeId == DayTypeWorking)
                {
                    amount = restDay
                        ? (line.RegularAmount * dmRest)
                          + line.NightAmount
                          + line.OvertimeNightAmount
                          + line.OvertimeAmount
                        : (line.RegularAmount * dmNormal)
                          + line.NightAmount
                          + line.OvertimeNightAmount
                          + line.OvertimeAmount;
                }
                else if (dayTypeId == DayTypeRegularHoliday)
                {
                    if (restDay)
                    {
                        amount = HasNoTimeSwipes(line)
                            ? (line.RegularAmount * 1)
                              + line.NightAmount
                              + line.OvertimeNightAmount
                              + line.OvertimeAmount
                            : (line.RegularAmount * dmRest)
                              + line.NightAmount
                              + line.OvertimeNightAmount
                              + line.OvertimeAmount
                              - deduction;
                    }
                    else
                    {
                        amount = HasNoTimeSwipes(line)
                            ? (line.RegularAmount * (dmNormal - 1))
                              + line.NightAmount
                              + line.OvertimeNightAmount
                              + line.OvertimeAmount
                            : (line.RegularAmount * dmNormal)
                              + line.NightAmount
                              + line.OvertimeNightAmount
                              + line.OvertimeAmount
                              - deduction;
                    }
                }
                else if (dayTypeId == DayTypeSpecialHoliday)
                {
                    if (restDay)
                    {
                        amount = HasNoTimeSwipes(line)
                            ? (line.RegularAmount * (dmRest - 1))
                              + line.NightAmount
                              + line.OvertimeNightAmount
                              + line.OvertimeAmount
                              - deduction
                            : (line.RegularAmount * dmRest)
                              + line.NightAmount
                              + line.OvertimeNightAmount
                              + line.OvertimeAmount
                              - deduction;
                    }
                    else
                    {
                        if (HasNoTimeSwipes(line))
                        {
                            amount =
                                line.NightAmount
                                + line.OvertimeNightAmount
                                + line.OvertimeAmount
                                - deduction;
                            if (amount < 0)
                                amount = 0;

                            if (line!.OnLeave)
                            {
                                var trnDtr = context.TrnDtrs.FirstOrDefault(
                                    x => x.Id == line.Dtrid
                                );
                                var withPay = context.TrnLeaveApplicationLines.FirstOrDefault(
                                    x =>
                                        x.EmployeeId == line.EmployeeId
                                        && x.LeaveApplicationId == trnDtr!.LeaveApplicationId
                                )?.WithPay;

                                if (withPay ?? false)
                                {
                                    amount = line.RegularAmount;
                                    line.RegularHours = 0;
                                }
                            }
                        }
                        else
                        {
                            amount =
                                (line.RegularAmount * dmNormal)
                                + line.NightAmount
                                + line.OvertimeNightAmount
                                + line.OvertimeAmount
                                - deduction;
                        }
                    }
                }
            }

            return Math.Round(amount, 5);
        }

        public static decimal ComputeTardyAmount(TrnDtrline line, HRISContext context)
        {
            var amount = line.RestDay
                ? (line.TardyLateHours + line.TardyUndertimeHours) * line.RatePerHourTardy
                : (line.TardyLateHours + line.TardyUndertimeHours) * line.RatePerHourTardy;

            return Math.Round(amount, 5);
        }

        public static decimal ComputeAbsentAmount(TrnDtrline line)
        {
            var rate = line.Absent
                ? line.RatePerAbsentDay
                : line.HalfdayAbsent ? line.RatePerAbsentDay / 2 : 0;

            return Math.Round(rate, 5);
        }

        public static decimal ComputeNetAmount(TrnDtrline line)
        {
            var rate = line.TotalAmount - line.TardyAmount;

            // Eggs Changes 04/13/2026
            if (line.OnLeave)
                rate = rate * line.DayMultiplier;

            return Math.Round(rate, 5);
        }

        #endregion

        #region Shift Resolution

        public static int QuickChangeShift(
            HRISContext context,
            DateTime timeIn1,
            int employeeId,
            DateTime dtrDate,
            int changeShiftId,
            int? origShiftCodeId = null
        )
        {
            var changeShiftCodeId =
                context.TrnChangeShiftLines?.FirstOrDefault(
                    x =>
                        x.ChangeShiftId == changeShiftId
                        && x.EmployeeId == employeeId
                        && x.Date.Date == dtrDate.Date
                )?.ShiftCodeId ?? 0;

            return changeShiftCodeId > 0 ? changeShiftCodeId : origShiftCodeId ?? 0;
        }

        public static int QuickChangeShiftv2(
            HRISContext context,
            IEnumerable<EmployeeShiftCodeDay.Record> employeeShiftCodeDays,
            IEnumerable<MstEmployeeShiftCode> employeeShiftCodes,
            int employeeId,
            DateTime dtrDate,
            int changeShiftId,
            int? origShiftCodeId = null
        )
        {
            var changeShiftCodeId =
                context.TrnChangeShiftLines?.FirstOrDefault(
                    x =>
                        x.ChangeShiftId == changeShiftId
                        && x.EmployeeId == employeeId
                        && x.Date.Date == dtrDate.Date
                )?.ShiftCodeId ?? 0;

            if (changeShiftCodeId > 0)
                return changeShiftCodeId;

            var empShiftCodes = employeeShiftCodes.Where(x => x.EmployeeId == employeeId);
            var shiftCodeId =
                origShiftCodeId
                ?? empShiftCodes.FirstOrDefault(x => x.EmployeeId == employeeId)?.ShiftCodeId
                ?? 0;
            var result =
                (empShiftCodes?.Any(x => x.ShiftCodeId == shiftCodeId) ?? false)
                    ? shiftCodeId
                    : (empShiftCodes?.FirstOrDefault()?.ShiftCodeId ?? 0);

            if (employeeShiftCodeDays is not null && employeeShiftCodeDays.Any())
                result =
                    employeeShiftCodeDays.OrderBy(x => x.Interval).FirstOrDefault()?.ShiftCodeId
                    ?? result;

            return result;
        }

        public static int QuickChangeShiftv3(
            HRISContext context,
            IEnumerable<EmployeeShiftCodeDay.Record> employeeShiftCodeDays,
            IEnumerable<MstEmployeeShiftCode> employeeShiftCodes,
            int employeeId,
            DateTime dtrDate,
            int changeShiftId,
            int? origShiftCodeId = null
        )
        {
            var changeShiftCodeId =
                context.TrnChangeShiftLines?.FirstOrDefault(
                    x =>
                        x.ChangeShiftId == changeShiftId
                        && x.EmployeeId == employeeId
                        && x.Date.Date == dtrDate.Date
                )?.ShiftCodeId ?? 0;

            var isFlexBreak =
                context.MstEmployees?.FirstOrDefault(x => x.Id == employeeId)?.IsFlexBreak ?? false;

            if (changeShiftCodeId > 0)
                return changeShiftCodeId;

            var empShiftCodes = employeeShiftCodes.Where(x => x.EmployeeId == employeeId);
            var shiftCodeId =
                origShiftCodeId
                ?? empShiftCodes.FirstOrDefault(x => x.EmployeeId == employeeId)?.ShiftCodeId
                ?? 0;
            var result =
                (empShiftCodes?.Any(x => x.ShiftCodeId == shiftCodeId) ?? false)
                    ? shiftCodeId
                    : (empShiftCodes?.FirstOrDefault()?.ShiftCodeId ?? 0);

            if (employeeShiftCodeDays is not null && employeeShiftCodeDays.Any())
            {
                result = isFlexBreak
                    ? employeeShiftCodeDays.OrderBy(x => x.Interval).FirstOrDefault()?.ShiftCodeId
                      ?? result
                    : employeeShiftCodeDays
                          .OrderByDescending(x => x.Interval)
                          .FirstOrDefault()?.ShiftCodeId ?? result;
            }

            return result;
        }

        #endregion

        #region Data Access

        public static IEnumerable<MstEmployee> GetEmployees(HRISContext context) =>
            context.MstEmployees.ToArray();

        public static IEnumerable<MstShiftCodeDay> GetShiftCodeDays(HRISContext context) =>
            context.MstShiftCodeDays.ToArray();

        public static List<int> GetEmployeeIds(int? departmentId, HRISContext context)
        {
            if (departmentId is not null)
                return context.MstEmployees
                        .Where(x => x.DepartmentId == departmentId)
                        .Select(x => x.Id)
                        .ToList() ?? new List<int>();

            return context.MstEmployees.Select(x => x.Id).ToList() ?? new List<int>();
        }

        #endregion

        #region DTR Processing

        internal static void ProcessDtrLog(
            AddDtrLinesByProcessDtr command,
            List<TrnDtrLineDto> dtrLines,
            HRISContext context
        )
        {
            var getEmployees = new GetEmployees()
            {
                PayrollGroupId = command.PayrollGroupId,
                DepartmentId = command.DepartmentId,
                EmployeeId = command.EmployeeId,
            };

            var employeeList = getEmployees.Result();
            var employeeIds = employeeList.Select(x => x.Id).ToList();

            if (command.EmployeeId != null)
            {
                context.TrnDtrlines
                    .Where(
                        l =>
                            l.Dtrid == command.DTRId
                            && l.EmployeeId == command.EmployeeId
                            && l.Date >= command.DateStart
                            && l.Date <= command.DateEnd
                    )
                    .ExecuteDelete();
            }
            else
            {
                context.TrnDtrlines
                    .Where(
                        l =>
                            l.Dtrid == command.DTRId
                            && employeeIds.Contains(l.EmployeeId)
                            && l.Date >= command.DateStart
                            && l.Date <= command.DateEnd
                    )
                    .ExecuteDelete();
            }

            var batchProcessor = new DtrBatchProcessor(
                employeeList,
                command.DateStart,
                command.DateEnd,
                command.ChangeShiftId,
                context
            );

            foreach (var employee in employeeList)
            {
                try
                {
                    var remarks = !employee.IsLocked ? "In-Active" : "";

                    for (
                        var dtrDate = command.DateStart;
                        dtrDate <= command.DateEnd;
                        dtrDate = dtrDate.AddDays(1)
                    )
                    {
                        dtrLines.Add(
                            new TrnDtrLineDto()
                            {
                                Dtrid = command.DTRId,
                                EmployeeId = employee.Id,
                                Date = dtrDate,
                                ShiftCodeId = batchProcessor.GetShiftCode(
                                    command.ChangeShiftId,
                                    employee.Id,
                                    dtrDate
                                ),
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
                            }
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void ProcessDtrLines(
            int changeShiftId,
            List<TmpDtrLogs> logs,
            List<TrnDtrLineDto> dtrLines,
            DateTime dateStart,
            DateTime dateEnd,
            HRISContext context
        )
        {
            var employeesInLog = logs.GroupBy(x => x.EmployeeId).Select(y => y.Key);
            var employees = context.MstEmployees.ToArray();
            var shiftCodeDays = context.MstShiftCodeDays.ToArray();
            var employeeShiftCodes =
                context.MstEmployeeShiftCodes?.ToArray() ?? Array.Empty<MstEmployeeShiftCode>();

            if (employeesInLog is null)
                return;

            foreach (var empId in employeesInLog)
            {
                var shiftCodeId = 0;
                var days = new List<string>
                {
                    "Sunday",
                    "Monday",
                    "Tuesday",
                    "Wednesday",
                    "Thursday",
                    "Friday",
                    "Saturday"
                };
                var fLogs = logs.Where(x => x.EmployeeId == empId)
                    .OrderBy(x => x.Date)
                    .ThenBy(x => x.Time);
                    
                var firstDateOfLogWeekly = fLogs.Any() 
                    ? fLogs.First().Date.AddDays(-(int)fLogs.First().Date.DayOfWeek) 
                    : DefaultDateOnly;
                    
                var dateOfLogWeekly = DefaultDateOnly;
                var isWorkDayCompleted = true;
                var lastTimeOutOfWorkShift = DefaultDate;
                var isFlex = false;
                var isFlexBreak = false;
                var isLongShift = false;
                var isWorkDayCompletedFlex = true;
                var shiftTimeIn1Flex = DefaultDate;
                var shiftTimeOut2Flex = DefaultDate;
                var currentProcessedEmpId = 0;
                var shiftTimeOut2OfWork = DefaultDate;
                var aWeekIsLapsed = false;
                var dlineIsJumped = false;
                var dline = new TrnDtrLineDto();
                var toBeProcessedPayGroupId =
                    employees?.FirstOrDefault(
                        x => x.Id == empId
                    )?.PayrollGroupId ?? 0;

                foreach (var log in fLogs)
                {
                    try
                    {
                        var logEmpPayGroupId =
                            employees?.FirstOrDefault(
                                x => x.Id == (log?.EmployeeId ?? 0)
                            )?.PayrollGroupId ?? 0;
                        if (logEmpPayGroupId != toBeProcessedPayGroupId)
                            continue;

                        var logDateTime = log.Date.ToDateTime(log.Time);
                        // Console.WriteLine($"Processing log for Emp {log.EmployeeId} at {logDateTime} type {log.LogType}");

                        if (dline is not null)
                        {
                            isFlex =
                                employees?.FirstOrDefault(x => x.Id == log.EmployeeId)?.IsFlex
                                ?? false;
                            isFlexBreak =
                                employees?.FirstOrDefault(x => x.Id == log.EmployeeId)?.IsFlexBreak
                                ?? false;
                            isLongShift =
                                employees?.FirstOrDefault(x => x.Id == log.EmployeeId)?.IsLongShift
                                ?? false;

                            if (isFlex)
                            {
                                if (currentProcessedEmpId != log.EmployeeId)
                                {
                                    currentProcessedEmpId = log.EmployeeId;
                                    isWorkDayCompletedFlex = true;
                                }

                                dline = dtrLines.FirstOrDefault(
                                    x =>
                                        x.EmployeeId == log.EmployeeId
                                        && DateOnly.FromDateTime(x.Date) == log.Date
                                );

                                if (dline is not null)
                                {
                                    if (dline.TimeIn1 != null && dline.TimeOut2 != null)
                                    {
                                        continue;
                                    }

                                    shiftTimeIn1Flex = dline.Date;

                                    if (isWorkDayCompletedFlex)
                                    {
                                        isWorkDayCompletedFlex = false;
                                        shiftTimeOut2Flex = DefaultDate;
                                    }
                                    else
                                    {
                                        if (shiftTimeIn1Flex.Date != dline.Date)
                                            dline =
                                                dtrLines.FirstOrDefault(
                                                    x =>
                                                        x.EmployeeId == log.EmployeeId
                                                        && DateOnly.FromDateTime(x.Date)
                                                            == log.Date.AddDays(-1)
                                                ) ?? dline;
                                    }

                                    dline.ShiftCodeId =
                                        employeeShiftCodes?.FirstOrDefault(
                                            x => x.EmployeeId == log.EmployeeId
                                        )?.ShiftCodeId ?? 0;

                                    if (dline.TimeIn1 == null && log.LogType == LogTypeIn)
                                    {
                                        var nextLog =
                                            fLogs?.Where(
                                                x =>
                                                    x.EmployeeId == log.EmployeeId
                                                    && DateTime.Parse($"{x.Date} {x.Time}")
                                                        > logDateTime
                                            )?.OrderBy(
                                                x => DateTime.Parse($"{x.Date} {x.Time}")
                                            )?.FirstOrDefault() ?? new TmpDtrLogs();
                                        var nextLogDT = DateTime.Parse(
                                            $"{nextLog.Date} {nextLog.Time}"
                                        );
                                        var intervalHours = (nextLogDT - logDateTime).TotalHours;

                                        if (intervalHours > FlexWorkWindowHours)
                                        {
                                            if (nextLogDT.Date != logDateTime.Date)
                                                dline.TimeIn1 = logDateTime;
                                            isWorkDayCompletedFlex = true;
                                            continue;
                                        }

                                        dline.TimeIn1 = logDateTime;
                                        var tempShiftOut2 = logDateTime.AddHours(
                                            FlexWorkWindowHours
                                        );
                                        var filteredLog = fLogs?
                                            .Where(
                                                x =>
                                                    x.EmployeeId == log.EmployeeId
                                                    && DateTime.Parse($"{x.Date} {x.Time}")
                                                        <= tempShiftOut2
                                            )
                                            .OrderByDescending(
                                                x => DateTime.Parse($"{x.Date} {x.Time}")
                                            )
                                            .FirstOrDefault();

                                        if (filteredLog is not null)
                                            shiftTimeOut2Flex = DateTime.Parse(
                                                $"{filteredLog.Date} {filteredLog.Time}"
                                            );

                                        if (shiftTimeIn1Flex == shiftTimeOut2Flex)
                                            isWorkDayCompletedFlex = true;
                                        continue;
                                    }

                                    if (dline.TimeIn1 != null && dline.TimeOut2 == null)
                                    {
                                        dline.TimeOut2 = logDateTime;
                                        isWorkDayCompletedFlex = true;
                                    }

                                    if (
                                        dline.TimeIn1 == null
                                        && shiftTimeOut2Flex == DefaultDate
                                        && dline.TimeOut2 == null
                                    )
                                    {
                                        dline.TimeOut2 = logDateTime;
                                        isWorkDayCompletedFlex = true;
                                    }
                                }
                                continue;
                            }
                            else
                            {
                                if (currentProcessedEmpId != log.EmployeeId)
                                {
                                    if (
                                        log.LogType == LogTypeOut
                                        || log.LogType == LogTypeBreakStart
                                        || log.LogType == LogTypeBreakEnd
                                    )
                                    {
                                        if (log.Date < DateOnly.FromDateTime(dateStart))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            if (dateStart == dateEnd)
                                            {
                                                var empDLines = dtrLines.Where(
                                                    x =>
                                                        x.EmployeeId == empId
                                                        && DateOnly.FromDateTime(x.Date) == log.Date
                                                );
                                                foreach (var empDLine in empDLines)
                                                {
                                                    var emp = employees.FirstOrDefault(
                                                        a => a.Id == empId
                                                    );
                                                    if (emp == null)
                                                        return;
                                                    var bioId = emp.BiometricIdNumber;

                                                    var startMain = dateStart.Date;
                                                    var endMain = startMain.AddDays(1).AddTicks(-1);
                                                    var startPrev = startMain.AddDays(-1);
                                                    var endPrev = startMain.AddTicks(-1);

                                                    var currentDayLogs = context.TrnLogs.Where(
                                                        d =>
                                                            d.BiometricIdNumber == bioId
                                                            && d.LogDateTime >= startMain
                                                            && d.LogDateTime <= endMain
                                                    );
                                                    var lastInYesterday = context.TrnLogs
                                                        .Where(
                                                            d =>
                                                                d.BiometricIdNumber == bioId
                                                                && d.LogType == LogTypeIn
                                                                && d.LogDateTime >= startPrev
                                                                && d.LogDateTime <= endPrev
                                                        )
                                                        .OrderByDescending(d => d.LogDateTime)
                                                        .Take(1);
                                                    var lastIn = lastInYesterday.FirstOrDefault();
                                                    var currentOut = currentDayLogs
                                                        .Where(a => a.LogType == LogTypeOut)
                                                        .FirstOrDefault();

                                                    if (lastIn?.LogDateTime is DateTime dt)
                                                        empDLine.TimeIn1 = lastIn.LogDateTime;
                                                    if (currentOut?.LogDateTime is DateTime dt1)
                                                    {
                                                        if (isFlexBreak)
                                                        {
                                                            var currentIn = currentDayLogs
                                                                .Where(
                                                                    a =>
                                                                        a.LogType == LogTypeBreakEnd
                                                                )
                                                                .FirstOrDefault();
                                                            if (
                                                                currentIn?.LogDateTime
                                                                is DateTime dt2
                                                            )
                                                                empDLine.TimeIn2 =
                                                                    currentIn.LogDateTime;
                                                        }
                                                        empDLine.TimeOut2 = currentOut.LogDateTime;
                                                    }

                                                    var escSetup = new EmployeeShiftCodeDay();
                                                    if (
                                                        employeeShiftCodes is not null
                                                        && dline is not null
                                                    )
                                                    {
                                                        escSetup.ParamEmployeeId =
                                                            empDLine.EmployeeId;
                                                        escSetup.ParamDay = empDLine.Date.ToString(
                                                            "dddd"
                                                        );
                                                        escSetup.ParamLogTimeIn1 = DateTime.Parse(
                                                            $"{log.Date:d} {log.Time:hh:mm tt}"
                                                        );
                                                        var escDays = escSetup.Result(
                                                            isLongShift ? log.LogType : null
                                                        );
                                                        empDLine.ShiftCodeId = QuickChangeShiftv3(
                                                            context,
                                                            escDays,
                                                            employeeShiftCodes,
                                                            empDLine.EmployeeId,
                                                            empDLine.Date,
                                                            0,
                                                            empDLine.ShiftCodeId
                                                        );
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (isFlexBreak)
                                                {
                                                    var empDLines = dtrLines.Where(
                                                        x =>
                                                            x.EmployeeId == empId
                                                            && DateOnly.FromDateTime(x.Date)
                                                                == log.Date
                                                    );
                                                    foreach (var empDLine in empDLines)
                                                    {
                                                        var emp = employees.FirstOrDefault(
                                                            a => a.Id == empId
                                                        );
                                                        if (emp == null)
                                                            return;
                                                        var bioId = emp.BiometricIdNumber;
                                                        var startDate = log.Date.ToDateTime(
                                                            TimeOnly.MinValue
                                                        );
                                                        var endDate = startDate.AddDays(1);
                                                        var currentDayLogs = context.TrnLogs.Where(
                                                            d =>
                                                                d.BiometricIdNumber == bioId
                                                                && d.LogDateTime >= startDate
                                                                && d.LogDateTime <= endDate
                                                        );
                                                        var currentOut = currentDayLogs
                                                            .Where(a => a.LogType == LogTypeOut)
                                                            .FirstOrDefault();

                                                        if (currentOut?.LogDateTime is DateTime dt1)
                                                        {
                                                            var currentIn = currentDayLogs
                                                                .Where(
                                                                    a =>
                                                                        a.LogType == LogTypeBreakEnd
                                                                )
                                                                .FirstOrDefault();
                                                            if (
                                                                currentIn?.LogDateTime
                                                                is DateTime dt2
                                                            )
                                                                empDLine.TimeIn2 =
                                                                    currentIn.LogDateTime;
                                                            empDLine.TimeOut2 =
                                                                currentOut.LogDateTime;
                                                        }

                                                        var escSetup = new EmployeeShiftCodeDay();
                                                        if (
                                                            employeeShiftCodes is not null
                                                            && dline is not null
                                                        )
                                                        {
                                                            escSetup.ParamEmployeeId =
                                                                empDLine.EmployeeId;
                                                            escSetup.ParamDay =
                                                                empDLine.Date.ToString("dddd");
                                                            escSetup.ParamLogTimeIn1 =
                                                                DateTime.Parse(
                                                                    $"{log.Date:d} {log.Time:hh:mm tt}"
                                                                );
                                                            var escDays = escSetup.Result(
                                                                isLongShift ? log.LogType : null
                                                            );
                                                            empDLine.ShiftCodeId =
                                                                QuickChangeShiftv3(
                                                                    context,
                                                                    escDays,
                                                                    employeeShiftCodes,
                                                                    empDLine.EmployeeId,
                                                                    empDLine.Date,
                                                                    0,
                                                                    empDLine.ShiftCodeId
                                                                );
                                                        }
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                    }

                                    currentProcessedEmpId = log.EmployeeId;
                                    isWorkDayCompleted = true;
                                }

                                if (
                                    dline is not null
                                    && dline.TimeIn1 != null
                                    && dline.TimeOut2 != null
                                    && !aWeekIsLapsed
                                )
                                { /* week lapse tracking */
                                }

                                if (dline is not null && isWorkDayCompleted)
                                {
                                    if (
                                        !isLongShift
                                        && dline.NoTimeOut2
                                        && dline.Is2SwipesOnly
                                        && (
                                            log.LogType == LogTypeBreakStart
                                            || log.LogType == LogTypeBreakEnd
                                        )
                                    )
                                    {
                                        isWorkDayCompleted = true;
                                        continue;
                                    }

                                    if (
                                        (
                                            log.LogType == LogTypeOut
                                            || log.LogType == LogTypeBreakStart
                                        )
                                        && dline.NoTimeOut2
                                        && !dline.Is2SwipesOnly
                                    )
                                    {
                                        dline.TimeOut1 = DateTime.Parse(
                                            $"{log.Date} {log.Time:hh:mm tt}"
                                        );
                                        dline.HalfdayAbsent = true;
                                        continue;
                                    }

                                    if (
                                        dline.TimeIn1 is not null
                                        && dline.TimeOut1 is null
                                        && dline.TimeIn2 is null
                                        && dline.TimeOut2 is not null
                                        && (
                                            log.LogType == LogTypeBreakStart
                                            || log.LogType == LogTypeBreakEnd
                                        )
                                    )
                                    {
                                        var shiftDateParts = dline.ShiftDates?.Split(",");
                                        var logDTOnWorkDone = DateTime.Parse(
                                            $"{log.Date} {log.Time}"
                                        );
                                        if (
                                            shiftDateParts is not null
                                            && !string.IsNullOrEmpty(shiftDateParts[0])
                                            && string.IsNullOrEmpty(shiftDateParts[1])
                                            && string.IsNullOrEmpty(shiftDateParts[2])
                                            && !string.IsNullOrEmpty(shiftDateParts[3])
                                            && logDTOnWorkDone > lastTimeOutOfWorkShift
                                        )
                                        {
                                            isWorkDayCompleted = true;
                                            continue;
                                        }
                                    }

                                    dline = dtrLines.FirstOrDefault(
                                        x =>
                                            x.EmployeeId == log.EmployeeId
                                            && DateOnly.FromDateTime(x.Date) == log.Date
                                    );

                                    if (
                                        dline is not null
                                        && dline.TimeIn1 is not null
                                        && dline.TimeOut2 is not null
                                        && isWorkDayCompleted
                                    )
                                    {
                                        dline.IsSplitted = true;
                                        dline = dtrLines.FirstOrDefault(
                                            x =>
                                                x.EmployeeId == log.EmployeeId
                                                && DateOnly.FromDateTime(x.Date)
                                                    == log.Date.AddDays(1)
                                        );
                                        if (dline is not null)
                                            dline.IsSplitted = true;
                                        dlineIsJumped = true;
                                    }

                                    if (isLongShift && log.LogType == LogTypeOut)
                                    {
                                        var lastTimeOut2 = dtrLines
                                            .Where(
                                                x =>
                                                    x.EmployeeId == log.EmployeeId
                                                    && x.TimeOut2 != null
                                                    && x.TimeOut2
                                                        == DateTime.Parse($"{log.Date} {log.Time}")
                                            )
                                            .OrderByDescending(x => x.Date)
                                            .FirstOrDefault();
                                        if (lastTimeOut2 != null)
                                            continue;
                                    }

                                    if (isFlexBreak && log.LogType == LogTypeOut)
                                    {
                                        dline = dtrLines.FirstOrDefault(
                                            x =>
                                                x.EmployeeId == log.EmployeeId
                                                && DateOnly.FromDateTime(x.Date) == log.Date
                                        );
                                        if (dline is not null && dline.TimeIn1 is not null)
                                            dline.TimeOut2 = DateTime.Parse(
                                                $"{log.Date} {log.Time:hh:mm tt}"
                                            );
                                        isWorkDayCompleted = true;
                                        continue;
                                    }

                                    if (!isFlexBreak && log.LogType == LogTypeOut)
                                    {
                                        dline = dtrLines.FirstOrDefault(
                                            x =>
                                                x.EmployeeId == log.EmployeeId
                                                && DateOnly.FromDateTime(x.Date) == log.Date
                                        );
                                        if (dline is not null)
                                            dline.TimeOut2 = DateTime.Parse(
                                                $"{log.Date} {log.Time:hh:mm tt}"
                                            );
                                        isWorkDayCompleted = true;
                                        continue;
                                    }

                                    if (employeeShiftCodes is not null && dline is not null)
                                    {
                                        var escDays2 = GetEmployeeShiftCodeDays(
                                            employeeShiftCodes,
                                            shiftCodeDays,
                                            dline.EmployeeId,
                                            dline.Date.ToString("dddd"),
                                            DateTime.Parse($"{log.Date:d} {log.Time:hh:mm tt}"),
                                            isLongShift ? log.LogType : null
                                        );
                                        dline.ShiftCodeId = shiftCodeId = QuickChangeShiftv2(
                                            context,
                                            escDays2,
                                            employeeShiftCodes,
                                            dline.EmployeeId,
                                            dline.Date,
                                            0,
                                            dline.ShiftCodeId
                                        );

                                        if (log.LogType == LogTypeIn)
                                            dline.TimeIn1ShiftCodeId = dline.ShiftCodeId;

                                        if (shiftCodeId == 0)
                                            dline.ShiftCodeId = shiftCodeId =
                                                employees?.FirstOrDefault(
                                                    x => x.Id == log.EmployeeId
                                                )?.ShiftCodeId ?? 0;

                                        isWorkDayCompleted = false;
                                    }
                                }

                                var logType = log.LogType;
                                var filteredShiftDays = shiftCodeDays.Where(
                                    x => x.ShiftCodeId == shiftCodeId
                                );
                                var shiftCodeDay = new MstShiftCodeDay();

                                // ── Shift code day resolution (goto replaced with bounded while-loop) ──
                                var shiftCodeResolved = false;
                                var weekIterations = 0;

                                while (!shiftCodeResolved && weekIterations++ < 54)
                                {
                                    if (firstDateOfLogWeekly == DefaultDateOnly)
                                    {
                                        firstDateOfLogWeekly =
                                            fLogs?.OrderBy(x => x.Date)?.FirstOrDefault(
                                                x => x.EmployeeId == log.EmployeeId
                                            )?.Date ?? DefaultDateOnly;
                                    }

                                    dateOfLogWeekly = firstDateOfLogWeekly;
                                    var startIdx = days.IndexOf(
                                        firstDateOfLogWeekly.ToString("dddd")
                                    );
                                    var weekDayMap = new Dictionary<string, int>();

                                    for (int i = 0; i < 7; i++)
                                    {
                                        if (startIdx >= days.Count)
                                            startIdx = 0;
                                        weekDayMap.Add(days[startIdx], i);
                                        startIdx++;
                                    }

                                    var tempDate = firstDateOfLogWeekly;
                                    var orderedShiftDays = filteredShiftDays
                                        .Select(
                                            sc =>
                                            {
                                                sc.Sort = weekDayMap[sc.Day];
                                                return sc;
                                            }
                                        )
                                        .OrderBy(x => x.Sort)
                                        .ToList();

                                    foreach (var sc in orderedShiftDays)
                                    {
                                        var scDate = firstDateOfLogWeekly.AddDays(weekDayMap[sc.Day]);

                                        sc.TimeIn1 = DateTime.Parse(
                                            $"{scDate} {sc.TimeIn1:hh:mm tt}"
                                        );

                                        if (sc.TimeOut1 is not null)
                                        {
                                            var in1o = TimeOnly.FromDateTime((DateTime)sc.TimeIn1);
                                            var o1o = TimeOnly.FromDateTime((DateTime)sc.TimeOut1);
                                            sc.TimeOut1 =
                                                in1o > o1o
                                                    ? DateTime
                                                      .Parse($"{scDate} {sc.TimeOut1:hh:mm tt}")
                                                      .AddDays(1)
                                                    : DateTime.Parse(
                                                          $"{scDate} {sc.TimeOut1:hh:mm tt}"
                                                      );
                                        }
                                        if (sc.TimeIn2 is not null)
                                        {
                                            var in1o = TimeOnly.FromDateTime((DateTime)sc.TimeIn1);
                                            var in2o = TimeOnly.FromDateTime((DateTime)sc.TimeIn2);
                                            sc.TimeIn2 =
                                                in1o > in2o
                                                    ? DateTime
                                                      .Parse($"{scDate} {sc.TimeIn2:hh:mm tt}")
                                                      .AddDays(1)
                                                    : DateTime.Parse(
                                                          $"{scDate} {sc.TimeIn2:hh:mm tt}"
                                                      );
                                        }
                                        if (sc.TimeOut2 is not null)
                                        {
                                            var in1o = TimeOnly.FromDateTime((DateTime)sc.TimeIn1);
                                            var out2o = TimeOnly.FromDateTime(
                                                (DateTime)sc.TimeOut2
                                            );
                                            sc.TimeOut2 =
                                                in1o > out2o
                                                    ? DateTime
                                                      .Parse($"{scDate} {sc.TimeOut2:hh:mm tt}")
                                                      .AddDays(1)
                                                    : DateTime.Parse(
                                                          $"{scDate} {sc.TimeOut2:hh:mm tt}"
                                                      );
                                        }
                                    }

                                    var logDateShiftDay = UnknownDayKey;
                                    MstShiftCodeDay? firstShiftCode = null;

                                    foreach (var sc in orderedShiftDays)
                                    {
                                        firstShiftCode ??= sc;
                                        var sIn1 = sc.TimeIn1.HasValue 
                                            ? sc.TimeIn1.Value.AddHours(ShiftScanBufferBefore)
                                            : DefaultDate.AddHours(ShiftScanBufferBefore);
                                        var sOut2 = sc.TimeOut2.HasValue
                                            ? sc.TimeOut2.Value.AddHours(ShiftScanBufferAfter)
                                            : DefaultDate.AddHours(ShiftScanBufferAfter);
                                        if (logDateTime >= sIn1 && logDateTime <= sOut2)
                                            logDateShiftDay = sc?.Day ?? UnknownDayKey;
                                    }

                                    if (logDateShiftDay != UnknownDayKey)
                                    {
                                        shiftCodeDay = orderedShiftDays.FirstOrDefault(
                                            x => x.Day == logDateShiftDay
                                        );
                                        shiftCodeResolved = true;
                                    }
                                    else
                                    {
                                        var lastShiftTimeout =
                                            orderedShiftDays.LastOrDefault()?.TimeOut2;
                                        if (logDateTime <= lastShiftTimeout)
                                        {
                                            var filterTime =
                                                (
                                                    log.LogType == LogTypeOut
                                                    && dline?.TimeIn1 is not null
                                                )
                                                    ? dline?.TimeIn1 ?? DefaultDate
                                                    : logDateTime;
                                            shiftCodeDay =
                                                orderedShiftDays.FirstOrDefault(
                                                    x => x.Day == filterTime.ToString("dddd")
                                                )
                                                ?? orderedShiftDays.FirstOrDefault(
                                                    x =>
                                                        x.Day
                                                        == logDateTime.AddDays(1).ToString("dddd")
                                                );
                                            shiftCodeResolved = true;
                                        }
                                        else
                                        {
                                            // Log falls outside current week window — advance and retry
                                            firstDateOfLogWeekly = firstDateOfLogWeekly.AddDays(7);
                                        }
                                    }
                                }
                                // ── End shift code day resolution ─────────────────────────────────────────

                                if (
                                    (
                                        shiftCodeDay?.TimeOut1 is null
                                        && shiftCodeDay?.TimeIn2 is null
                                    )
                                    && (
                                        log.LogType == LogTypeBreakStart
                                        || log.LogType == LogTypeBreakEnd
                                    )
                                    && !isLongShift
                                )
                                {
                                    if (dline is not null && isFlexBreak)
                                    {
                                        if (dline.TimeIn2 == null && log.LogType == LogTypeBreakEnd)
                                            dline.TimeIn2 = logDateTime;
                                        if (log.LogType == LogTypeBreakStart)
                                            dline.TimeOut1 = logDateTime;

                                        var workDayLastLog = fLogs?.Where(
                                            x =>
                                                x.EmployeeId == dline.EmployeeId
                                                && DateTime.Parse($"{x.Date} {x.Time}")
                                                    >= shiftCodeDay?.TimeIn1?.AddHours(
                                                        ShiftWindowLookBackHours
                                                    )
                                                && DateTime.Parse($"{x.Date} {x.Time}")
                                                    <= shiftCodeDay?.TimeOut2?.AddHours(
                                                        ShiftWindowLookAheadHours
                                                    )
                                                && x.LogType != LogTypeIn
                                        )?.OrderByDescending(
                                            x => DateTime.Parse($"{x.Date} {x.Time}")
                                        )?.FirstOrDefault();

                                        var lastLogDT = DateTime.Parse(
                                            $"{workDayLastLog?.Date} {workDayLastLog?.Time}"
                                        );
                                        if (
                                            (workDayLastLog?.LogType ?? UnknownDayKey) != LogTypeOut
                                            && logDateTime == lastLogDT
                                        )
                                            isWorkDayCompleted = true;
                                    }
                                }

                                if (dline is not null && shiftCodeDay is not null)
                                {
                                    dline.IsShiftCodeIsTommorow = shiftCodeDay.IsTommorow;

                                    if (dline.ShiftDates is null && log.LogType == LogTypeIn)
                                    {
                                        dline.ShiftDates = string.Join(
                                            ",",
                                            shiftCodeDay.TimeIn1,
                                            shiftCodeDay.TimeOut1,
                                            shiftCodeDay.TimeIn2,
                                            shiftCodeDay.TimeOut2
                                        );

                                        if (isLongShift)
                                        {
                                            var prevDLine = dtrLines
                                                .Where(
                                                    x =>
                                                        x.EmployeeId == log.EmployeeId
                                                        && x.Date < dline.Date
                                                )
                                                .OrderByDescending(x => x.Date)
                                                .FirstOrDefault();
                                            if (
                                                prevDLine is not null
                                                && logDateTime.Date
                                                    == prevDLine.TimeOut2.GetValueOrDefault().Date
                                                && shiftCodeDay.IsTommorow
                                            )
                                            {
                                                dline.ShiftDates = string.Join(
                                                    ",",
                                                    shiftCodeDay.TimeIn1?.AddDays(-1).ToString()
                                                        ?? string.Empty,
                                                    shiftCodeDay.TimeOut1?.AddDays(-1).ToString()
                                                        ?? string.Empty,
                                                    shiftCodeDay.TimeIn2?.AddDays(-1).ToString()
                                                        ?? string.Empty,
                                                    shiftCodeDay.TimeOut2?.AddDays(-1).ToString()
                                                        ?? string.Empty
                                                );
                                            }
                                        }
                                    }

                                    if (fLogs is not null && shiftCodeDay is not null)
                                    {
                                        if (dlineIsJumped)
                                        {
                                            shiftCodeDay.TimeIn1?.AddDays(1);
                                            shiftCodeDay.TimeOut2?.AddDays(1);
                                        }

                                        var closestOut = fLogs
                                            .Where(
                                                x =>
                                                    x.EmployeeId == dline.EmployeeId
                                                    && DateTime.Parse($"{x.Date} {x.Time}")
                                                        >= shiftCodeDay?.TimeIn1
                                                    && DateTime.Parse($"{x.Date} {x.Time}")
                                                        <= shiftCodeDay?.TimeOut2?.AddHours(
                                                            ShiftWindowLookAheadHours
                                                        )
                                                    && (
                                                        x.LogType == LogTypeOut
                                                        || x.LogType == LogTypeBreakStart
                                                    )
                                            )
                                            .OrderByDescending(
                                                x => DateTime.Parse($"{x.Date} {x.Time}")
                                            )
                                            .FirstOrDefault();

                                        if (
                                            shiftCodeDay?.TimeOut1 == null
                                            && shiftCodeDay?.TimeIn2 == null
                                        )
                                        {
                                            closestOut = fLogs
                                                .Where(
                                                    x =>
                                                        x.EmployeeId == dline.EmployeeId
                                                        && DateTime.Parse($"{x.Date} {x.Time}")
                                                            >= shiftCodeDay?.TimeIn1
                                                        && DateTime.Parse($"{x.Date} {x.Time}")
                                                            <= shiftCodeDay?.TimeOut2?.AddHours(
                                                                ShiftWindowLookAheadHours
                                                            )
                                                        && x.LogType == LogTypeOut
                                                )
                                                .OrderByDescending(
                                                    x => DateTime.Parse($"{x.Date} {x.Time}")
                                                )
                                                .FirstOrDefault();
                                        }

                                        if (closestOut is not null)
                                            lastTimeOutOfWorkShift = DateTime.Parse(
                                                $"{closestOut.Date} {closestOut.Time}"
                                            );
                                    }

                                    if (logType == LogTypeIn || logType == LogTypeBreakEnd)
                                    {
                                        if (
                                            (shiftCodeDay?.TimeIn1 ?? dline.Date) != dline.Date
                                            && (shiftCodeDay?.TimeOut1 ?? dline.Date) != dline.Date
                                            && shiftCodeDay?.TimeOut2 != dline.Date
                                        )
                                        {
                                            if (
                                                shiftCodeDay?.TimeIn1 > logDateTime
                                                || (
                                                    logDateTime >= shiftCodeDay?.TimeIn1
                                                    && logDateTime < shiftCodeDay?.TimeOut2
                                                )
                                            )
                                            {
                                                if (dline.TimeIn1 is null)
                                                {
                                                    if (dline.NoTimeIn1)
                                                        dline.TimeIn2 = logDateTime;
                                                    else
                                                        dline.TimeIn1 = logDateTime;
                                                }
                                                else
                                                {
                                                    if (
                                                        logDateTime < (dline.TimeIn1 ?? DefaultDate)
                                                    )
                                                    {
                                                        var nextLog =
                                                            fLogs?.Where(
                                                                x =>
                                                                    x.EmployeeId == log.EmployeeId
                                                                    && DateTime.Parse(
                                                                        $"{x.Date} {x.Time}"
                                                                    ) > logDateTime
                                                            )?.OrderBy(
                                                                x =>
                                                                    DateTime.Parse(
                                                                        $"{x.Date} {x.Time}"
                                                                    )
                                                            )?.FirstOrDefault() ?? new TmpDtrLogs();
                                                        var nextLogDT = DateTime.Parse(
                                                            $"{nextLog.Date} {nextLog.Time}"
                                                        );
                                                        var intervalHours =
                                                            (nextLogDT - logDateTime).TotalHours;
                                                        if (intervalHours > LongShiftGapHours)
                                                        {
                                                            continue;
                                                        }
                                                        dline.TimeIn1 = logDateTime;
                                                    }
                                                    else
                                                    {
                                                        var timeOut1Compare =
                                                            shiftCodeDay?.TimeOut1;
                                                        if (timeOut1Compare is not null)
                                                        {
                                                            var interval =
                                                                (
                                                                    timeOut1Compare.Value
                                                                    - logDateTime
                                                                ).TotalHours;
                                                            if (interval > 2)
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        dline.TimeIn2 = logDateTime;
                                                    }
                                                }
                                            }
                                            else if (
                                                logDateTime > shiftCodeDay?.TimeOut1
                                                && logDateTime < shiftCodeDay?.TimeOut2
                                            )
                                            {
                                                if (dline.TimeIn2 is null)
                                                    dline.TimeIn2 = logDateTime;
                                                else if (
                                                    logDateTime < (dline.TimeIn2 ?? DefaultDate)
                                                )
                                                    dline.TimeIn2 = logDateTime;
                                            }
                                        }

                                        if (
                                            (shiftCodeDay?.TimeIn1 ?? dline.Date) != dline.Date
                                            && (shiftCodeDay?.TimeOut1 ?? dline.Date) == dline.Date
                                            && shiftCodeDay?.TimeOut2 != dline.Date
                                        )
                                        {
                                            if (
                                                logDateTime < shiftCodeDay?.TimeIn1
                                                || (
                                                    logDateTime >= shiftCodeDay?.TimeIn1
                                                    && logDateTime < shiftCodeDay?.TimeOut2
                                                )
                                            )
                                            {
                                                if (dline.TimeIn1 is null)
                                                {
                                                    // Eggs Changes 04/13/2026
                                                    if (log.LogType != LogTypeBreakEnd)
                                                        dline.TimeIn1 = logDateTime;
                                                }
                                                else
                                                {
                                                    if (
                                                        logDateTime < (dline.TimeIn1 ?? DefaultDate)
                                                    )
                                                        dline.TimeIn1 = logDateTime;
                                                }
                                            }
                                        }

                                        if (fLogs is not null && shiftCodeDay is not null)
                                        {
                                            var workDaysWithOuts = fLogs
                                                .Where(
                                                    x =>
                                                        x.EmployeeId == dline.EmployeeId
                                                        && DateTime.Parse($"{x.Date} {x.Time}")
                                                            >= shiftCodeDay?.TimeIn1?.AddHours(
                                                                ShiftWindowLookBackHours
                                                            )
                                                        && DateTime.Parse($"{x.Date} {x.Time}")
                                                            <= shiftCodeDay?.TimeOut2?.AddHours(
                                                                ShiftWindowLookAheadHours
                                                            )
                                                        && x.LogType == LogTypeOut
                                                )
                                                .OrderByDescending(
                                                    x => DateTime.Parse($"{x.Date} {x.Time}")
                                                );

                                            var is4Swipes =
                                                shiftCodeDay?.TimeIn1 != null
                                                && shiftCodeDay?.TimeOut1 != null
                                                && shiftCodeDay?.TimeIn2 != null
                                                && shiftCodeDay?.TimeOut2 != null;

                                            if (is4Swipes)
                                            {
                                                workDaysWithOuts = fLogs
                                                    .Where(
                                                        x =>
                                                            x.EmployeeId == dline.EmployeeId
                                                            && DateTime.Parse($"{x.Date} {x.Time}")
                                                                >= shiftCodeDay?.TimeIn1?.AddHours(
                                                                    ShiftWindowLookBackHours
                                                                )
                                                            && DateTime.Parse($"{x.Date} {x.Time}")
                                                                <= shiftCodeDay?.TimeOut2?.AddHours(
                                                                    ShiftWindowLookAheadHours
                                                                )
                                                            && (
                                                                x.LogType == LogTypeBreakStart
                                                                || x.LogType == LogTypeOut
                                                            )
                                                    )
                                                    .OrderByDescending(
                                                        x => DateTime.Parse($"{x.Date} {x.Time}")
                                                    );
                                            }

                                            if (!workDaysWithOuts.Any() && !isFlexBreak)
                                            {
                                                if (isLongShift)
                                                {
                                                    if (
                                                        shiftCodeDay.TimeOut1 == null
                                                        && shiftCodeDay.TimeIn2 == null
                                                    )
                                                        dline.Is2SwipesOnly = true;

                                                    var nextLog = fLogs
                                                        .Where(
                                                            x =>
                                                                x.EmployeeId == dline.EmployeeId
                                                                && DateTime.Parse(
                                                                    $"{x.Date} {x.Time}"
                                                                ) > logDateTime
                                                        )
                                                        .OrderBy(
                                                            x =>
                                                                DateTime.Parse($"{x.Date} {x.Time}")
                                                        )
                                                        .Select(
                                                            x =>
                                                                DateTime.Parse($"{x.Date} {x.Time}")
                                                        )
                                                        .FirstOrDefault();
                                                    var logCountForShift = fLogs
                                                        .Where(
                                                            x =>
                                                                x.EmployeeId == dline.EmployeeId
                                                                && DateTime.Parse(
                                                                    $"{x.Date} {x.Time}"
                                                                ) >= logDateTime
                                                                && DateTime.Parse(
                                                                    $"{x.Date} {x.Time}"
                                                                )
                                                                    <= (
                                                                        shiftCodeDay?.TimeOut2
                                                                        ?? DefaultDate
                                                                    ).AddHours(
                                                                        ShiftWindowLookAheadHours
                                                                    )
                                                        )
                                                        .Count();
                                                    var hoursConsumed =
                                                        (nextLog - logDateTime).TotalHours;

                                                    if (hoursConsumed > LongShiftGapHours)
                                                    {
                                                        if (hoursConsumed > LongShiftMaxHours)
                                                        {
                                                            dline.NoTimeOut2 = true;
                                                            isWorkDayCompleted = true;
                                                            continue;
                                                        }
                                                        dline.TimeOut2 = nextLog;
                                                        if (
                                                            (
                                                                log.LogType == LogTypeIn
                                                                || log.LogType == LogTypeBreakEnd
                                                            )
                                                            && logCountForShift > 1
                                                        )
                                                        {
                                                            var nLog = fLogs
                                                                .Where(
                                                                    x =>
                                                                        DateTime.Parse(
                                                                            $"{x.Date} {x.Time}"
                                                                        ) == nextLog
                                                                )
                                                                .FirstOrDefault();
                                                            if (nLog is not null)
                                                                nLog.LogType = LogTypeOut;
                                                        }
                                                        isWorkDayCompleted = true;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }

                                                var closestIn = fLogs
                                                    .Where(
                                                        x =>
                                                            x.EmployeeId == dline.EmployeeId
                                                            && DateTime.Parse($"{x.Date} {x.Time}")
                                                                != dline?.TimeIn1
                                                            && DateTime.Parse($"{x.Date} {x.Time}")
                                                                >= shiftCodeDay?.TimeIn1
                                                            && DateTime.Parse($"{x.Date} {x.Time}")
                                                                <= shiftCodeDay?.TimeOut2?.AddHours(
                                                                    ShiftWindowLookAheadHours
                                                                )
                                                            && x.LogType == LogTypeIn
                                                    )
                                                    .OrderByDescending(
                                                        x => DateTime.Parse($"{x.Date} {x.Time}")
                                                    )
                                                    .FirstOrDefault();

                                                var workDayLogsI = fLogs.Where(
                                                    x =>
                                                        x.EmployeeId == dline.EmployeeId
                                                        && DateTime.Parse($"{x.Date} {x.Time}")
                                                            >= shiftCodeDay?.TimeIn1?.AddHours(
                                                                ShiftWindowLookBackHours
                                                            )
                                                        && DateTime.Parse($"{x.Date} {x.Time}")
                                                            <= shiftCodeDay?.TimeOut2?.AddHours(
                                                                ShiftWindowLookAheadHours
                                                            )
                                                );

                                                if (
                                                    workDayLogsI != null
                                                    && workDayLogsI.Any(x => x.LogType == LogTypeIn)
                                                )
                                                {
                                                    if (closestIn is not null)
                                                    {
                                                        lastTimeOutOfWorkShift = DateTime.Parse(
                                                            $"{closestIn.Date} {closestIn.Time}"
                                                        );
                                                        logType = "O";
                                                    }
                                                    else
                                                    {
                                                        if (is4Swipes)
                                                        {
                                                            var wdLastLog = fLogs
                                                                .Where(
                                                                    x =>
                                                                        x.EmployeeId
                                                                            == dline.EmployeeId
                                                                        && DateTime.Parse(
                                                                            $"{x.Date} {x.Time}"
                                                                        )
                                                                            >= shiftCodeDay?.TimeIn1?.AddHours(
                                                                                ShiftWindowLookBackHours
                                                                            )
                                                                        && DateTime.Parse(
                                                                            $"{x.Date} {x.Time}"
                                                                        )
                                                                            <= shiftCodeDay?.TimeOut2?.AddHours(
                                                                                ShiftWindowLookAheadHours
                                                                            )
                                                                        && (
                                                                            x.LogType == LogTypeIn
                                                                            || x.LogType
                                                                                == LogTypeBreakEnd
                                                                        )
                                                                )
                                                                .OrderByDescending(
                                                                    x =>
                                                                        DateTime.Parse(
                                                                            $"{x.Date} {x.Time}"
                                                                        )
                                                                )
                                                                .FirstOrDefault();

                                                            if (
                                                                wdLastLog != null
                                                                && DateTime.Parse(
                                                                    $"{wdLastLog.Date} {wdLastLog.Time}"
                                                                ) == logDateTime
                                                            )
                                                            {
                                                                var wdOuts2 = fLogs
                                                                    .Where(
                                                                        x =>
                                                                            x.EmployeeId
                                                                                == dline.EmployeeId
                                                                            && DateTime.Parse(
                                                                                $"{x.Date} {x.Time}"
                                                                            )
                                                                                >= shiftCodeDay?.TimeIn1?.AddHours(
                                                                                    ShiftWindowLookBackHours
                                                                                )
                                                                            && DateTime.Parse(
                                                                                $"{x.Date} {x.Time}"
                                                                            )
                                                                                <= shiftCodeDay?.TimeOut2?.AddHours(
                                                                                    ShiftWindowLookAheadHours
                                                                                )
                                                                            && x.LogType
                                                                                == LogTypeOut
                                                                    )
                                                                    .OrderByDescending(
                                                                        x =>
                                                                            DateTime.Parse(
                                                                                $"{x.Date} {x.Time}"
                                                                            )
                                                                    );

                                                                if (!wdOuts2.Any())
                                                                {
                                                                    dline.IsShiftCodeIsTommorow =
                                                                        shiftCodeDay?.IsTommorow
                                                                        ?? false;
                                                                    dline.NoTimeOut2 = true;
                                                                    isWorkDayCompleted = true;
                                                                    continue;
                                                                }
                                                                isWorkDayCompleted = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var wdLastLog2 = fLogs
                                                                .Where(
                                                                    x =>
                                                                        x.EmployeeId
                                                                            == dline.EmployeeId
                                                                        && DateTime.Parse(
                                                                            $"{x.Date} {x.Time}"
                                                                        )
                                                                            >= shiftCodeDay?.TimeIn1?.AddHours(
                                                                                ShiftWindowLookBackHours
                                                                            )
                                                                        && DateTime.Parse(
                                                                            $"{x.Date} {x.Time}"
                                                                        )
                                                                            <= shiftCodeDay?.TimeOut2?.AddHours(
                                                                                ShiftWindowLookAheadHours
                                                                            )
                                                                        && x.LogType == LogTypeIn
                                                                )
                                                                .OrderByDescending(
                                                                    x =>
                                                                        DateTime.Parse(
                                                                            $"{x.Date} {x.Time}"
                                                                        )
                                                                )
                                                                .FirstOrDefault();

                                                            if (
                                                                wdLastLog2 != null
                                                                && DateTime.Parse(
                                                                    $"{wdLastLog2.Date} {wdLastLog2.Time}"
                                                                ) == logDateTime
                                                            )
                                                            {
                                                                dline.IsShiftCodeIsTommorow =
                                                                    shiftCodeDay?.IsTommorow
                                                                    ?? false;
                                                                dline.Is2SwipesOnly = true;
                                                                dline.NoTimeOut2 = true;
                                                                isWorkDayCompleted = true;
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (logDateTime == lastTimeOutOfWorkShift)
                                            {
                                                if (dline != null)
                                                    dline.TimeOut2 = logDateTime;
                                            }
                                        }

                                        if (isFlexBreak)
                                        {
                                            var nextSwipe = fLogs?
                                                .Where(
                                                    x =>
                                                        x.EmployeeId == dline.EmployeeId
                                                        && DateTime.Parse($"{x.Date} {x.Time}")
                                                            > logDateTime
                                                )
                                                .OrderBy(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                                .FirstOrDefault();
                                            if (nextSwipe is not null)
                                            {
                                                var gapHrs =
                                                    (
                                                        DateTime.Parse(
                                                            $"{nextSwipe.Date} {nextSwipe.Time}"
                                                        ) - logDateTime
                                                    ).TotalHours;
                                                if (
                                                    gapHrs > OvernightSpanHours
                                                    && nextSwipe.LogType == LogTypeIn
                                                )
                                                {
                                                    isWorkDayCompleted = true;
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                    else if (logType == LogTypeOut || logType == LogTypeBreakStart)
                                    {
                                        if (
                                            (shiftCodeDay?.TimeOut1 ?? dline.Date) != dline.Date
                                            && shiftCodeDay?.TimeOut2 != dline.Date
                                            && shiftCodeDay?.TimeIn2 != dline.Date
                                        )
                                        {
                                            if (
                                                logDateTime > shiftCodeDay?.TimeIn1
                                                && logDateTime < shiftCodeDay?.TimeIn2
                                            )
                                            {
                                                if (dline.TimeOut1 is null)
                                                {
                                                    dline.TimeOut1 = logDateTime;
                                                    if (dline.TimeIn1 == null)
                                                        dline.NoTimeIn1 = true;
                                                }
                                                else if (
                                                    dline.TimeOut1 is not null
                                                    && logDateTime > dline.TimeOut1
                                                )
                                                    dline.TimeOut1 = logDateTime;
                                            }
                                            else if (logDateTime > shiftCodeDay?.TimeIn2)
                                            {
                                                if (dline.TimeOut2 is null)
                                                    dline.TimeOut2 = logDateTime;
                                                else if (
                                                    dline.TimeOut2 is not null
                                                    && logDateTime > (dline.TimeOut2 ?? DefaultDate)
                                                )
                                                    dline.TimeOut2 = logDateTime;
                                            }
                                        }

                                        if (
                                            (shiftCodeDay?.TimeOut1 ?? dline.Date) == dline.Date
                                            && shiftCodeDay?.TimeOut2 != dline.Date
                                        )
                                        {
                                            var scIn1 = TimeOnly.FromDateTime(
                                                shiftCodeDay?.TimeIn1 ?? DefaultDate
                                            );
                                            var scOut2 = TimeOnly.FromDateTime(
                                                shiftCodeDay?.TimeOut2 ?? DefaultDate
                                            );

                                            if (scIn1 > scOut2)
                                            {
                                                if (dline.TimeOut2 is null)
                                                    dline.TimeOut2 = logDateTime;
                                                else if (
                                                    dline.TimeOut2 is not null
                                                    && logDateTime > (dline.TimeOut2 ?? DefaultDate)
                                                )
                                                    dline.TimeOut2 = logDateTime;
                                            }
                                            else
                                            {
                                                if (
                                                    log.LogType == LogTypeOut
                                                    && logDateTime > shiftCodeDay?.TimeIn1
                                                )
                                                {
                                                    if (dline.TimeOut2 is null)
                                                        dline.TimeOut2 = logDateTime;
                                                    else if (
                                                        dline.TimeOut2 is not null
                                                        && logDateTime
                                                            > (dline.TimeOut2 ?? DefaultDate)
                                                    )
                                                        dline.TimeOut2 = logDateTime;
                                                }
                                            }
                                        }

                                        if (
                                            shiftCodeDay?.TimeIn1 != null
                                            && shiftCodeDay?.TimeOut1 != null
                                            && shiftCodeDay?.TimeIn2 != null
                                            && shiftCodeDay?.TimeOut2 != null
                                        )
                                            isWorkDayCompleted = true;
                                    }
                                }

                                if (logDateTime == lastTimeOutOfWorkShift && logType == LogTypeOut)
                                {
                                    isWorkDayCompleted = true;

                                    if (dlineIsJumped)
                                    {
                                        if (
                                            dline is not null
                                            && dline.Date.Date
                                                == (dline.TimeOut2?.Date ?? DefaultDate)
                                        )
                                            dline.IsSplitted = true;
                                        dlineIsJumped = false;
                                    }

                                    if (dline is not null && (shiftCodeDay?.IsTommorow ?? false))
                                    {
                                        if (
                                            DateOnly.FromDateTime(dline.TimeIn1 ?? DefaultDate)
                                            != DateOnly.FromDateTime(dline.TimeOut2 ?? DefaultDate)
                                        )
                                            dline.IsShiftCodeIsTommorow = true;
                                    }
                                }
                                else if (logType == LogTypeOut)
                                {
                                    isWorkDayCompleted = true;
                                }

                                // ── Week rollover tracking ──────────────────────────────────────────────────
                                if (fLogs is not null)
                                {
                                    var lastShiftOut2Date = DateOnly.FromDateTime(
                                        shiftCodeDays
                                            .Where(x => x.ShiftCodeId == shiftCodeId)
                                            .OrderByDescending(x => x.TimeOut2)
                                            .FirstOrDefault()?.TimeOut2 ?? DefaultDate
                                    );
                                    var lastShiftOut2Time = TimeOnly.FromDateTime(
                                        shiftCodeDays
                                            .Where(x => x.ShiftCodeId == shiftCodeId)
                                            .OrderByDescending(x => x.TimeOut2)
                                            .FirstOrDefault()?.TimeOut2 ?? DefaultDate
                                    );

                                    var lastLogOfWeek = fLogs
                                        .Where(
                                            x =>
                                                x.EmployeeId == log.EmployeeId
                                                && x.Date == lastShiftOut2Date
                                        )
                                        .OrderByDescending(
                                            x => DateTime.Parse($"{x.Date} {x.Time}")
                                        )
                                        .FirstOrDefault();
                                    var lastLogDTOfWeek = lastLogOfWeek is not null
                                        ? DateTime.Parse(
                                              $"{lastLogOfWeek.Date} {lastLogOfWeek.Time}"
                                          )
                                        : DefaultDate;
                                    var lastShiftDTOfWeek = DateTime.Parse(
                                        $"{lastShiftOut2Date} {lastShiftOut2Time}"
                                    );

                                    var nextLogOfEmp = fLogs
                                        .Where(
                                            x =>
                                                x.EmployeeId == log.EmployeeId
                                                && DateTime.Parse($"{x.Date} {x.Time}")
                                                    > logDateTime
                                        )
                                        .OrderBy(x => DateTime.Parse($"{x.Date} {x.Time}"))
                                        .FirstOrDefault();
                                    if (nextLogOfEmp is not null)
                                    {
                                        var nextLogDT = DateTime.Parse(
                                            $"{nextLogOfEmp.Date} {nextLogOfEmp.Time}"
                                        );
                                        if (
                                            isWorkDayCompleted
                                            && nextLogDT
                                                > lastShiftDTOfWeek.AddHours(WeekRolloverCheckHours)
                                        )
                                        {
                                            var dateTomorrow = (dline?.Date ?? DefaultDate).AddDays(
                                                1
                                            );
                                            var hasLogsNextDay = fLogs.Any(
                                                x => x.Date == DateOnly.FromDateTime(dateTomorrow)
                                            );
                                            if (!hasLogsNextDay)
                                                continue;

                                            var newShiftCodeDayTimeIn1 = DefaultDate;
                                            var escSetup3 = new EmployeeShiftCodeDay();
                                            if (employeeShiftCodes is not null && dline is not null)
                                            {
                                                escSetup3.ParamEmployeeId = dline.EmployeeId;
                                                escSetup3.ParamDay = dline.Date.ToString("dddd");
                                                escSetup3.ParamLogTimeIn1 = nextLogDT;
                                                var escDays3 = escSetup3.Result();
                                                var newShiftCodeId = QuickChangeShiftv2(
                                                    context,
                                                    escDays3,
                                                    employeeShiftCodes,
                                                    dline.EmployeeId,
                                                    dline.Date,
                                                    0,
                                                    dline.ShiftCodeId
                                                );
                                                if (shiftCodeId == 0)
                                                    newShiftCodeId =
                                                        employees?.FirstOrDefault(
                                                            x => x.Id == log.EmployeeId
                                                        )?.ShiftCodeId ?? 0;
                                                newShiftCodeDayTimeIn1 =
                                                    shiftCodeDays
                                                        .Where(x => x.ShiftCodeId == newShiftCodeId)
                                                        .FirstOrDefault()?.TimeIn1
                                                    ?? new DateTime();
                                            }

                                            var nextWeekFirstShift = DateTime.Parse(
                                                $"{firstDateOfLogWeekly.AddDays(7)} {TimeOnly.FromDateTime(newShiftCodeDayTimeIn1)}"
                                            );
                                            var interval =
                                                (nextWeekFirstShift - nextLogDT).TotalHours;

                                            firstDateOfLogWeekly =
                                                interval > WeekIntervalBoundaryHours
                                                    ? firstDateOfLogWeekly.AddDays(6)
                                                    : firstDateOfLogWeekly.AddDays(7);

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

            // ── Post-process: handle "IsTomorrow" shift lines ─────────────────────────────
            var empsWithTomorrowShift = dtrLines
                .Where(x => x.IsShiftCodeIsTommorow)
                .GroupBy(x => x.EmployeeId)
                .Select(x => x.Key);

            foreach (var employeeId in empsWithTomorrowShift)
            {
                var empDLines = dtrLines.Where(
                    x => x.EmployeeId == employeeId && x.IsShiftCodeIsTommorow && !x.IsSplitted
                );

                foreach (var empDLine in empDLines)
                {
                    var empDLineYesterday =
                        dtrLines.FirstOrDefault(
                            x =>
                                x.EmployeeId == employeeId
                                && !x.IsSplitted
                                && x.Date.Date == empDLine.Date.AddDays(-1).Date
                        ) ?? new TrnDtrLineDto();

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

                    if (dateStart == dateEnd)
                    {
                        var emp = employees.FirstOrDefault(a => a.Id == employeeId);
                        if (emp == null)
                            return;
                        var bioId = emp.BiometricIdNumber;

                        var startMain = dateStart.Date;
                        var endMain = startMain.AddDays(1).AddTicks(-1);
                        var startPrev = startMain.AddDays(-1);
                        var endPrev = startMain.AddTicks(-1);

                        var currentDayLogs = context.TrnLogs.Where(
                            d =>
                                d.BiometricIdNumber == bioId
                                && d.LogDateTime >= startMain
                                && d.LogDateTime <= endMain
                        );
                        var lastInYesterday = context.TrnLogs
                            .Where(
                                d =>
                                    d.BiometricIdNumber == bioId
                                    && d.LogType == LogTypeIn
                                    && d.LogDateTime >= startPrev
                                    && d.LogDateTime <= endPrev
                            )
                            .OrderByDescending(d => d.LogDateTime)
                            .Take(1);
                        var lastIn = lastInYesterday.FirstOrDefault();
                        var currentOut = currentDayLogs
                            .Where(a => a.LogType == LogTypeOut)
                            .FirstOrDefault();

                        if (lastIn?.LogDateTime is DateTime)
                            empDLine.TimeIn1 = lastIn.LogDateTime;
                        if (currentOut?.LogDateTime is DateTime)
                            empDLine.TimeOut2 = currentOut.LogDateTime;
                    }

                    var empDLineTomorrow = dtrLines.FirstOrDefault(
                        x =>
                            x.EmployeeId == employeeId
                            && x.Date.Date == empDLine.Date.AddDays(1).Date
                    );
                    if (empDLineTomorrow is not null)
                    {
                        empDLineTomorrow.OldShiftCodeId = empDLineTomorrow.ShiftCodeId;
                        empDLineTomorrow.OldTimeIn1 = empDLineTomorrow.TimeIn1;
                        empDLineTomorrow.OldTimeOut1 = empDLineTomorrow.TimeOut1;
                        empDLineTomorrow.OldTimeIn2 = empDLineTomorrow.TimeIn2;
                        empDLineTomorrow.OldTimeOut2 = empDLineTomorrow.TimeOut2;
                        empDLineTomorrow.OldShiftDates = empDLineTomorrow.ShiftDates;

                        empDLineTomorrow.ShiftCodeId = empDLine.OldShiftCodeId;
                        empDLineTomorrow.TimeIn1 = empDLine.OldTimeIn1;
                        empDLineTomorrow.TimeOut1 = empDLine.OldTimeOut1;
                        empDLineTomorrow.TimeIn2 = empDLine.OldTimeIn2;
                        empDLineTomorrow.TimeOut2 = empDLine.OldTimeOut2;
                        empDLineTomorrow.ShiftDates = empDLine.OldShiftDates;
                        empDLineTomorrow.IsDateMoved = true;
                    }
                }
            }

            // ── Assign default shift code to lines with no shift ──────────────────────────
            var defaultShiftCode = context.MstShiftCodes.FirstOrDefault(
                x => x.ShiftCode == DefaultShiftCodeName
            );
            foreach (var line in dtrLines.Where(x => x.ShiftCodeId == 0))
                line.ShiftCodeId = defaultShiftCode?.Id ?? 0;

            // ── Honour TimeIn1-detected shift code ────────────────────────────────────────
            foreach (var line in dtrLines)
            {
                if (
                    line.TimeIn1 != null
                    && line.ShiftCodeId != line.TimeIn1ShiftCodeId
                    && line.TimeIn1ShiftCodeId != 0
                )
                    line.ShiftCodeId = line.TimeIn1ShiftCodeId;

                line.RegularHours = ComputeRegularHours(
                    new TrnDtrline
                    {
                        TimeIn1 = line.TimeIn1,
                        TimeOut1 = line.TimeOut1,
                        TimeIn2 = line.TimeIn2,
                        TimeOut2 = line.TimeOut2,
                        DayTypeId = line.DayTypeId,
                        ShiftCodeId = line.ShiftCodeId,
                        EmployeeId = line.EmployeeId
                    },
                    shiftCodeDays,
                    true,
                    context
                );
                line.TardyLateHours = ComputeTardyLateHours(
                    new TrnDtrline
                    {
                        Date = line.Date,
                        ShiftCodeId = line.ShiftCodeId,
                        TimeIn1 = line.TimeIn1,
                        TimeOut1 = line.TimeOut1,
                        TimeIn2 = line.TimeIn2,
                        TimeOut2 = line.TimeOut2,
                        ShiftDates = line.ShiftDates,
                        EmployeeId = line.EmployeeId
                    },
                    shiftCodeDays,
                    employees,
                    context
                );
                line.NetTotalHours = line.RegularHours - line.TardyLateHours;
            }

            employees = null;
            shiftCodeDays = null;
            employeeShiftCodes = null;
        }

        internal static void ComputeDtrLines(
            TrnDtr dtr,
            EditDtrLinesByComputeDtr command,
            HRISContext context
        )
        {
            var empId = command?.EmployeeId;
            var dtrLines = empId is not null
                ? dtr.TrnDtrlines.Where(x => x.EmployeeId == empId).ToList()
                : dtr.TrnDtrlines;

            var employees = context.MstEmployees.ToArray();
            var shiftCodeDays = context.MstShiftCodeDays.ToArray();
            var dayTypeDays = context.MstDayTypeDays.ToArray();

            foreach (var line in dtrLines)
            {
                try
                {
                    if (command is null || line is null)
                        continue;

                    var empLines = dtrLines?.Where(x => x.EmployeeId == empId)?.OrderBy(
                        x => x.Date
                    )?.ToArray();
                    if (empLines is not null)
                    {
                        var lineDate = line?.Date;
                        var empStartDate = empLines.FirstOrDefault()?.Date;
                        if (lineDate == empStartDate && lineDate != command.DateStart)
                        {
                            if (string.IsNullOrEmpty(line?.Dtrremarks?.Trim()))
                                continue;
                        }
                    }

                    if (!dtr.IsComputeRestDay)
                    {
                        line.RestDay = ComputeRestDay(line, shiftCodeDays);
                        line.OnLeave = ComputeOnLeave(line, context);
                        line.Absent = ComputeAbsent(line, context);
                        line.HalfdayAbsent = false;
                    }

                    // ── Holiday eligibility ───────────────────────────────────────────────
                    var isEligibleForHolidayPay = true;
                    var empRecord = employees.FirstOrDefault(x => x.Id == line.EmployeeId);
                    var isProjectBased = empRecord?.EmploymentType ?? 0;
                    var payrollTypeId = empRecord?.PayrollTypeId ?? 0;

                    if (isProjectBased == EmploymentTypeProjectBased)
                        isEligibleForHolidayPay = false;

                    if (line.DayTypeId > DayTypeWorking)
                    {
                        var dayTypeSetup = dayTypeDays.FirstOrDefault(
                            x => x.Date.Date == line.Date.Date && x.BranchId == empRecord.BranchId
                        );
                        var dateBefore = dayTypeSetup?.DateBefore ?? DefaultDate;
                        var dateAfter = dayTypeSetup?.DateAfter ?? DefaultDate;

                        var lineBefore = dtrLines.FirstOrDefault(
                            x => x.EmployeeId == line.EmployeeId && x.Date == dateBefore
                        );
                        var isAbsentBefore = lineBefore?.Absent ?? true;
                        if (lineBefore != null && (lineBefore.OnLeave || lineBefore.RestDay))
                            isAbsentBefore = false;

                        var lineAfter = dtrLines.FirstOrDefault(
                            x => x.EmployeeId == line.EmployeeId && x.Date == dateAfter
                        );
                        var isAbsentAfter = lineAfter?.Absent ?? true;
                        if (lineAfter != null && (lineAfter.OnLeave || lineAfter.RestDay))
                            isAbsentAfter = false;

                        if (isAbsentBefore || isAbsentAfter)
                            isEligibleForHolidayPay = false;

                        if (
                            whris.Application.Common.Common.GlobalSettings.EnableHolidayPay == true
                            && line.DayTypeId == DayTypeRegularHoliday
                            && payrollTypeId == PayrollTypeVariable
                            && dateBefore == dateAfter
                        )
                        {
                            isEligibleForHolidayPay = true;
                        }
                    }

                    // ── Compute hours ─────────────────────────────────────────────────────
                    line.RegularHours = ComputeRegularHours(
                        line,
                        shiftCodeDays,
                        isEligibleForHolidayPay,
                        context
                    );
                    line.NightHours = ComputeNightHours(line, shiftCodeDays);
                    line.OvertimeHours = ComputeOverTimeHours(line, context);
                    line.OvertimeNightHours = ComputeOvertimeNightHours(line, context);
                    line.GrossTotalHours = ComputeGrossTotalHours(line);
                    line.TardyLateHours = ComputeTardyLateHours(
                        line,
                        shiftCodeDays,
                        employees,
                        context
                    );

                    line.TardyUndertimeHours = string.IsNullOrEmpty(line.ShiftDates)
                        ? ComputeTardyUndertimeHours(line, shiftCodeDays, context)
                        : ComputeTardyUndertimeHoursv2(line, shiftCodeDays, context);

                    if (Math.Abs(line.TardyUndertimeHours) > 4)
                        line.TardyUndertimeHours = ComputeTardyUndertimeHours(
                            line,
                            shiftCodeDays,
                            context
                        );

                    line.NetTotalHours = ComputeNetTotalHours(line);

                    // ── Compute multiplier & rates ────────────────────────────────────────
                    line.DayMultiplier = ComputeDayMultiplier(
                        line,
                        employees,
                        dayTypeDays,
                        isEligibleForHolidayPay,
                        context
                    );
                    line.RatePerHour = ComputeRatePerHour(line, employees);
                    line.RatePerNightHour = ComputeRatePerNightHour(line, employees);
                    line.RatePerOvertimeHour = ComputeRatePerOvertimeHour(line, employees);
                    line.RatePerOvertimeNightHour = ComputeRatePerOvertimeNightHour(
                        line,
                        employees
                    );

                    // ── Compute amounts ───────────────────────────────────────────────────
                    line.RegularAmount = ComputeRegularAmount(line);
                    line.NightAmount = ComputeNightAmount(line, isEligibleForHolidayPay);

                    // Absent override for project-based on holiday
                    if (
                        empRecord is not null
                        && empRecord.PayrollTypeId == PayrollTypeProjectBased
                        && line.DayTypeId > DayTypeWorking
                    )
                        line.Absent = false;

                    if (
                        payrollTypeId != PayrollTypeProjectBased
                        && line.DayTypeId > DayTypeWorking
                        && !isEligibleForHolidayPay
                    )
                    {
                        if (line is not null && HasNoTimeSwipes(line))
                        {
                            line.Absent = true;
                            line.RegularHours = 0;
                            line.RegularAmount = 0;
                        }
                    }

                    // OvertimeAmount and OvertimeNightAmount computed once (after Absent adjustments)
                    line.OvertimeAmount = ComputeOverTimeAmount(
                        line,
                        employees,
                        dayTypeDays,
                        isEligibleForHolidayPay
                    );
                    line.OvertimeNightAmount = ComputeOvertimeNightAmount(
                        line,
                        employees,
                        dayTypeDays
                    );

                    line.TotalAmount = ComputeTotalAmount(
                        line,
                        shiftCodeDays,
                        isEligibleForHolidayPay,
                        context
                    );
                    line.RatePerHourTardy = ComputeRatePerHourTardy(line, employees);
                    line.RatePerAbsentDay = ComputeRatePerAbsentDay(line, employees);
                    line.TardyAmount = ComputeTardyAmount(line, context);
                    line.AbsentAmount = ComputeAbsentAmount(line);
                    line.NetAmount = ComputeNetAmount(line);

                    // ── Half-day absent adjustments ───────────────────────────────────────
                    if (line.HalfdayAbsent && line.Employee.IsFlexBreak == false)
                    {
                        if (line.TardyLateHours > line.TardyUndertimeHours)
                        {
                            var savedLate = line.TardyLateHours;
                            line.TardyLateHours = 0;

                            if (line.TimeOut1 == null && line.TimeIn2 == null)
                            {
                                line.TardyLateHours =
                                    savedLate > (line.RegularHours / 2)
                                        ? (line.RegularHours / 2)
                                        : savedLate;
                                if (savedLate > ((line.RegularHours / 2) + 1))
                                    line.TardyLateHours = savedLate - 1;
                                line.TardyAmount = ComputeTardyAmount(line, context);
                                line.RegularHours = line.RegularHours / 2;
                                line.GrossTotalHours = line.NetTotalHours;
                                line.NetAmount = ComputeNetAmount(line);
                                line.TardyLateHours = 0;
                                line.TardyAmount = 0;
                                if (line.NightHours > line.RegularHours)
                                {
                                    line.NightHours = line.RegularHours;
                                    line.NightAmount = ComputeNightAmount(
                                        line,
                                        isEligibleForHolidayPay
                                    );
                                }
                            }
                            else
                            {
                                if (savedLate > (line.RegularHours / 2))
                                {
                                    line.TardyLateHours =
                                        savedLate > (line.RegularHours / 2)
                                            ? (line.RegularHours / 2)
                                            : savedLate;
                                    if (savedLate > ((line.RegularHours / 2) + 1))
                                        line.TardyLateHours = savedLate - 1;
                                    line.TardyAmount = ComputeTardyAmount(line, context);
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
                            var savedUnder = line.TardyUndertimeHours;
                            line.TardyUndertimeHours = 0;

                            if (line.TimeOut1 == null && line.TimeIn2 == null)
                            {
                                line.TardyUndertimeHours = (line.RegularHours / 2);
                                if (savedUnder > ((line.RegularHours / 2) + 1))
                                    line.TardyUndertimeHours = savedUnder - 1;
                                line.TardyAmount = ComputeTardyAmount(line, context);
                                line.NetTotalHours = ComputeNetTotalHours(line);
                                line.RegularHours = line.RegularHours / 2;
                                line.GrossTotalHours = line.NetTotalHours;
                                line.NetAmount = ComputeNetAmount(line);
                                line.TardyUndertimeHours = 0;
                                line.TardyAmount = 0;
                                if (line.NightHours > line.RegularHours)
                                {
                                    line.NightHours = line.RegularHours;
                                    line.NightAmount = ComputeNightAmount(
                                        line,
                                        isEligibleForHolidayPay
                                    );
                                }
                            }
                            else
                            {
                                if (savedUnder > (line.RegularHours / 2))
                                {
                                    line.TardyUndertimeHours =
                                        savedUnder > (line.RegularHours / 2)
                                            ? (line.RegularHours / 2)
                                            : savedUnder;
                                    if (savedUnder > ((line.RegularHours / 2) + 1))
                                        line.TardyUndertimeHours = savedUnder - 1;
                                    line.TardyAmount = ComputeTardyAmount(line, context);
                                    line.NetTotalHours = ComputeNetTotalHours(line);
                                    line.RegularHours = line.RegularHours / 2;
                                    line.GrossTotalHours = line.NetTotalHours;
                                    line.NetAmount = ComputeNetAmount(line);
                                    line.TardyUndertimeHours = 0;
                                    line.TardyAmount = 0;
                                }
                            }
                        }

                        line.TardyAmount = ComputeTardyAmount(line, context);
                        line.TotalAmount = line.NetAmount;
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

        internal static void QuickChangeLines(TrnDtr dtr, HRISContext context)
        {
            var shiftCodeDays = context.MstShiftCodeDays.ToArray();

            foreach (var line in dtr.TrnDtrlines)
            {
                line.ShiftCodeId = QuickChangeShift(
                    context,
                    line?.TimeIn1 ?? DefaultDate,
                    line?.EmployeeId ?? 0,
                    line?.Date ?? DefaultDate,
                    line?.Dtr.ChangeShiftId ?? 0,
                    line?.ShiftCodeId ?? 0
                );

                if (line is not null)
                {
                    var changeShiftId = line.Dtr.ChangeShiftId ?? 0;
                    var employeeId = line.EmployeeId;
                    var lineDate = line.Date;

                    var changeShiftCodeId =
                        context.TrnChangeShiftLines?.FirstOrDefault(
                            x =>
                                x.ChangeShiftId == changeShiftId
                                && x.EmployeeId == employeeId
                                && x.Date.Date == lineDate
                        )?.ShiftCodeId ?? 0;

                    if (changeShiftCodeId > 0)
                    {
                        var shiftDay = shiftCodeDays.FirstOrDefault(
                            x =>
                                x.ShiftCodeId == changeShiftCodeId
                                && x.Day == line.Date.DayOfWeek.ToString()
                        );

                        line.ShiftCodeId = changeShiftCodeId;

                        if (shiftDay is not null)
                        {
                            if (
                                shiftDay.TimeIn1 is not null
                                && shiftDay.TimeOut1 is null
                                && shiftDay.TimeIn2 is null
                                && shiftDay.TimeOut2 is not null
                            )
                            {
                                shiftDay.TimeIn1 = shiftDay.TimeIn1.HasValue
                                    ? line.Date.Add(shiftDay.TimeIn1.Value.TimeOfDay)
                                    : null;
                                shiftDay.TimeOut1 = null;
                                shiftDay.TimeIn2 = null;
                                shiftDay.TimeOut2 = shiftDay.TimeOut2.HasValue
                                    ? line.Date.Add(shiftDay.TimeOut2.Value.TimeOfDay)
                                    : null;

                                if (shiftDay.TimeOut2 < shiftDay.TimeIn1)
                                    shiftDay.TimeIn1 = shiftDay.TimeIn1?.AddDays(-1);
                                if (shiftDay.TimeOut2 < shiftDay.TimeIn1 && shiftDay.IsTommorow)
                                    shiftDay.TimeOut2 = shiftDay.TimeOut2?.AddDays(1);
                            }
                            else
                            {
                                shiftDay.TimeIn1 = shiftDay.TimeIn1.HasValue
                                    ? line.Date.Add(shiftDay.TimeIn1.Value.TimeOfDay)
                                    : null;
                                shiftDay.TimeOut1 = shiftDay.TimeOut1.HasValue
                                    ? line.Date.Add(shiftDay.TimeOut1.Value.TimeOfDay)
                                    : null;
                                shiftDay.TimeIn2 = shiftDay.TimeIn2.HasValue
                                    ? line.Date.Add(shiftDay.TimeIn2.Value.TimeOfDay)
                                    : null;
                                shiftDay.TimeOut2 = shiftDay.TimeOut2.HasValue
                                    ? line.Date.Add(shiftDay.TimeOut2.Value.TimeOfDay)
                                    : null;

                                // Normal order corrections
                                if (shiftDay.TimeOut1 < shiftDay.TimeIn1)
                                    shiftDay.TimeIn1 = shiftDay.TimeIn1?.AddDays(-1);
                                if (shiftDay.TimeIn2 < shiftDay.TimeOut1)
                                    shiftDay.TimeOut1 = shiftDay.TimeOut1?.AddDays(-1);
                                if (shiftDay.TimeOut2 < shiftDay.TimeIn2)
                                    shiftDay.TimeIn2 = shiftDay.TimeIn2?.AddDays(-1);

                                // IsTomorrow order corrections
                                if (shiftDay.TimeOut1 < shiftDay.TimeIn1)
                                    shiftDay.TimeOut1 = shiftDay.TimeOut1?.AddDays(1);
                                if (shiftDay.TimeIn2 < shiftDay.TimeOut1)
                                    shiftDay.TimeIn2 = shiftDay.TimeIn2?.AddDays(1);
                                if (shiftDay.TimeOut2 < shiftDay.TimeIn2)
                                    shiftDay.TimeOut2 = shiftDay.TimeOut2?.AddDays(1);
                            }

                            line.ShiftDates = string.Join(
                                ",",
                                shiftDay.TimeIn1,
                                shiftDay.TimeOut1,
                                shiftDay.TimeIn2,
                                shiftDay.TimeOut2
                            );
                        }
                    }
                }
            }

            shiftCodeDays = null;
        }

        internal static void QuickEditLines(
            TrnDtr dtr,
            EditDtrLinesByQuickEdit command,
            HRISContext context
        )
        {
            if (command is null || dtr is null || dtr?.TrnDtrlines is null)
                return;

            var listOfEmployeeIds = context.MstEmployees
                .Where(x => x.IsLocked)
                .Select(x => x.Id)
                .ToList();

            if (command.DepartmentId is not null)
                listOfEmployeeIds = context.MstEmployees
                    .Where(x => x.DepartmentId == command.DepartmentId)
                    .Select(x => x.Id)
                    .ToList();

            if (command.EmployeeId is not null)
            {
                listOfEmployeeIds.Clear();
                listOfEmployeeIds.Add(command?.EmployeeId ?? 0);
            }

            foreach (var employeeId in listOfEmployeeIds)
            {
                for (
                    var dtrDate = command.DateStart;
                    dtrDate <= command?.DateEnd;
                    dtrDate = dtrDate.AddDays(1)
                )
                {
                    var line = new TrnDtrline();
                    line.Dtrid = command?.DTRId ?? 0;
                    line.EmployeeId = employeeId;
                    line.Date = dtrDate;
                    line.ShiftCodeId = ComputeShiftCode(null, employeeId, dtrDate, context);
                    line.TimeIn1 =
                        command?.TimeIn1 == null
                            ? null
                            : DateTime.Parse($"{line.Date:MM/dd/yyyy} {command.TimeIn1:hh:mm tt}");
                    line.TimeOut1 =
                        command?.TimeOut1 == null
                            ? null
                            : DateTime.Parse($"{line.Date:MM/dd/yyyy} {command.TimeOut1:hh:mm tt}");
                    line.TimeIn2 =
                        command?.TimeIn2 == null
                            ? null
                            : DateTime.Parse($"{line.Date:MM/dd/yyyy} {command.TimeIn2:hh:mm tt}");
                    line.TimeOut2 =
                        command?.TimeOut2 == null
                            ? null
                            : DateTime.Parse($"{line.Date:MM/dd/yyyy} {command.TimeOut2:hh:mm tt}");

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
                    line.DayTypeId = ComputeDayType(employeeId, dtrDate, context);
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

        public static List<TmpDtrLogs>? ProcessLogsFromDb(
            int? departmentId,
            int? employeeId,
            DateTime startDate,
            DateTime endDate,
            HRISContext context
        )
        {
            var logs = new List<TmpDtrLogs>();
            var empIds = GetEmployeeIds(departmentId, context);

            if (employeeId is not null)
                empIds = new List<int> { employeeId ?? 0 };

            foreach (var empId in empIds)
            {
                var companyId = Common.Lookup.GetCompanyIdByEmployeeId(empId);
                var noField = Common.Lookup.GetDTRNoFieldByCompanyId(companyId);
                var dateTimeField = Common.Lookup.GetDTRDateTimeFieldByCompanyId(companyId);
                var logTypeField = Common.Lookup.GetDTRLogTypeFieldByCompanyId(companyId);
                var bioId = Common.Lookup.GetBioIdByEmployeeId(empId);

                var table = context.TrnLogs.Where(
                    x => x.BiometricIdNumber == bioId && x.LogType != LogTypeExcluded
                );
                if (!table.Any())
                    continue;

                try
                {
                    var filteredLogs = table.Where(
                        row =>
                            row.LogDateTime.HasValue
                            && row.LogDateTime.Value.Date >= startDate.Date
                            && row.LogDateTime.Value.Date <= endDate.Date
                    );

                    foreach (var row in filteredLogs)
                    {
                        logs.Add(
                            new TmpDtrLogs()
                            {
                                EmployeeId = Common.Lookup.GetEmployeeIdByBioId(
                                    row.BiometricIdNumber ?? ""
                                ),
                                Date = DateOnly.FromDateTime(row.LogDateTime ?? DefaultDate),
                                Time = TimeOnly.FromDateTime(row.LogDateTime ?? DefaultDate),
                                LogType = row.LogType?.ToString()
                            }
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }

            return logs;
        }
        #endregion

        #region Helpers
        public static IEnumerable<EmployeeShiftCodeDay.Record> GetEmployeeShiftCodeDays(
            IEnumerable<MstEmployeeShiftCode> employeeShiftCodes,
            IEnumerable<MstShiftCodeDay> shiftCodeDays,
            int employeeId,
            string day,
            DateTime logDateTime,
            string? logType = "I"
        )
        {
            var result = (from sc in employeeShiftCodes
                         join scd in shiftCodeDays on sc.ShiftCodeId equals scd.ShiftCodeId
                         where sc.EmployeeId == employeeId && scd.Day == day
                         select new EmployeeShiftCodeDay.Record
                         {
                             EmployeeId = sc.EmployeeId,
                             ShiftCodeId = scd.ShiftCodeId,
                             Day = scd.Day,
                             TimeIn1 = scd.TimeIn1 ?? DateTime.MinValue,
                             TimeOut2 = scd.TimeOut2 ?? DateTime.MinValue,
                             LogTimeIn1 = logDateTime
                         }).ToList();

            foreach (var item in result)
            {
                item.TimeIn1 = DateTime.Parse($"{logDateTime.Date:d} {item.TimeIn1:hh:mm tt}");
                item.TimeOut2 = DateTime.Parse($"{logDateTime.Date:d} {item.TimeOut2:hh:mm tt}");

                var discrepancy = (item.TimeIn1 - item.LogTimeIn1).TotalHours;
                if (discrepancy < -20) item.TimeIn1 = item.TimeIn1.AddDays(1);
                if (discrepancy > 20) item.TimeIn1 = item.TimeIn1.AddDays(-1);

                item.Interval = Math.Abs((item.TimeIn1 - item.LogTimeIn1).TotalHours);

                if (logType == "O")
                {
                    discrepancy = (item.TimeOut2 - item.LogTimeIn1).TotalHours;
                    if (discrepancy < -20) item.TimeOut2 = item.TimeOut2.AddDays(1);
                    if (discrepancy > 20) item.TimeOut2 = item.TimeOut2.AddDays(-1);
                    item.Interval = Math.Abs((item.TimeOut2 - item.LogTimeIn1).TotalHours);
                }
            }

            return result;
        }
        #endregion
    }
}
