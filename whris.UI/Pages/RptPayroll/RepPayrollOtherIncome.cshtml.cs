using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Library;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class RepPayrollOtherIncomeModel : PageModel
    {
        public Reports.RepPayrollOtherIncome? PayrollOtherIncome = null;
        public void OnGet(int paramId, int paramCompanyId, int paramBranchId)
        {
            PayrollOtherIncome = new Reports.RepPayrollOtherIncome();

            PayrollOtherIncome.Parameters["ParamPayrollId"].Value = paramId;
            PayrollOtherIncome.Parameters["ParamPayrollId"].Visible = false;

            PayrollOtherIncome.Parameters["ParamCompanyId"].Value = paramCompanyId;
            PayrollOtherIncome.Parameters["ParamCompanyId"].Visible = false;

            PayrollOtherIncome.Parameters["ParamBranchId"].Value = paramBranchId;
            PayrollOtherIncome.Parameters["ParamBranchId"].Visible = false;
        }
    }
}
