using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLeave
{
    [Authorize]
    [Secure("RepLeave")]
    public class RepLeaveSummaryModel : PageModel
    {
        public Reports.RepLeaveSummary? LeaveSummary = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd)
        {
            LeaveSummary = new Reports.RepLeaveSummary();

            LeaveSummary.Parameters["ParamDateStart"].Value = paramDateStart;
            LeaveSummary.Parameters["ParamDateStart"].Visible = false;

            LeaveSummary.Parameters["ParamDateEnd"].Value = paramDateEnd;
            LeaveSummary.Parameters["ParamDateEnd"].Visible = false;
        }
    }
}
