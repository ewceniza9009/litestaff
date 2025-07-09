using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class RepMonthlyPayroll : PageModel
    {
        public Reports.RepMonthlyPayroll? MonthlyPayroll = null;
        //public void OnGet(int paramId, int? paramEmploymentType, int paramCompanyId, int paramBranchId, int paramMonthId)
        public void OnGet( int paramPayrollGroupId, int paramMonthId, int paramEmploymentType, int paramCompanyId, int paramBranchId, int paramPeriod)
        {
            MonthlyPayroll = new Reports.RepMonthlyPayroll();

            MonthlyPayroll.Parameters["ParamPayrollGroupId"].Value = paramPayrollGroupId;
            MonthlyPayroll.Parameters["ParamPayrollGroupId"].Visible = false;

            MonthlyPayroll.Parameters["ParamPeriod"].Value = paramPeriod;
            MonthlyPayroll.Parameters["ParamPeriod"].Visible = false;

            MonthlyPayroll.Parameters["ParamMonthId"].Value = paramMonthId;
            MonthlyPayroll.Parameters["ParamMonthId"].Visible = false;

            MonthlyPayroll.Parameters["ParamEmploymentType"].Value = paramEmploymentType;
            MonthlyPayroll.Parameters["ParamEmploymentType"].Visible = false;

            MonthlyPayroll.Parameters["ParamCompanyId"].Value = paramCompanyId;
            MonthlyPayroll.Parameters["ParamCompanyId"].Visible = false;

            MonthlyPayroll.Parameters["ParamBranchId"].Value = paramBranchId;
            MonthlyPayroll.Parameters["ParamBranchId"].Visible = false;

            
        }
    }
}
