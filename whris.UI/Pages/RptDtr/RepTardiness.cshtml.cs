using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptDtr
{
    [Authorize]
    [Secure("RepDTR")]
    public class RepTardinessModel : PageModel
    {
        public Reports.RepTardiness? Tardiness = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd, int paramCompanyId, int paramBranchId)
        {
            Tardiness = new Reports.RepTardiness();

            Tardiness.Parameters["ParamDateStart"].Value = paramDateStart;
            Tardiness.Parameters["ParamDateStart"].Visible = false;

            Tardiness.Parameters["ParamDateEnd"].Value = paramDateEnd;
            Tardiness.Parameters["ParamDateEnd"].Visible = false;

            Tardiness.Parameters["ParamCompanyId"].Value = paramCompanyId;
            Tardiness.Parameters["ParamCompanyId"].Visible = false;

            Tardiness.Parameters["ParamBranchId"].Value = paramBranchId;
            Tardiness.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
