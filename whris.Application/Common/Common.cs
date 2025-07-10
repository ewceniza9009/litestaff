using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Application.Queries.TrnPayrollOtherDeduction;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Common
{
    public class Common
    {
        /// <summary>
        /// This private helper is the core of the fix. It creates and disposes the DbContext
        /// for every operation, preventing any resource leaks while keeping the public method
        /// signatures unchanged for legacy compatibility.
        /// </summary>
        /// <typeparam name="TResult">The return type of the operation.</typeparam>
        /// <param name="databaseOperation">The database logic to execute.</param>
        /// <returns>The result of the database operation.</returns>
        private static TResult ExecuteWithContext<TResult>(Func<HRISContext, TResult> databaseOperation)
        {
            // The 'using' declaration guarantees disposal, fixing the leak.
            using var context = new HRISContext();

            // Execute the database logic that was passed in using the new, safe context.
            return databaseOperation(context);
        }

        private static async Task<TResult> ExecuteWithContextAsync<TResult>(Func<HRISContext, Task<TResult>> databaseOperation)
        {
            // The 'using' declaration works seamlessly with async.
            using var context = new HRISContext();

            // Await the async database logic that was passed in.
            return await databaseOperation(context);
        }

        public static JsonResult GetDepartmentList()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstDepartments
                .OrderBy(x => x.Department)
                .Select(x => new MstDepartmentDto()
                {
                    Id = x.Id,
                    Department = x.Department
                })
                .ToList()));
        }

        public static JsonResult GetZipCodes()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstZipCodes
                .OrderBy(x => x.ZipCode)
                .Select(x => new MstZipCode()
                {
                    Id = x.Id,
                    ZipCode = x.ZipCode
                })
                .ToList()));
        }

        public static JsonResult GetCitizenships()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstCitizenships
                .OrderBy(x => x.Citizenship)
                .Select(x => new MstCitizenship()
                {
                    Id = x.Id,
                    Citizenship = x.Citizenship
                })
                .ToList()));
        }

        public static JsonResult GetReligions()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstReligions
                .OrderBy(x => x.Religion)
                .Select(x => new MstReligion()
                {
                    Id = x.Id,
                    Religion = x.Religion
                })
                .ToList()));
        }

        public static JsonResult GetTaxCodes()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstTaxCodes
                .OrderBy(x => x.TaxCode)
                .Select(x => new MstTaxCode()
                {
                    Id = x.Id,
                    TaxCode = x.TaxCode
                })
                .ToList()));
        }

        public static JsonResult GetCompanies()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstCompanies
                .OrderBy(x => x.Company)
                .Select(x => new MstCompany()
                {
                    Id = x.Id,
                    Company = x.Company
                })
                .ToList()));
        }

        public static JsonResult GetBranches()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstBranches
                .OrderBy(x => x.Branch)
                .Select(x => new MstBranch()
                {
                    Id = x.Id,
                    Branch = x.Branch
                })
                .ToList()));
        }

        public static JsonResult GetDepartments()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstDepartments
                .OrderBy(x => x.Department)
                .Select(x => new MstDepartment()
                {
                    Id = x.Id,
                    Department = x.Department
                })
                .ToList()));
        }

        public static JsonResult GetPositions()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstPositions
                .OrderBy(x => x.Position)
                .Select(x => new MstPosition()
                {
                    Id = x.Id,
                    Position = x.Position
                })
                .ToList()));
        }

        public static JsonResult GetDivisions()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstDivisions
                .OrderBy(x => x.Division)
                .Select(x => new MstDivision()
                {
                    Id = x.Id,
                    Division = x.Division
                })
                .ToList()));
        }

        public static JsonResult GetPayrollGroups()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstPayrollGroups
                .OrderBy(x => x.PayrollGroup)
                .Select(x => new MstPayrollGroup()
                {
                    Id = x.Id,
                    PayrollGroup = x.PayrollGroup
                })
                .ToList()));
        }

        public static JsonResult GetPayrollTypes()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstPayrollTypes
                .OrderBy(x => x.PayrollType)
                .Select(x => new MstPayrollType()
                {
                    Id = x.Id,
                    PayrollType = x.PayrollType
                })
                .ToList()));
        }

        public static JsonResult GetGLAccounts()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstAccounts
                .OrderBy(x => x.Account)
                .Select(x => new MstAccount()
                {
                    Id = x.Id,
                    Account = x.Account
                })
                .ToList()));
        }

        public static JsonResult GetShiftCodes()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstShiftCodes
                .Where(x => x.IsLocked)
                .OrderBy(x => x.ShiftCode)
                .Select(x => new MstShiftCode()
                {
                    Id = x.Id,
                    ShiftCode = x.ShiftCode
                })
                .ToList()));
        }

        public static JsonResult GetDayTypes()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstDayTypes
                .Where(x => x.IsLocked)
                .OrderBy(x => x.DayType)
                .Select(x => new MstDayType()
                {
                    Id = x.Id,
                    DayType = x.DayType
                })
                .ToList()));
        }

        public static JsonResult GetUsers()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstUsers
                .Where(x => x.IsLocked)
                .OrderBy(x => x.FullName)
                .Select(x => new MstUser()
                {
                    Id = x.Id,
                    FullName = x.FullName
                })
                .ToList()));
        }

        public static JsonResult GetAspUsers()
        {
            return ExecuteWithContext(context => new JsonResult(context.AspNetUsers
                .OrderBy(x => x.Email)
                .Select(x => new AspNetUser()
                {
                    Id = x.Id,
                    Email = x.Email
                })
                .ToList()));
        }

        public static JsonResult GetForms()
        {
            return ExecuteWithContext(context => new JsonResult(context.SysForms
                .OrderBy(x => x.Remarks)
                .Select(x => new SysForm()
                {
                    Id = x.Id,
                    Remarks = x.Remarks
                })
                .ToList()));
        }

        public static JsonResult GetPeriods()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstPeriods
                .OrderByDescending(x => x.Period)
                .Select(x => new MstPeriod()
                {
                    Id = x.Id,
                    Period = x.Period
                })
                .ToList()));
        }

        public static JsonResult GetChangeShifts()
        {
            return ExecuteWithContext(context => new JsonResult(context.TrnChangeShifts
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Csnumber)
                .Select(x => new TrnChangeShift()
                {
                    Id = x.Id,
                    Csnumber = $"{x.Csnumber} - {x.Remarks}"
                })
                .ToList()));
        }

        public static JsonResult GetOvertimes()
        {
            return ExecuteWithContext(context => new JsonResult(context.TrnOverTimes
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Otnumber)
                .Select(x => new TrnOverTime()
                {
                    Id = x.Id,
                    Otnumber = x.Otnumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList()));
        }

        public static JsonResult GetLeaves()
        {
            return ExecuteWithContext(context => new JsonResult(context.TrnLeaveApplications
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Lanumber)
                .Select(x => new TrnLeaveApplication()
                {
                    Id = x.Id,
                    Lanumber = x.Lanumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList()));
        }

        // CONSOLIDATED GetEmployees and GetAllEmployees into one method
        public static JsonResult GetEmployees(int? payrollGroupId = null, bool includeAll = false)
        {
            return ExecuteWithContext(context =>
            {
                var query = context.MstEmployees.AsQueryable();

                if (!includeAll)
                {
                    query = query.Where(x => x.IsLocked);
                }

                if (payrollGroupId is not null)
                {
                    query = query.Where(x => x.PayrollGroupId == payrollGroupId);
                }

                var data = query
                    .OrderBy(x => x.FullName)
                    .Select(x => new MstEmployeeDto()
                    {
                        Id = x.Id,
                        FullName = x.FullName,
                        OvertimeHourlyRate = x.OvertimeHourlyRate,
                        DepartmentId = x.DepartmentId,
                        PayrollGroupId = x.PayrollGroupId
                    })
                    .ToList();

                return new JsonResult(data);
            });
        }

        // This method is now redundant due to the new `includeAll` flag in GetEmployees
        // but is kept for backward compatibility if you prefer. Or you can delete it.
        public static JsonResult GetAllEmployees(int? payrollGroupId = null)
        {
            return GetEmployees(payrollGroupId, true);
        }

        public static JsonResult GetSetupLeaves()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstLeaves
                .OrderBy(x => x.Leave)
                .Select(x => new MstLeave()
                {
                    Id = x.Id,
                    Leave = x.Leave
                })
                .ToList()));
        }

        public static JsonResult GetDeductions()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstOtherDeductions
                .OrderBy(x => x.OtherDeduction)
                .Select(x => new MstOtherDeduction()
                {
                    Id = x.Id,
                    OtherDeduction = x.OtherDeduction
                })
                .ToList()));
        }

        public static JsonResult GetLoans(int? employeeId = null)
        {
            var loans = new EmployeeLoanList()
            {
                EmployeeId = employeeId
            };

            return new JsonResult(loans.Result());
        }

        public static JsonResult GetIncomes()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstOtherIncomes
                .OrderBy(x => x.OtherIncome)
                .Select(x => new MstOtherIncome()
                {
                    Id = x.Id,
                    OtherIncome = x.OtherIncome
                })
                .ToList()));
        }

        public static JsonResult GetDeductionLoans()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstOtherDeductions
                .OrderBy(x => x.OtherDeduction)
                .Where(x => x.LoanType)
                .Select(x => new MstOtherDeduction()
                {
                    Id = x.Id,
                    OtherDeduction = x.OtherDeduction
                })
                .ToList()));
        }

        public static JsonResult GetMonths()
        {
            return ExecuteWithContext(context => new JsonResult(context.MstMonths
                .Select(x => new MstMonth()
                {
                    Id = x.Id,
                    Month = x.Month
                })
                .ToList()));
        }

        public static JsonResult GetDTRs()
        {
            return ExecuteWithContext(context => new JsonResult(context.TrnDtrs
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Dtrnumber)
                .Select(x => new TrnDtr()
                {
                    Id = x.Id,
                    Dtrnumber = x.Dtrnumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList()));
        }

        public static JsonResult GetPayrollOtherIncomes()
        {
            return ExecuteWithContext(context => new JsonResult(context.TrnPayrollOtherIncomes
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Poinumber)
                .Select(x => new TrnPayrollOtherIncome()
                {
                    Id = x.Id,
                    Poinumber = x.Poinumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList()));
        }

        public static JsonResult GetPayrollOtherDeductions()
        {
            return ExecuteWithContext(context => new JsonResult(context.TrnPayrollOtherDeductions
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Podnumber)
                .Select(x => new TrnPayrollOtherDeduction()
                {
                    Id = x.Id,
                    Podnumber = x.Podnumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList()));
        }

        public static JsonResult GetLastWithholdingTaxes()
        {
            return ExecuteWithContext(context => new JsonResult(context.TrnLastWithholdingTaxes
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Lwtnumber)
                .Select(x => new TrnLastWithholdingTax()
                {
                    Id = x.Id,
                    Lwtnumber = x.Lwtnumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList()));
        }

        public static JsonResult GetPayrollNumbers(int? payrollGroupId = null)
        {
            return ExecuteWithContext(context =>
            {
                var query = context.TrnPayrolls.Where(x => x.IsLocked);

                if (payrollGroupId is not null)
                {
                    query = query.Where(x => x.PayrollGroupId == payrollGroupId);
                }

                var data = query
                    .OrderByDescending(x => x.PayrollNumber)
                    .Select(x => new TrnPayrollDto()
                    {
                        Id = x.Id,
                        PayrollNumber = x.PayrollNumber + " - " + x.Remarks
                    })
                    .ToList();

                return new JsonResult(data);
            });
        }

        public static JsonResult GetPayrollNumbersWithRemarks()
        {
            return ExecuteWithContext(context => new JsonResult(context.TrnPayrolls
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.PayrollNumber)
                .Select(x => new TrnPayrollDto()
                {
                    Id = x.Id,
                    Remarks = x.PayrollNumber + " - " + x.Remarks
                })
                .ToList()));
        }

        public static async Task<List<TrnPayrollDto>> GetPayrollNumbers2Async(string Code)
        {
            return await ExecuteWithContextAsync(async context =>
            {
                var payrollGroupId = await MobileUtils.GetPayrollGroupIdAsync(Code);

                return await context.TrnPayrolls
                    .Where(x => x.IsLocked && x.IsApproved && x.PayrollGroupId == payrollGroupId)
                    .OrderByDescending(x => x.PayrollNumber)
                    .Select(x => new TrnPayrollDto()
                    {
                        Id = x.Id,
                        PayrollNumber = x.Remarks ?? "NA"
                    })
                    .ToListAsync(); // Use ToListAsync for the database query
            });
        }

        public static JsonResult GetDTRs2(string Code)
        {
            return ExecuteWithContext(context =>
            {
                var payrollGroupId = MobileUtils.GetPayrollGroupId(Code);

                var data = context.TrnDtrs
                    .Where(x => x.IsLocked && x.IsApproved && x.PayrollGroupId == payrollGroupId)
                    .OrderByDescending(x => x.Dtrnumber)
                    .Select(x => new TrnDtr()
                    {
                        Id = x.Id,
                        Dtrnumber = x.Dtrnumber + " - " + x.Remarks
                    })
                    .ToList();

                return new JsonResult(data);
            });
        }

        public static async Task<JsonResult> GetDTRs2Async(string Code)
        {
            // Assumes an async version of your helper method now exists
            return await ExecuteWithContextAsync(async context =>
            {
                // Assumes an async version of this utility now exists
                var payrollGroupId = await MobileUtils.GetPayrollGroupIdAsync(Code);

                var data = await context.TrnDtrs
                    .Where(x => x.IsLocked && x.IsApproved && x.PayrollGroupId == payrollGroupId)
                    .OrderByDescending(x => x.Dtrnumber)
                    .Select(x => new TrnDtr()
                    {
                        Id = x.Id,
                        Dtrnumber = x.Dtrnumber + " - " + x.Remarks
                    })
                    .ToListAsync(); // Use ToListAsync for the database query

                return new JsonResult(data);
            });
        }

        public static List<MstEmployeeDto> GetEmployeesWithSalary()
        {
            return ExecuteWithContext(context =>
            {
                // IMPORTANT: This fixes the DbContext leak but does NOT fix the
                // underlying performance issue of N+1 queries from the Lookup calls.
                // For a true performance fix, those Lookup calls should be replaced
                // with database joins (e.g., using .Include() in EF Core).
                return context.MstEmployees
                    .OrderBy(x => x.FullName)
                    .Select(x => new MstEmployeeDto()
                    {
                        Id = x.Id,
                        BiometricIdNumber = x.BiometricIdNumber,
                        FullName = x.FullName,
                        CellphoneNumber = x.CellphoneNumber,
                        EmailAddress = x.EmailAddress,
                        DepartmentId = x.DepartmentId,
                        DepartmentName = x.DepartmentName,
                        IsLocked = x.IsLocked,
                        Gsisnumber = x.Gsisnumber,
                        Sssnumber = x.Sssnumber,
                        Hdmfnumber = x.Hdmfnumber,
                        Phicnumber = x.Phicnumber,
                        Tin = x.Tin,
                        AtmaccountNumber = x.AtmaccountNumber,
                        Company = Lookup.GetCompanyNameById(x.CompanyId),
                        Branch = Lookup.GetBranchNameById(x.BranchId),
                        Department = Lookup.GetDepartmentNameById(x.DepartmentId),
                        Position = Lookup.GetPositionNameById(x.PositionId),
                        MonthlyRate = x.MonthlyRate,
                        DailyRate = x.DailyRate,
                        Allowance = x.Allowance,
                        LeaveBalance = x.LeaveBalance ?? 0,
                    })
                    .ToList();
            });
        }
    }
}