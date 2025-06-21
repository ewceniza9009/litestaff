using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.TrnPayrollOtherIncome
{
    [Authorize]
    [Secure("TrnPayrollOtherIncome")]
    public class OtherIncomeReportModel : PageModel
    {
        public Reports.TrnOtherIncome? OtherIncome = null;
        public void OnGet(int paramId)
        {
            OtherIncome = new Reports.TrnOtherIncome();

            OtherIncome.Parameters["ParamId"].Value = paramId;
            OtherIncome.Parameters["ParamId"].Visible = false;
        }
    }
}
