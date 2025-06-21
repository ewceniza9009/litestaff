using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptDemographics
{
    [Authorize]
    [Secure("RepDemographics")]
    public class RepEmployeeListModel : PageModel
    {
        public Reports.RepEmployeeList? EmployeeList = null;
        public void OnGet(int paramPayrollGroupId)
        {
            EmployeeList = new Reports.RepEmployeeList();

            EmployeeList.Parameters["ParamPayrollGroupId"].Value = paramPayrollGroupId;
            EmployeeList.Parameters["ParamPayrollGroupId"].Visible = false;
        }
    }
}
