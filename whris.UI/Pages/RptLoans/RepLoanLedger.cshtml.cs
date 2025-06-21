using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLoans
{
    [Authorize]
    [Secure("RepLoan")]
    public class RepLoanLedgerModel : PageModel
    {
        public Reports.RepLoanLedger? LoanLedger = null;
        public void OnGet(int paramEmployeeId, int paramLoanId)
        {
            LoanLedger = new Reports.RepLoanLedger();

            LoanLedger.Parameters["ParamEmployeeId"].Value = paramEmployeeId;
            LoanLedger.Parameters["ParamEmployeeId"].Visible = false;

            LoanLedger.Parameters["ParamLoanId"].Value = paramLoanId;
            LoanLedger.Parameters["ParamLoanId"].Visible = false;
        }
    }
}
