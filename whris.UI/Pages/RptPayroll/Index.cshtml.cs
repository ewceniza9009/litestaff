using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public MstEmployeeComboboxDatasources ComboboxDatasources = MstEmployeeComboboxDatasources.Instance;
        public List<TrnPayrollDto> PayrollNumbers => (List<TrnPayrollDto>)(Common.GetPayrollNumbers()?.Value ?? new List<TrnPayrollDto>());
        public List<MstEmploymentTypeDto> EmploymentTypeCmbDs => new List<MstEmploymentTypeDto>()
        {
            new MstEmploymentTypeDto() { Id = 1, EmploymentType = "Regular" },
            new MstEmploymentTypeDto() { Id = 2, EmploymentType = "Probationary"},
            new MstEmploymentTypeDto() { Id = 3, EmploymentType = "Newly Hired"}
        };
        public List<MstEmployeeDto> Employees => (List<MstEmployeeDto>)(Common.GetEmployees()?.Value ?? new List<MstEmployeeDto>());

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Payslip" },
                new ReportList(){ Value = "2", Text = "Payslip (Lengthwise)" },
                new ReportList(){ Value = "2.1", Text = "Payslip (Continues)" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "4", Text = "Payroll Worksheet w/ No. of Hrs" },
                new ReportList(){ Value = "5", Text = "Payroll Worksheet w/ Departments" },
                //new ReportList(){ Value = "6", Text = "Payroll Worksheet w/ Departments and Details" },
                new ReportList(){ Value = "7", Text = "" },
                new ReportList(){ Value = "8", Text = "Other Deduction Detail Report" },
                new ReportList(){ Value = "9", Text = "Other Income Detail Report" },
                //new ReportList(){ Value = "10", Text = "" },
                //new ReportList(){ Value = "11", Text = "Overtime/Holiday Payslip" },
                //new ReportList(){ Value = "12", Text = "Overtime/Holiday Worksheet w/ No. of Hrs" },                 
            };
        }
    }
}
