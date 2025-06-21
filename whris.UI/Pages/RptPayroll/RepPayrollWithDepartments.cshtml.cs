using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class RepPayrollWithDepartments : PageModel
    {
        public Reports.RepPayrollWithDepartments? PayrollWitDepartments = null;
        public void OnGet(int paramId, int? paramEmploymentType, int paramCompanyId, int paramBranchId, int paramDepartmentId)
        {
            PayrollWitDepartments = new Reports.RepPayrollWithDepartments();

            PayrollWitDepartments.Parameters["ParamPayrollId"].Value = paramId;
            PayrollWitDepartments.Parameters["ParamPayrollId"].Visible = false;

            PayrollWitDepartments.Parameters["ParamEmploymentType"].Value = paramEmploymentType;
            PayrollWitDepartments.Parameters["ParamEmploymentType"].Visible = false;

            PayrollWitDepartments.Parameters["ParamCompanyId"].Value = paramCompanyId;
            PayrollWitDepartments.Parameters["ParamCompanyId"].Visible = false;

            PayrollWitDepartments.Parameters["ParamBranchId"].Value = paramBranchId;
            PayrollWitDepartments.Parameters["ParamBranchId"].Visible = false;

            PayrollWitDepartments.Parameters["ParamDepartmentId"].Value = paramDepartmentId;
            PayrollWitDepartments.Parameters["ParamDepartmentId"].Visible = false;
        }
    }
}
