using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.TrnDtr
{
    [Authorize]
    [Secure("TrnDTR")]
    public class DtrReportModel : PageModel
    {
        public Reports.TrnDTR? Dtr = null;
        public void OnGet(int paramId, string employeeFilter, bool withRemarks, bool inActive)
        {
            Dtr = new Reports.TrnDTR();

            Dtr.Parameters["ParamId"].Value = paramId;
            Dtr.Parameters["ParamId"].Visible = false;

            Dtr.Parameters["EmployeeFilter"].Value = employeeFilter;
            Dtr.Parameters["EmployeeFilter"].Visible = false;

            Dtr.Parameters["WithRemarks"].Value = withRemarks;
            Dtr.Parameters["WithRemarks"].Visible = false;

            Dtr.Parameters["ParamInActive"].Value = inActive;
            Dtr.Parameters["ParamInActive"].Visible = false;
        }
    }
}
