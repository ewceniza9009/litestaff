using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLeave
{
    [Authorize]
    [Secure("RepLeave")]
    public class RepLeaveLedgerModel : PageModel
    {
        public Reports.RepLeaveLedger? LeaveLedger = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd, int paramEmployeeId)
        {
            LeaveLedger = new Reports.RepLeaveLedger();

            LeaveLedger.Parameters["ParamDateStart"].Value = paramDateStart;
            LeaveLedger.Parameters["ParamDateStart"].Visible = false;

            LeaveLedger.Parameters["ParamDateEnd"].Value = paramDateEnd;
            LeaveLedger.Parameters["ParamDateEnd"].Visible = false;

            LeaveLedger.Parameters["ParamEmployeeId"].Value = paramEmployeeId;
            LeaveLedger.Parameters["ParamEmployeeId"].Visible = false;
        }
    }
}
