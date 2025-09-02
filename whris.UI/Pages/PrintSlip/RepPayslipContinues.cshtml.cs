using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Services;

namespace whris.UI.Pages.PrintSlip
{
    [IgnoreAntiforgeryToken]
    public class RepSlipContinuesModel : PageModel
    {
        public Reports.RepPayslipLengthwiseContinues? PayslipContinues = null;

        public IActionResult OnGet(int paramId, int paramEmployeeId, int? paramEmploymentType)
        {
            // 1. Validate token from cookie
            var token = Request.Cookies["SaintSeiya"];
            if (string.IsNullOrEmpty(token))
            {
                return Redirect("/LogToPrintSlip");
            }

            var validatedEmployeeId = TokenService.ValidateToken(token);
            if (validatedEmployeeId == null)
            {
                return Redirect("/LogToPrintSlip");
            }

            // 2. Continue with your existing logic
            PayslipContinues = new Reports.RepPayslipLengthwiseContinues();

            PayslipContinues.Parameters["ParamPayrollId"].Value = paramId;
            PayslipContinues.Parameters["ParamPayrollId"].Visible = false;

            PayslipContinues.Parameters["ParamEmployeeId"].Value = paramEmployeeId;
            PayslipContinues.Parameters["ParamEmployeeId"].Visible = false;

            PayslipContinues.Parameters["ParamEmploymentType"].Value = paramEmploymentType;
            PayslipContinues.Parameters["ParamEmploymentType"].Visible = false;

            return Page();
        }
    }
}
