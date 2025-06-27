using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Application.Common;
using whris.Application.CQRS.MstCompany.Queries;
using whris.Application.Library;

namespace whris.Application.Dtos
{
    public class MstEmployeeDto
    {
        public int Id { get; set; }

        public string? MobileCode => MobileUtils.Encode(Id.ToString());
        public string? MobileCodeHidden
        {
            get 
            {
                var mobileCode = MobileUtils.Encode(Id.ToString());
                var result = "****" + mobileCode.Substring(mobileCode.Length - 2, 2);

                return result;
            }
        }

        public string BiometricIdNumber { get; set; } = null!;

        public string? ExtensionName { get; set; }

        public string FullName { get; set; } = null!;

        public decimal OvertimeHourlyRate { get; set; }

        public string? CellphoneNumber { get; set; }

        public string? EmailAddress { get; set; }

        [NotMapped]
        public string Status => IsLocked ? "Active" : "In-Active";

        public bool IsLocked { get; set; }

        public string? Gsisnumber { get; set; }
        public string? Sssnumber { get; set; }
        public string? Hdmfnumber { get; set; }
        public string? Phicnumber { get; set; }
        public string? Tin { get; set; }
        public string? AtmaccountNumber { get; set; }
        public string? Company { get; set; }
        public string? Branch { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public decimal MonthlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? NewDailyRate { get; set; }
        public decimal? NewAllowance { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public string? BranchName { get; set; }

        public int PayrollGroupId { get; set; }

        public string? ForApprove
        {
            get 
            {
                var forApproveShiftCode = false;
                var forApproveNewSalary = false;
                var forApproveNewAllowance = false;

                var listMessage = new List<string>();
                var result = string.Empty;

                var empShiftCodeService = new EmployeeShiftCodeService();
                var getEmployeeShifts = empShiftCodeService.GetByEmployeeIdAsync(Id).Result;

                forApproveShiftCode = getEmployeeShifts.Any(x => x.Status == "New" || x.Status == "Modified" || x.Status == "Deleted");
                forApproveNewSalary = (NewDailyRate ?? 0) > 0;
                forApproveNewAllowance = (NewAllowance ?? 0) > 0; 

                if (forApproveShiftCode) 
                {
                    listMessage.Add("Shift");
                }

                if (forApproveNewSalary)
                {
                    listMessage.Add("Salary");
                }

                if (forApproveNewAllowance)
                {
                    listMessage.Add("Allowance");
                }

                if (listMessage.Count > 0) 
                {
                    result = string.Join(", ", listMessage);
                }

                return listMessage.Count == 0 ? "Verified" : result;
            }
        }

        public decimal LeaveBalance { get; set; }

        public string? Search => $"{FullName} {BiometricIdNumber} {EmailAddress}";
    }
}
