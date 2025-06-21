using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptDtr
{
    [Authorize]
    [Secure("RepDTR")]
    public class RepAbsencesModel : PageModel
    {
        public Reports.RepAbsences? Absences = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd, int paramBranchId)
        {
            Absences = new Reports.RepAbsences();

            Absences.Parameters["ParamDateStart"].Value = paramDateStart;
            Absences.Parameters["ParamDateStart"].Visible = false;

            Absences.Parameters["ParamDateEnd"].Value = paramDateEnd;
            Absences.Parameters["ParamDateEnd"].Visible = false;

            Absences.Parameters["ParamBranchId"].Value = paramBranchId;
            Absences.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
