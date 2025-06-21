using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace whris.UI.Pages.PrintSlip
{
    [IgnoreAntiforgeryToken]
    public class RepSlipContinuesModel : PageModel
    {
        public Reports.RepPayslipLengthwiseContinues? PayslipContinues = null;
        public void OnGet(int paramId, int paramEmployeeId, int? paramEmploymentType)
        {
            PayslipContinues = new Reports.RepPayslipLengthwiseContinues();

            PayslipContinues.Parameters["ParamPayrollId"].Value = paramId;
            PayslipContinues.Parameters["ParamPayrollId"].Visible = false;

            PayslipContinues.Parameters["ParamEmployeeId"].Value = paramEmployeeId;
            PayslipContinues.Parameters["ParamEmployeeId"].Visible = false;

            PayslipContinues.Parameters["ParamEmploymentType"].Value = paramEmploymentType;
            PayslipContinues.Parameters["ParamEmploymentType"].Visible = false;
        }
    }
}
