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
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd)
        {
            LoanSummary = new Reports.RepLoanSummary();

            LoanSummary.Parameters["ParamDateStart"].Value = paramDateStart;
            LoanSummary.Parameters["ParamDateStart"].Visible = false;

            LoanSummary.Parameters["ParamDateEnd"].Value = paramDateEnd;
            LoanSummary.Parameters["ParamDateEnd"].Visible = false;
        }
    }
}
