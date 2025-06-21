using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.TrnLeaveApplication
{
    [Authorize]
    [Secure("TrnLeaveApplication")]
    public class LAReportModel : PageModel
    {
        public Reports.TrnLeave? LA = null;
        public void OnGet(int paramId)
        {
            LA = new Reports.TrnLeave();

            LA.Parameters["ParamId"].Value = paramId;
            LA.Parameters["ParamId"].Visible = false;
        }
    }
}
