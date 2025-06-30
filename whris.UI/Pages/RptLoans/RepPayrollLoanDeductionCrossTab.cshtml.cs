using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptLoans
{
    [Authorize]
    [Secure("RepLoan")]
    public class RepPayrollLoanDeductionCrossTabModel : PageModel
    {
        public Reports.RepPayrollLoanDeductionCrossTab? PayrollLoanDeductionCrossTab = null;
        public void OnGet(int paramPayrollId, DateTime paramDateStart, DateTime paramDateEnd)
        {
            var payrollNumber = Lookup.GetPayrollNoById(paramPayrollId);
            PayrollLoanDeductionCrossTab = new Reports.RepPayrollLoanDeductionCrossTab();

            PayrollLoanDeductionCrossTab.Parameters["ParamPayrollId"].Value = paramPayrollId;
            PayrollLoanDeductionCrossTab.Parameters["ParamPayrollId"].Visible = false;

            PayrollLoanDeductionCrossTab.Parameters["ParamPayrollNumber"].Value = payrollNumber;
            PayrollLoanDeductionCrossTab.Parameters["ParamPayrollNumber"].Visible = false;

            PayrollLoanDeductionCrossTab.Parameters["ParamDateStart"].Value = paramDateStart;
            PayrollLoanDeductionCrossTab.Parameters["ParamDateStart"].Visible = false;

            PayrollLoanDeductionCrossTab.Parameters["ParamDateEnd"].Value = paramDateEnd;
            PayrollLoanDeductionCrossTab.Parameters["ParamDateEnd"].Visible = false;
        }
    }
}
