using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLoans
{
    [Authorize]
    [Secure("RepLoan")]
    public class RepSSSLoanCoverLetterModel : PageModel
    {
        public Reports.RepSSSLoanCoverLetter? SSSLoanCoverLetter = null;
        public void OnGet(int paramPeriodId, int paramMonthId, int paramCompanyId, string paramReceiptNumber, DateTime paramReceiptDate, decimal paramReceiptAmount, string paramFileName)
        {
            SSSLoanCoverLetter = new Reports.RepSSSLoanCoverLetter();

            SSSLoanCoverLetter.Parameters["ParamPeriodId"].Value = paramPeriodId;
            SSSLoanCoverLetter.Parameters["ParamPeriodId"].Visible = false;

            SSSLoanCoverLetter.Parameters["ParamMonthId"].Value = paramMonthId;
            SSSLoanCoverLetter.Parameters["ParamMonthId"].Visible = false;

            SSSLoanCoverLetter.Parameters["ParamCompanyId"].Value = paramCompanyId;
            SSSLoanCoverLetter.Parameters["ParamCompanyId"].Visible = false;

            SSSLoanCoverLetter.Parameters["ParamReceiptNumber"].Value = paramReceiptNumber;
            SSSLoanCoverLetter.Parameters["ParamReceiptNumber"].Visible = false;

            SSSLoanCoverLetter.Parameters["ParamReceiptDate"].Value = paramReceiptDate;
            SSSLoanCoverLetter.Parameters["ParamReceiptDate"].Visible = false;

            SSSLoanCoverLetter.Parameters["ParamReceiptAmount"].Value = paramReceiptAmount;
            SSSLoanCoverLetter.Parameters["ParamReceiptAmount"].Visible = false;

            SSSLoanCoverLetter.Parameters["ParamFileName"].Value = paramFileName;
            SSSLoanCoverLetter.Parameters["ParamFileName"].Visible = false;
        }
    }
}
