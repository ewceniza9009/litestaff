using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.RptDemographics
{
    [Authorize]
    [Secure("RepDemographics")]
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Employee List" },
                new ReportList(){ Value = "2", Text = "Employee Shifts" },
            };
        }
    }
}
