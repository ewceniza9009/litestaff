using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptDtr
{
    [Authorize]
    [Secure("RepDTR")]
    public class RepAllowanceModel : PageModel
    {
        public Reports.RepAllowance? Allowance = null;
        public void OnGet(DateTime paramDateStart, DateTime paramDateEnd, int paramCompanyId, int paramBranchId)
        {
            Allowance = new Reports.RepAllowance();

            Allowance.Parameters["ParamDateStart"].Value = paramDateStart;
            Allowance.Parameters["ParamDateStart"].Visible = false;

            Allowance.Parameters["ParamDateEnd"].Value = paramDateEnd;
            Allowance.Parameters["ParamDateEnd"].Visible = false;

            Allowance.Parameters["ParamCompanyId"].Value = paramCompanyId;
            Allowance.Parameters["ParamCompanyId"].Visible = false;

            Allowance.Parameters["ParamBranchId"].Value = paramBranchId;
            Allowance.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
