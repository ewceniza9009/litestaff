using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.UI.Authorization;

namespace whris.UI.Pages.RptAccounting
{
    [Authorize]
    [Secure("RepAccounting")]
    public class IndexModel : PageModel
    {
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public List<TrnPayrollDto> PayrollNumbers => (List<TrnPayrollDto>)(Common.GetPayrollNumbers()?.Value ?? new List<TrnPayrollDto>());

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Journal Voucher" }
            };
        }
    }
}
