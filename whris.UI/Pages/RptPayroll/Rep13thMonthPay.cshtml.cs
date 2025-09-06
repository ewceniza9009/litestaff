using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("Rep13thMonthPay")]
    public class Rep13thMonthPay : PageModel
    {
        public Reports.Rep13thMonthPay? ThirteenthMonth = null;
        //public void OnGet(int paramId, int? paramEmploymentType, int paramCompanyId, int paramBranchId, int paramMonthId)
        public void OnGet(int ParamStartPayNo, int ParamEndPayNo, int ParamEmployeeId, int ParamPayrollGroupId)
        {
            ThirteenthMonth = new Reports.Rep13thMonthPay();

            ThirteenthMonth.Parameters["ParamStartPayNo"].Value = ParamStartPayNo;
            ThirteenthMonth.Parameters["ParamStartPayNo"].Visible = false;

            ThirteenthMonth.Parameters["ParamEndPayNo"].Value = ParamEndPayNo;
            ThirteenthMonth.Parameters["ParamEndPayNo"].Visible = false;

            ThirteenthMonth.Parameters["ParamEmployeeId"].Value = ParamEmployeeId;
            ThirteenthMonth.Parameters["ParamEmployeeId"].Visible = false;

            ThirteenthMonth.Parameters["ParamPayrollGroupId"].Value = ParamPayrollGroupId;
            ThirteenthMonth.Parameters["ParamPayrollGroupId"].Visible = false;
        }
    }
}