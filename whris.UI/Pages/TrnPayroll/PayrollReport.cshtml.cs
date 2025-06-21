using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.TrnPayroll
{
    [Authorize]
    [Secure("TrnPayroll")]
    public class PayrollReportModel : PageModel
    {
        public Reports.TrnPayroll? Payroll = null;
        public void OnGet(int paramId)
        {
            Payroll = new Reports.TrnPayroll();

            Payroll.Parameters["ParamId"].Value = paramId;
            Payroll.Parameters["ParamId"].Visible = false;
        }
    }
}
