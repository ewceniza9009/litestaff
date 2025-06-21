using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.TrnChangeShiftCode
{
    [Authorize]
    [Secure("TrnChangeShift")]
    public class ChangeShiftReportModel : PageModel
    {
        public Reports.TrnChangeShift? ChangeShift = null;
        public void OnGet(int paramId)
        {
            ChangeShift = new Reports.TrnChangeShift();

            ChangeShift.Parameters["ParamId"].Value = paramId;
            ChangeShift.Parameters["ParamId"].Visible = false;
        }
    }
}
