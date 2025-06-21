using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLoans
{
    [Authorize]
    [Secure("RepLoan")]
    public class RepSSSLoansReportModel : PageModel
    {
        public Reports.RepSSSLoansReport? SSSLoansReport = null;
        public void OnGet(int paramPeriodId, int paramMonthId, int paramCompanyId)
        {
            SSSLoansReport = new Reports.RepSSSLoansReport();

            SSSLoansReport.Parameters["ParamPeriodId"].Value = paramPeriodId;
            SSSLoansReport.Parameters["ParamPeriodId"].Visible = false;

            SSSLoansReport.Parameters["ParamMonthId"].Value = paramMonthId;
            SSSLoansReport.Parameters["ParamMonthId"].Visible = false;

            SSSLoansReport.Parameters["ParamCompanyId"].Value = paramCompanyId;
            SSSLoansReport.Parameters["ParamCompanyId"].Visible = false;
        }
    }
}
