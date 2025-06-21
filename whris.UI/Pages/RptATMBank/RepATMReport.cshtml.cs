using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptATMBank
{
    [Authorize]
    [Secure("RepBank")]
    public class RepATMReportModel : PageModel
    {
        public Reports.RepATMReport? ATMReport = null;
        public void OnGet(int paramPayrollId, DateTime paramDateStart, DateTime paramDateEnd, int paramBranchId)
        {
            ATMReport = new Reports.RepATMReport();

            ATMReport.Parameters["ParamPayrollId"].Value = paramPayrollId;
            ATMReport.Parameters["ParamPayrollId"].Visible = false;

            ATMReport.Parameters["ParamDateStart"].Value = paramDateStart;
            ATMReport.Parameters["ParamDateStart"].Visible = false;

            ATMReport.Parameters["ParamDateEnd"].Value = paramDateEnd;
            ATMReport.Parameters["ParamDateEnd"].Visible = false;

            ATMReport.Parameters["ParamBranchId"].Value = paramBranchId;
            ATMReport.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
