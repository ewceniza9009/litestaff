using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("Rep13thMonthPaySummary")]
    public class Rep13thMonthPaySummary : PageModel
    {
        public Reports.Rep13thMonthPaySummary? ThirteenthMonthSummary = null;
        //public void OnGet(int paramId, int? paramEmploymentType, int paramCompanyId, int paramBranchId, int paramMonthId)
        public void OnGet(int ParamStartPayNo, int ParamEndPayNo, int ParamEmployeeId, int ParamPayrollGroupId, int ParamCompanyId, int ParamBranchId, int ParamDepartmentId)
        {
            ThirteenthMonthSummary = new Reports.Rep13thMonthPaySummary();

            ThirteenthMonthSummary.Parameters["ParamStartPayNo"].Value = ParamStartPayNo;
            ThirteenthMonthSummary.Parameters["ParamStartPayNo"].Visible = false;

            ThirteenthMonthSummary.Parameters["ParamEndPayNo"].Value = ParamEndPayNo;
            ThirteenthMonthSummary.Parameters["ParamEndPayNo"].Visible = false;

            ThirteenthMonthSummary.Parameters["ParamEmployeeId"].Value = ParamEmployeeId;
            ThirteenthMonthSummary.Parameters["ParamEmployeeId"].Visible = false;

            ThirteenthMonthSummary.Parameters["ParamPayrollGroupId"].Value = ParamPayrollGroupId;
            ThirteenthMonthSummary.Parameters["ParamPayrollGroupId"].Visible = false;

            ThirteenthMonthSummary.Parameters["ParamCompanyId"].Value = ParamCompanyId;
            ThirteenthMonthSummary.Parameters["ParamCompanyId"].Visible = false;

            ThirteenthMonthSummary.Parameters["ParamBranchId"].Value = ParamBranchId;
            ThirteenthMonthSummary.Parameters["ParamBranchId"].Visible = false;

            ThirteenthMonthSummary.Parameters["ParamDepartmentId"].Value = ParamDepartmentId;
            ThirteenthMonthSummary.Parameters["ParamDepartmentId"].Visible = false;
        }
    }
}