using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class RepPayslipModel : PageModel
    {
        public Reports.RepPayslip? Payslip = null;
        public void OnGet(int paramId)
        {
            Payslip = new Reports.RepPayslip();

            Payslip.Parameters["ParamPayrollId"].Value = paramId;
            Payslip.Parameters["ParamPayrollId"].Visible = false;
        }
    }
}
