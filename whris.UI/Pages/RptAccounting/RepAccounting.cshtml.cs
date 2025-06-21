using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptAccounting
{
    [Authorize]
    [Secure("RepAccounting")]
    public class RepAccountingModel : PageModel
    {
        public Reports.RepAccounting? Accounting = null;
        public void OnGet(int paramPayrollId)
        {
            Accounting = new Reports.RepAccounting();

            Accounting.Parameters["ParamPayrollId"].Value = paramPayrollId;
            Accounting.Parameters["ParamPayrollId"].Visible = false;
        }
    }
}
