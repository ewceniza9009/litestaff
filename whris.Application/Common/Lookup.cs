using whris.Data.Data;

namespace whris.Application.Common
{
    public class Lookup
    {
        static HRISContext HRISContext
        {
            get => new HRISContext();
        }

        public static string? WebRootPathPhoto { get; set; }

        public static int GetUserIdByAspUserId(string Id) => HRISContext.MstUsers?.FirstOrDefault(x => x.ASPUserId == Id)?.Id ?? 0;
        public static int GetDefaultEmployeeShiftCode() => HRISContext.MstShiftCodes.FirstOrDefault()?.Id ?? 0;
        public static string GetFormNameById(int Id) => HRISContext.SysForms.Find(Id)?.Remarks ?? "NA";
        public static string GetEmployeeNameById(int Id) => HRISContext.MstEmployees.Find(Id)?.FullName ?? "NA";
        public static decimal GetEmployeeBasic(int Id) => HRISContext.MstEmployees.Find(Id)?.PayrollRate ?? 0;
        public static string GetEmployeeShiftById(int ShiftId) => HRISContext.MstShiftCodes.Find(ShiftId)?.ShiftCode ?? "NA";
        public static decimal GetEmployeeOvertimeRateById(int Id) => HRISContext.MstEmployees.Find(Id)?.OvertimeHourlyRate ?? 0m;
        public static string GetPayrollTypeById(int Id) => HRISContext.MstPayrollTypes.Find(Id)?.PayrollType ?? "NA";
        public static int GetCompanyIdByEmployeeId(int Id) => HRISContext.MstEmployees.Find(Id)?.CompanyId ?? 0;
        public static int GetEmployeeIdByBioId(string BId) => HRISContext.MstEmployees.FirstOrDefault(x => x.BiometricIdNumber == BId)?.Id ?? 0;
        public static int GetEmploymentTypeByEmployeeId(int employeeId) => HRISContext.MstEmployees.FirstOrDefault(x => x.Id == employeeId)?.EmploymentType ?? 1;
        public static int GetPayrollGroupIdByEmployeeId(int employeeId) => HRISContext.MstEmployees.FirstOrDefault(x => x.Id == employeeId)?.PayrollGroupId ?? 1;
        public static int GetLeaveIdByDescription(string Desc) => HRISContext.MstLeaves.FirstOrDefault(x => x.Leave.ToUpper().Trim() == Desc.ToUpper().Trim())?.Id ?? 0;
        public static int GetOtherIncomeIdByDescription(string Desc) => HRISContext.MstOtherIncomes.FirstOrDefault(x => x.OtherIncome.ToUpper().Trim() == Desc.ToUpper().Trim())?.Id ?? 0;
        public static int GetOtherDeductionIdByDescription(string Desc) => HRISContext.MstOtherDeductions.FirstOrDefault(x => x.OtherDeduction.ToUpper().Trim() == Desc.ToUpper().Trim())?.Id ?? 0;
        public static string GetBioIdByEmployeeId(int EId) => HRISContext.MstEmployees.FirstOrDefault(x => x.Id == EId)?.BiometricIdNumber ?? "";
        public static string GetLoanDateTextByLoanId(int LoanId) => HRISContext.MstEmployeeLoans.FirstOrDefault(x => x.Id == LoanId)?.DateStart.ToString("MM/dd/yyyy") ?? "";
        public static Data.Models.TrnPayroll GetPayrollById(int Id) => HRISContext.TrnPayrolls.FirstOrDefault(x => x.Id == Id) ?? new Data.Models.TrnPayroll();
        public static string GetPayrollNoById(int Id) => HRISContext.TrnPayrolls.FirstOrDefault(x => x.Id == Id)?.PayrollNumber ?? "NA";
        public static string GetPayrollOINumberById(int Id) => HRISContext.TrnPayrollOtherIncomes.FirstOrDefault(x => x.Id == Id)?.Poinumber ?? "NA";
        public static decimal GetLoanTotalAmountByLoanId(int Id) => HRISContext.MstEmployeeLoans.FirstOrDefault(x => x.Id == Id)?.TotalPayment ?? 0m;
        public static decimal GetLoanBalanceByLoanId(int Id) => HRISContext.MstEmployeeLoans.FirstOrDefault(x => x.Id == Id)?.Balance ?? 0m;
        public static string GetDTRNoFieldByCompanyId(int compId) => HRISContext.MstCompanies.FirstOrDefault(x => x.Id == compId)?.DtrnoField ?? "No";
        public static string GetDTRDateTimeFieldByCompanyId(int compId) => HRISContext.MstCompanies.FirstOrDefault(x => x.Id == compId)?.DtrdateTimeField ?? "Date/Time";
        public static string GetDTRLogTypeFieldByCompanyId(int compId) => HRISContext.MstCompanies.FirstOrDefault(x => x.Id == compId)?.DtrlogTypeField ?? "LogType";
        public static string GetOldPictureFilePathByEmployeeId(int Id) => HRISContext.MstEmployees.FirstOrDefault(x => x.Id == Id)?.OldPictureFilePath ?? "NA";
        public static string GetOldImageLogoPathByCompanyId(int Id) => HRISContext.MstCompanies.FirstOrDefault(x => x.Id == Id)?.OldImageLogo ?? "NA";
        public static string GetCompletePictureFilePathByEmployeeId(int Id) => $@"{ WebRootPathPhoto }{ HRISContext.MstEmployees.FirstOrDefault(x => x.Id == Id)?.PictureFilePath ?? "NA" }";
        public static string GetDTRNumberById(int Id) => HRISContext.TrnDtrs.FirstOrDefault(x => x.Id == Id)?.Dtrnumber ?? "NA";
        public static string GetDTRRemarksById(int Id) => HRISContext.TrnDtrs.FirstOrDefault(x => x.Id == Id)?.Remarks ?? "NA";
        public static string GetCompanyNameById(int Id) => HRISContext.MstCompanies.FirstOrDefault(x => x.Id == Id)?.Company ?? "";
        public static string GetBranchNameById(int Id) => HRISContext.MstBranches.FirstOrDefault(x => x.Id == Id)?.Branch ?? "";
        public static string GetDepartmentNameById(int Id) => HRISContext.MstDepartments.FirstOrDefault(x => x.Id == Id)?.Department ?? "";
        public static string GetPositionNameById(int Id) => HRISContext.MstPositions.FirstOrDefault(x => x.Id == Id)?.Position ?? "";

        public static string? GetBranchNameByEmployeeId(int employeeId)
        {
            var branchId = HRISContext.MstEmployees.FirstOrDefault(x => x.Id == employeeId)?.BranchId ?? 0;
            return HRISContext.MstBranches.FirstOrDefault(x => x.Id == branchId)?.Branch ?? "";
        }
    }
}
