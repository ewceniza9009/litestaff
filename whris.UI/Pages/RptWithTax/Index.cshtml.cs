using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Models;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptWithTax
{
    [Authorize]
    [Secure("RepWithholdingTax")]
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public List<MstPeriod> Periods => (List<MstPeriod>)(Common.GetPeriods()?.Value ?? new List<MstPeriod>());
        public List<string> Quarters => new List<string>() {"1", "2", "3", "4"};
        public List<MstMonth> Months => (List<MstMonth>)(Common.GetMonths()?.Value ?? new List<MstMonth>());
        public List<Data.Models.MstCompany> Companies => (List<Data.Models.MstCompany>)(Common.GetCompanies()?.Value ?? new List<Data.Models.MstCompany>());

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Withholding Tax Monthly Report" },
                new ReportList(){ Value = "2", Text = "" },
                new ReportList(){ Value = "3", Text = "Alphalist of Employees" },
                new ReportList(){ Value = "4", Text = "Alphalist of Employees Text File" },
            };
        }
    }
}
