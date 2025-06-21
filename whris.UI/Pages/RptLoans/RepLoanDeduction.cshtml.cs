using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLoans
{
    [Authorize]
    [Secure("RepLoan")]
    public class RepLoanDeductionModel : PageModel
    {
        public Reports.RepLoanDeduction? LoanDeduction = null;
        public void OnGet(int paramPayrollId)
        {
            LoanDeduction = new Reports.RepLoanDeduction();

            LoanDeduction.Parameters["ParamPayrollId"].Value = paramPayrollId;
            LoanDeduction.Parameters["ParamPayrollId"].Visible = false;
        }
    }
}
