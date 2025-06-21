using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLeave
{
    [Authorize]
    [Secure("RepLeave")]
    public class RepLeaveLedgerSummaryModel : PageModel
    {
        public Reports.RepLeaveLedgerSummary? LeaveLedgerSummary = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd)
        {
            LeaveLedgerSummary = new Reports.RepLeaveLedgerSummary();

            LeaveLedgerSummary.Parameters["ParamDateStart"].Value = paramDateStart;
            LeaveLedgerSummary.Parameters["ParamDateStart"].Visible = false;

            LeaveLedgerSummary.Parameters["ParamDateEnd"].Value = paramDateEnd;
            LeaveLedgerSummary.Parameters["ParamDateEnd"].Visible = false;
        }
    }
}
