using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Models;

namespace whris.UI.Services.Datasources
{
    public class TrnDtrComboboxDatasources
    {
        public List<MstPayrollGroup> PayrollGroupCmbDs => (List<MstPayrollGroup>)(Common.GetPayrollGroups()?.Value ?? new List<MstPayrollGroup>());
        public List<MstPeriod> PeriodCmbDs => (List<MstPeriod>)(Common.GetPeriods()?.Value ?? new List<MstPeriod>());
        public List<TrnChangeShift> ChangeShiftCmbDs => (List<TrnChangeShift>)(Common.GetChangeShifts()?.Value ?? new List<TrnChangeShift>());
        public List<TrnOverTime> OvertimeCmbDs = (List<TrnOverTime>)(Common.GetOvertimes()?.Value ?? new List<TrnOverTime>());
        public List<TrnLeaveApplication> LeaveCmbDs = (List<TrnLeaveApplication>)(Common.GetLeaves()?.Value ?? new List<TrnLeaveApplication>());
        public List<MstUser> UserCmbDs => (List<MstUser>)(Common.GetUsers()?.Value ?? new List<MstUser>());
        public List<MstDepartment> DepartmentCmbDs => (List<MstDepartment>)(Common.GetDepartments()?.Value ?? new List<MstDepartment>());
        public List<MstEmployeeDto> EmployeeCmbDs = (List<MstEmployeeDto>)(Common.GetEmployees()?.Value ?? new List<MstEmployeeDto>());
        public List<MstEmployeeDto> AllEmployeeCmbDs = (List<MstEmployeeDto>)(Common.GetAllEmployees()?.Value ?? new List<MstEmployeeDto>());
        public List<MstLeave> LeaveSetupCmbDs => (List<MstLeave>)(Common.GetSetupLeaves()?.Value ?? new List<MstLeave>());
        public List<MstOtherDeduction> DeductionLoanCmbDs => (List<MstOtherDeduction>)(Common.GetDeductionLoans()?.Value ?? new List<MstOtherDeduction>());
        public List<MstShiftCode> ShiftCodeCmbDs => (List<MstShiftCode>)(Common.GetShiftCodes()?.Value ?? new List<MstShiftCode>());
        public List<MstOtherIncome> OtherIncomeCmbDs => (List<MstOtherIncome>)(Common.GetIncomes()?.Value ?? new List<MstOtherIncome>());
        public List<TrnPayrollDto> PayrollNoWithRemarksCmbDs => (List<TrnPayrollDto>)(Common.GetPayrollNumbersWithRemarks()?.Value ?? new List<TrnPayrollDto>());
        public List<LoanNo> LoanNumbers => new List<LoanNo> 
        {
            new LoanNo(0),
            new LoanNo(1),
            new LoanNo(2),
            new LoanNo(3),
            new LoanNo(4)           
        };

        public static TrnDtrComboboxDatasources Instance => new TrnDtrComboboxDatasources();

        private TrnDtrComboboxDatasources()
        {

        }

        public class LoanNo
        {
            public LoanNo(int loanNoName)
            {
                LoanNoName = loanNoName;
            }

            public int LoanNoName { get; set; }
        }
    }
}
