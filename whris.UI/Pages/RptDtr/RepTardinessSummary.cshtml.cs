using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptDtr
{
    [Authorize]
    [Secure("RepDTR")]
    public class RepTardinessSummaryModel : PageModel
    {
        public Reports.RepTardinessSummary? TardinessSummary = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd, int paramCompanyId, int paramBranchId)
        {
            TardinessSummary = new Reports.RepTardinessSummary();

            TardinessSummary.Parameters["ParamDateStart"].Value = paramDateStart;
            TardinessSummary.Parameters["ParamDateStart"].Visible = false;

            TardinessSummary.Parameters["ParamDateEnd"].Value = paramDateEnd;
            TardinessSummary.Parameters["ParamDateEnd"].Visible = false;

            TardinessSummary.Parameters["ParamCompanyId"].Value = paramCompanyId;
            TardinessSummary.Parameters["ParamCompanyId"].Visible = false;

            TardinessSummary.Parameters["ParamBranchId"].Value = paramBranchId;
            TardinessSummary.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
