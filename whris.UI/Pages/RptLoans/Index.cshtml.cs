using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Models;
using whris.UI.Authorization;
using static whris.Application.Queries.TrnPayrollOtherDeduction.EmployeeLoanList;

namespace whris.UI.Pages.RptLoans
{
    [Authorize]
    [Secure("RepLoan")]
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public List<MstEmployeeDto> Employees => (List<MstEmployeeDto>)(Common.GetEmployees()?.Value ?? new List<MstEmployeeDto>());
        public List<EmployeeLoan> Loans => (List<EmployeeLoan>)(Common.GetLoans()?.Value ?? new List<EmployeeLoan>());
        public List<TrnPayrollDto> PayrollNumbers => (List<TrnPayrollDto>)(Common.GetPayrollNumbers()?.Value ?? new List<TrnPayrollDto>());
        public List<MstPeriod> Periods => (List<MstPeriod>)(Common.GetPeriods()?.Value ?? new List<MstPeriod>());
        public List<MstMonth> Months => (List<MstMonth>)(Common.GetMonths()?.Value ?? new List<MstMonth>());
        public List<Data.Models.MstCompany> Companies => (List<Data.Models.MstCompany>)(Common.GetCompanies()?.Value ?? new List<Data.Models.MstCompany>());

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Loan Summary" },
                new ReportList(){ Value = "2", Text = "Loan Ledger" },
                new ReportList(){ Value = "2.1", Text = "Loan Cross Tab" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "4", Text = "Loan Deduction" },
                new ReportList(){ Value = "5", Text = "" },
                new ReportList(){ Value = "6", Text = "SSS Loans Cover Letter" },
                new ReportList(){ Value = "7", Text = "SSS Loans Report" },
                new ReportList(){ Value = "8", Text = "SSS Loans Text File" },
            };
        }
    }
}
