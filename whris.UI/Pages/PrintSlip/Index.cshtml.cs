using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.UI.Authorization;
using whris.UI.Services;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.PrintSlip
{
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public MstEmployeeComboboxDatasources ComboboxDatasources = MstEmployeeComboboxDatasources.Instance;
        public List<TrnPayrollDto> PayrollNumbers = new List<TrnPayrollDto>();
        public List<MstEmploymentTypeDto> EmploymentTypeCmbDs => new List<MstEmploymentTypeDto>()
        {
            new MstEmploymentTypeDto() { Id = 1, EmploymentType = "Regular" },
            new MstEmploymentTypeDto() { Id = 2, EmploymentType = "Probationary"},
            new MstEmploymentTypeDto() { Id = 3, EmploymentType = "Newly Hired"}
        };
        public List<MstEmployeeDto> Employees => (List<MstEmployeeDto>)(Common.GetEmployees()?.Value ?? new List<MstEmployeeDto>());

        public int EmployeeId = 0;
        public int EmploymentType = 0;

        public void OnGet(string key)
        {
            var mobileCode = EncryptionHelper.Decrypt(key);
            var employeeId = MobileUtils.GetEmployeeByMobileCode(mobileCode);
            var employeeType = Lookup.GetEmploymentTypeByEmployeeId(employeeId);
            var payrollGroupId = Lookup.GetPayrollGroupIdByEmployeeId(employeeId);

            PayrollNumbers = (List<TrnPayrollDto>)(Common.GetPayrollNumbers(payrollGroupId)?.Value ?? new List<TrnPayrollDto>());

            EmployeeId = employeeId;
            EmploymentType = employeeType;

            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "2.1", Text = "Payslip (Continues)" },               
            };
        }
    }
}
