using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class RepPayslipLengthwiseModel : PageModel
    {
        public Reports.RepPayslipLengthwise? PayslipLengthwise = null;
        public void OnGet(int paramId, int? paramEmploymentType)
        {
            PayslipLengthwise = new Reports.RepPayslipLengthwise();

            PayslipLengthwise.Parameters["ParamPayrollId"].Value = paramId;
            PayslipLengthwise.Parameters["ParamPayrollId"].Visible = false;

            PayslipLengthwise.Parameters["ParamEmploymentType"].Value = paramEmploymentType;
            PayslipLengthwise.Parameters["ParamEmploymentType"].Visible = false;
        }
    }
}
