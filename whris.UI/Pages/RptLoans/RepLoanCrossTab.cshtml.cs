using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLoans
{
    [Authorize]
    [Secure("RepLoan")]
    public class RepLoanCrossTabModel : PageModel
    {
        public Reports.RepLoanCrossTab? LoanCrossTab = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd, int companyId)
        {
            LoanCrossTab = new Reports.RepLoanCrossTab();

            LoanCrossTab.Parameters["ParamDateStart"].Value = paramDateStart;
            LoanCrossTab.Parameters["ParamDateStart"].Visible = false;
            LoanCrossTab.Parameters["CompanyId"].Value = companyId;

            LoanCrossTab.Parameters["ParamDateEnd"].Value = paramDateEnd;
            LoanCrossTab.Parameters["ParamDateEnd"].Visible = false;
            LoanCrossTab.Parameters["CompanyId"].Visible = false;
        }
    }
}
