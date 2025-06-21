using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptMandatory
{
    [Authorize]
    [Secure("RepMandatory")]
    public class RepGovernmentMonthlyModel : PageModel
    {
        public Reports.RepMonthlyGovernmentContributions? GovernmentMonthly = null;
        public void OnGet(int paramCompanyId, DateTime paramDateStart, DateTime paramDateEnd)
        {
            GovernmentMonthly = new Reports.RepMonthlyGovernmentContributions();

            GovernmentMonthly.Parameters["ParamCompanyId"].Value = paramCompanyId;
            GovernmentMonthly.Parameters["ParamCompanyId"].Visible = false;

            GovernmentMonthly.Parameters["ParamDateStart"].Value = paramDateStart;
            GovernmentMonthly.Parameters["ParamDateStart"].Visible = false;

            GovernmentMonthly.Parameters["ParamDateEnd"].Value = paramDateEnd;
            GovernmentMonthly.Parameters["ParamDateEnd"].Visible = false;
        }
    }
}

