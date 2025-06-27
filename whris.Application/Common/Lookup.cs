using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.Common
{
    public class Lookup
    {
        /// <summary>
        /// This private helper is the core of the fix. It creates and disposes the DbContext
        /// for every operation, preventing any resource leaks.
        /// </summary>
        private static TResult ExecuteWithContext<TResult>(Func<HRISContext, TResult> databaseOperation)
        {
            using var context = new HRISContext();
            // We can disable change tracking for read-only lookups to improve performance.
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return databaseOperation(context);
        }

        // WARNING: This is a global static property. It should be set only once
        // during application startup to avoid unpredictable behavior.
        public static string? WebRootPathPhoto { get; set; }

        public static int GetUserIdByAspUserId(string Id) => ExecuteWithContext(context => context.MstUsers.FirstOrDefault(x => x.ASPUserId == Id)?.Id ?? 0);

        public static int GetDefaultEmployeeShiftCode() => ExecuteWithContext(context => context.MstShiftCodes.FirstOrDefault()?.Id ?? 0);

        // Using .Find() is slightly more efficient for primary key lookups.
        public static string GetFormNameById(int Id) => ExecuteWithContext(context => context.SysForms.Find(Id)?.Remarks ?? "NA");

        public static string GetEmployeeNameById(int Id) => ExecuteWithContext(context => context.MstEmployees.Find(Id)?.FullName ?? "NA");

        public static decimal GetEmployeeBasic(int Id) => ExecuteWithContext(context => context.MstEmployees.Find(Id)?.PayrollRate ?? 0);

        public static string GetEmployeeShiftById(int ShiftId) => ExecuteWithContext(context => context.MstShiftCodes.Find(ShiftId)?.ShiftCode ?? "NA");

        public static decimal GetEmployeeOvertimeRateById(int Id) => ExecuteWithContext(context => context.MstEmployees.Find(Id)?.OvertimeHourlyRate ?? 0m);

        public static string GetPayrollTypeById(int Id) => ExecuteWithContext(context => context.MstPayrollTypes.Find(Id)?.PayrollType ?? "NA");

        public static int GetCompanyIdByEmployeeId(int Id) => ExecuteWithContext(context => context.MstEmployees.Find(Id)?.CompanyId ?? 0);

        public static int GetEmployeeIdByBioId(string BId) => ExecuteWithContext(context => context.MstEmployees.FirstOrDefault(x => x.BiometricIdNumber == BId)?.Id ?? 0);

        public static int GetEmploymentTypeByEmployeeId(int employeeId) => ExecuteWithContext(context => context.MstEmployees.Find(employeeId)?.EmploymentType ?? 1);

        public static int GetPayrollGroupIdByEmployeeId(int employeeId) => ExecuteWithContext(context => context.MstEmployees.Find(employeeId)?.PayrollGroupId ?? 1);

        public static int GetLeaveIdByDescription(string Desc) => ExecuteWithContext(context => context.MstLeaves.FirstOrDefault(x => x.Leave.ToUpper().Trim() == Desc.ToUpper().Trim())?.Id ?? 0);

        public static int GetOtherIncomeIdByDescription(string Desc) => ExecuteWithContext(context => context.MstOtherIncomes.FirstOrDefault(x => x.OtherIncome.ToUpper().Trim() == Desc.ToUpper().Trim())?.Id ?? 0);

        public static int GetOtherDeductionIdByDescription(string Desc) => ExecuteWithContext(context => context.MstOtherDeductions.FirstOrDefault(x => x.OtherDeduction.ToUpper().Trim() == Desc.ToUpper().Trim())?.Id ?? 0);

        public static string GetBioIdByEmployeeId(int EId) => ExecuteWithContext(context => context.MstEmployees.Find(EId)?.BiometricIdNumber ?? "");

        public static string GetLoanDateTextByLoanId(int LoanId) => ExecuteWithContext(context => context.MstEmployeeLoans.Find(LoanId)?.DateStart.ToString("MM/dd/yyyy") ?? "");

        public static Data.Models.TrnPayroll GetPayrollById(int Id) => ExecuteWithContext(context => context.TrnPayrolls.Find(Id) ?? new Data.Models.TrnPayroll());

        public static string GetPayrollNoById(int Id) => ExecuteWithContext(context => context.TrnPayrolls.Find(Id)?.PayrollNumber ?? "NA");

        public static string GetPayrollOINumberById(int Id) => ExecuteWithContext(context => context.TrnPayrollOtherIncomes.Find(Id)?.Poinumber ?? "NA");

        public static decimal GetLoanTotalAmountByLoanId(int Id) => ExecuteWithContext(context => context.MstEmployeeLoans.Find(Id)?.TotalPayment ?? 0m);

        public static decimal GetLoanBalanceByLoanId(int Id) => ExecuteWithContext(context => context.MstEmployeeLoans.Find(Id)?.Balance ?? 0m);

        public static string GetDTRNoFieldByCompanyId(int compId) => ExecuteWithContext(context => context.MstCompanies.Find(compId)?.DtrnoField ?? "No");

        public static string GetDTRDateTimeFieldByCompanyId(int compId) => ExecuteWithContext(context => context.MstCompanies.Find(compId)?.DtrdateTimeField ?? "Date/Time");

        public static string GetDTRLogTypeFieldByCompanyId(int compId) => ExecuteWithContext(context => context.MstCompanies.Find(compId)?.DtrlogTypeField ?? "LogType");

        public static string GetOldPictureFilePathByEmployeeId(int Id) => ExecuteWithContext(context => context.MstEmployees.Find(Id)?.OldPictureFilePath ?? "NA");

        public static string GetOldImageLogoPathByCompanyId(int Id) => ExecuteWithContext(context => context.MstCompanies.Find(Id)?.OldImageLogo ?? "NA");

        public static string GetCompletePictureFilePathByEmployeeId(int Id)
        {
            // This now uses the safe context wrapper for its single database call.
            var picturePath = ExecuteWithContext(context => context.MstEmployees.Find(Id)?.PictureFilePath ?? "NA");
            return $@"{WebRootPathPhoto}{picturePath}";
        }

        public static string GetDTRNumberById(int Id) => ExecuteWithContext(context => context.TrnDtrs.Find(Id)?.Dtrnumber ?? "NA");

        public static string GetDTRRemarksById(int Id) => ExecuteWithContext(context => context.TrnDtrs.Find(Id)?.Remarks ?? "NA");

        public static string GetCompanyNameById(int Id) => ExecuteWithContext(context => context.MstCompanies.Find(Id)?.Company ?? "");

        public static string GetBranchNameById(int Id) => ExecuteWithContext(context => context.MstBranches.Find(Id)?.Branch ?? "");

        public static string GetDepartmentNameById(int Id) => ExecuteWithContext(context => context.MstDepartments.Find(Id)?.Department ?? "");

        public static string GetPositionNameById(int Id) => ExecuteWithContext(context => context.MstPositions.Find(Id)?.Position ?? "");

        /// <summary>
        /// REFACTORED for performance. This now uses a single, efficient database query
        /// instead of two separate ones.
        /// </summary>
        public static string? GetBranchNameByEmployeeId(int employeeId)
        {
            return ExecuteWithContext(context =>
                context.MstEmployees
                    .Where(e => e.Id == employeeId)
                    .Select(e => e.Branch.Branch) // Uses the navigation property to create a JOIN
                    .FirstOrDefault() ?? ""
            );
        }
    }
}