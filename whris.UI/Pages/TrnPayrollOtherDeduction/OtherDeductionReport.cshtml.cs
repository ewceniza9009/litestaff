using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.TrnPayrollOtherDeduction
{
    [Authorize]
    [Secure("TrnPayrollOtherDeduction")]
    public class OtherDeductionReportModel : PageModel
    {
        public Reports.TrnOtherDeduction? OtherDeduction = null;
        public void OnGet(int paramId)
        {
            OtherDeduction = new Reports.TrnOtherDeduction();

            OtherDeduction.Parameters["ParamId"].Value = paramId;
            OtherDeduction.Parameters["ParamId"].Visible = false;
        }
    }
}
