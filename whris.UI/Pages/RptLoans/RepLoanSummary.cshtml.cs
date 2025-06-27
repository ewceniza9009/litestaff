using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLoans
{
    [Authorize]
    [Secure("RepLoan")]
    public class RepLoanSummaryModel : PageModel
    {
        public Reports.RepLoanSummary? LoanSummary = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd, bool? isPaid, int employeeId, int companyId)
        {
            LoanSummary = new Reports.RepLoanSummary();

            LoanSummary.Parameters["ParamDateStart"].Value = paramDateStart;
            LoanSummary.Parameters["ParamDateStart"].Visible = false;

            LoanSummary.Parameters["ParamDateEnd"].Value = paramDateEnd;
            LoanSummary.Parameters["ParamDateEnd"].Visible = false;

            LoanSummary.Parameters["IsPaid"].Value = isPaid;
            LoanSummary.Parameters["IsPaid"].Visible = false;

            LoanSummary.Parameters["EmployeeId"].Value = employeeId;
            LoanSummary.Parameters["EmployeeId"].Visible = false;

            LoanSummary.Parameters["CompanyId"].Value = companyId;
            LoanSummary.Parameters["CompanyId"].Visible = false;
        }
    }
}
