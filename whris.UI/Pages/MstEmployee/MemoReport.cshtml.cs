using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace whris.UI.Pages.MstEmployee
{
    [Authorize]
    public class MemoReportModel : PageModel
    {
        public Reports.MstEmployeeMemo? EmployeeMemo = null;
        public void OnGet(int paramId)
        {
            EmployeeMemo = new Reports.MstEmployeeMemo();

            EmployeeMemo.Parameters["ParamId"].Value = paramId;
            EmployeeMemo.Parameters["ParamId"].Visible = false;
        }
    }
}
