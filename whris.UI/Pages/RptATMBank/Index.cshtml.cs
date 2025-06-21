using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.RptATMBank
{
    [Authorize]
    [Secure("RepBank")]
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public List<TrnPayrollDto> PayrollNumbers => (List<TrnPayrollDto>)(Common.GetPayrollNumbers()?.Value ?? new List<TrnPayrollDto>());
        public MstEmployeeComboboxDatasources ComboboxDatasources = MstEmployeeComboboxDatasources.Instance;

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "ATM/Bank Report" },
                new ReportList(){ Value = "2", Text = "Non-ATM/Bank Report" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "4", Text = "ATM Text File" },
            };
        }
    }
}
