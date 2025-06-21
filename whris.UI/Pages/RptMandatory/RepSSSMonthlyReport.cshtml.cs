using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptMandatory
{
    [Authorize]
    [Secure("RepMandatory")]
    public class RepSSSMonthlyModel : PageModel
    {
        public Reports.RepSSSMonthly? SSSMonthly = null;
        public void OnGet(int paramPeriodId, int paramMonthId, int paramCompanyId)
        {
            SSSMonthly = new Reports.RepSSSMonthly();

            SSSMonthly.Parameters["ParamPeriodId"].Value = paramPeriodId;
            SSSMonthly.Parameters["ParamPeriodId"].Visible = false;

            SSSMonthly.Parameters["ParamMonthId"].Value = paramMonthId;
            SSSMonthly.Parameters["ParamMonthId"].Visible = false;

            SSSMonthly.Parameters["ParamCompanyId"].Value = paramCompanyId;
            SSSMonthly.Parameters["ParamCompanyId"].Visible = false;
        }
    }
}
