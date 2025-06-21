using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptATMBank
{
    [Authorize]
    [Secure("RepBank")]
    public class RepNoneATMReportModel : PageModel
    {
        public Reports.RepNoneATMReport? NoneATMReport = null;
        public void OnGet(int paramPayrollId, DateTime paramDateStart, DateTime paramDateEnd, int paramBranchId)
        {
            NoneATMReport = new Reports.RepNoneATMReport();

            NoneATMReport.Parameters["ParamPayrollId"].Value = paramPayrollId;
            NoneATMReport.Parameters["ParamPayrollId"].Visible = false;

            NoneATMReport.Parameters["ParamDateStart"].Value = paramDateStart;
            NoneATMReport.Parameters["ParamDateStart"].Visible = false;

            NoneATMReport.Parameters["ParamDateEnd"].Value = paramDateEnd;
            NoneATMReport.Parameters["ParamDateEnd"].Visible = false;

            NoneATMReport.Parameters["ParamBranchId"].Value = paramBranchId;
            NoneATMReport.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
