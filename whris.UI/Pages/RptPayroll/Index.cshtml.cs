using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Models;
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

        public List<MstMonthDto> MonthCmbDs => new List<MstMonthDto>()
        {
            new MstMonthDto() { Id = 1, Month = "January" },
            new MstMonthDto() { Id = 2, Month = "February" },
            new MstMonthDto() { Id = 3, Month = "March" },
            new MstMonthDto() { Id = 4, Month = "April" },
            new MstMonthDto() { Id = 5, Month = "May" },
            new MstMonthDto() { Id = 6, Month = "June" },
            new MstMonthDto() { Id = 7, Month = "July" },
            new MstMonthDto() { Id = 8, Month = "August" },
            new MstMonthDto() { Id = 9, Month = "September" },
            new MstMonthDto() { Id = 10, Month = "October" },
            new MstMonthDto() { Id = 11, Month = "November" },
            new MstMonthDto() { Id = 12, Month = "December" }
        };
            
        public List<MstPayrollGroupDto> PayrollGroupCmbDs => new List<MstPayrollGroupDto>()
        {
            new MstPayrollGroupDto() { Id = 51, PayrollGroup = "Semi-Monthly" },
            new MstPayrollGroupDto() { Id = 52, PayrollGroup = "Weekly" },
            new MstPayrollGroupDto() { Id = 54, PayrollGroup = "Daily" },
            new MstPayrollGroupDto() { Id = 55, PayrollGroup = "Monthly" }
        };

        public List<MstPeriodDto> PeriodCmbDs => new List<MstPeriodDto>()
        {
            new MstPeriodDto() { Id = 7, Period = "2024" },
            new MstPeriodDto() { Id = 8, Period = "2025" }
        };


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
                new ReportList(){ Value = "6", Text = "Monthly Payroll Worksheet" },
                new ReportList(){ Value = "8", Text = "" },
                new ReportList(){ Value = "9", Text = "Other Deduction Detail Report" },
                new ReportList(){ Value = "10", Text = "Other Income Detail Report" },
                //new ReportList(){ Value = "10", Text = "" },
                //new ReportList(){ Value = "11", Text = "Overtime/Holiday Payslip" },
                //new ReportList(){ Value = "12", Text = "Overtime/Holiday Worksheet w/ No. of Hrs" },                 
            };
        }
    }
}
