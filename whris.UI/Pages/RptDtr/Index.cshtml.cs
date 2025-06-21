using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.RptDtr
{
    [Authorize]
    [Secure("RepDTR")]
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public MstEmployeeComboboxDatasources ComboboxDatasources = MstEmployeeComboboxDatasources.Instance;

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Tardiness Report" },
                new ReportList(){ Value = "1.5", Text = "Tardiness Summary Report" },
                new ReportList(){ Value = "2", Text = "Absences Report" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "4", Text = "Allowance Report" },
            };
        }
    }
}
