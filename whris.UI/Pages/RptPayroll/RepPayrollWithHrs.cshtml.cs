using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class RepPayrollWithHrs : PageModel
    {
        public Reports.RepPayrollWithHrs? PayrollWithHours = null;
        public void OnGet(int paramId, int? paramEmploymentType, int paramCompanyId, int paramBranchId)
        {
            PayrollWithHours = new Reports.RepPayrollWithHrs();

            PayrollWithHours.Parameters["ParamPayrollId"].Value = paramId;
            PayrollWithHours.Parameters["ParamPayrollId"].Visible = false;

            PayrollWithHours.Parameters["ParamEmploymentType"].Value = paramEmploymentType;
            PayrollWithHours.Parameters["ParamEmploymentType"].Visible = false;

            PayrollWithHours.Parameters["ParamCompanyId"].Value = paramCompanyId;
            PayrollWithHours.Parameters["ParamCompanyId"].Visible = false;

            PayrollWithHours.Parameters["ParamBranchId"].Value = paramBranchId;
            PayrollWithHours.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
