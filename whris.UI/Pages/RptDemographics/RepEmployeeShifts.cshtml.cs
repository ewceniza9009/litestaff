using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptDemographics
{
    [Authorize]
    [Secure("RepDemographics")]
    public class RepEmployeeShiftsModel : PageModel
    {
        public Reports.RepEmployeeShifts? EmployeeShifts = null;
        public void OnGet(int paramPayrollGroupId)
        {
            EmployeeShifts = new Reports.RepEmployeeShifts();

            EmployeeShifts.Parameters["ParamId"].Value = paramPayrollGroupId;
            EmployeeShifts.Parameters["ParamId"].Visible = false;
        }
    }
}
