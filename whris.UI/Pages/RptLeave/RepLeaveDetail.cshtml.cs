using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLeave
{
    [Authorize]
    [Secure("RepLeave")]
    public class RepLeaveDetailModel : PageModel
    {
        public Reports.RepLeaveDetail? LeaveDetail = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd)
        {
            LeaveDetail = new Reports.RepLeaveDetail();

            LeaveDetail.Parameters["ParamDateStart"].Value = paramDateStart;
            LeaveDetail.Parameters["ParamDateStart"].Visible = false;

            LeaveDetail.Parameters["ParamDateEnd"].Value = paramDateEnd;
            LeaveDetail.Parameters["ParamDateEnd"].Visible = false;
        }
    }
}
