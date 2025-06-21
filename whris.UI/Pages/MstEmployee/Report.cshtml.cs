using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstEmployee
{
    [Authorize]
    [Secure("MstEmployee")]
    public class ReportModel : PageModel
    {
        public Reports.MstEmployee? EmployeeInformation = null;
        public void OnGet(int paramId)
        {
            EmployeeInformation = new Reports.MstEmployee();

            EmployeeInformation.Parameters["ParamId"].Value = paramId;
            EmployeeInformation.Parameters["ParamId"].Visible = false;
        }
    }
}
