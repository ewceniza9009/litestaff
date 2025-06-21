using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.RptLeave
{
    [Authorize]
    [Secure("RepLeave")]
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Leave Application Summary" },
                new ReportList(){ Value = "2", Text = "Leave Application Detail" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "4", Text = "Leave Ledger" },
                new ReportList(){ Value = "5", Text = "Leave Ledger Summary" },
            };
        }
    }
}
