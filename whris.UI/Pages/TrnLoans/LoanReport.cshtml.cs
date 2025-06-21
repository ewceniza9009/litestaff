using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.UI.Authorization;

namespace whris.UI.Pages.TrnLoans
{
    [Authorize]
    [Secure("MstEmployeeLoan")]
    public class LoanReportModel : PageModel
    {
        public Reports.TrnLoan? Loan = null;
        public void OnGet(int paramId)
        {
            Loan = new Reports.TrnLoan();

            Loan.Parameters["ParamId"].Value = paramId;
            Loan.Parameters["ParamId"].Visible = false;

            Loan.Parameters["TotalPayment"].Value = Lookup.GetLoanTotalAmountByLoanId(paramId);
            Loan.Parameters["TotalPayment"].Visible = false;

            Loan.Parameters["Balance"].Value = Lookup.GetLoanBalanceByLoanId(paramId);
            Loan.Parameters["Balance"].Visible = false;
        }
    }
}
