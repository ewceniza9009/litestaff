using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.TrnPayrollOtherIncome.Queries;
using whris.Data.Models;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayrollOtherIncome
{
    [Authorize]
    [Secure("TrnPayrollOtherIncome")]
    public class IndexModel : PageModel
    {
        private IMediator _mediator;

        public List<MstPayrollGroup> allPayrollGroups = new List<MstPayrollGroup>();
        public int PayrollGroupId { get; set; } = 51;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            GlobalComboboxSrc.PayrollGroupId = PayrollGroupId;
            allPayrollGroups = TrnDtrComboboxDatasources.Instance.PayrollGroupCmbDs;

            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostReadPODList([DataSourceRequest] DataSourceRequest request, int payrollGroupId)
        {
            var allPods = new GetTrnPayrollOtherIncomesByPayrollGroupId()
            {
                Request = request,
                PayrollGroupId = payrollGroupId
            };

            GlobalComboboxSrc.PayrollGroupId = payrollGroupId;

            return new JsonResult(await _mediator.Send(allPods));
        }
    }
}
