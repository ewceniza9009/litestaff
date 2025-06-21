using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class RepPayrollOtherDeductionModel : PageModel
    {
        public Reports.RepPayrollOtherDeduction? PayrollOtherDeduction = null;
        public void OnGet(int paramId, int paramCompanyId, int paramBranchId)
        {
            PayrollOtherDeduction = new Reports.RepPayrollOtherDeduction();

            PayrollOtherDeduction.Parameters["ParamPayrollId"].Value = paramId;
            PayrollOtherDeduction.Parameters["ParamPayrollId"].Visible = false;

            PayrollOtherDeduction.Parameters["ParamCompanyId"].Value = paramCompanyId;
            PayrollOtherDeduction.Parameters["ParamCompanyId"].Visible = false;

            PayrollOtherDeduction.Parameters["ParamBranchId"].Value = paramBranchId;
            PayrollOtherDeduction.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
