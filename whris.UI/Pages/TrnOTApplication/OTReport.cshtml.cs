using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.TrnOTApplication
{
    [Authorize]
    [Secure("TrnOverTime")]
    public class OTReportModel : PageModel
    {
        public Reports.TrnOvertime? OT = null;
        public void OnGet(int paramId)
        {
            OT = new Reports.TrnOvertime();

            OT.Parameters["ParamId"].Value = paramId;
            OT.Parameters["ParamId"].Visible = false;
        }
    }
}
