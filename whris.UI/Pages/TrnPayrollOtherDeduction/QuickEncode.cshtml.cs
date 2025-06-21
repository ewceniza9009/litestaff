using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.TrnPayrollOtherDeduction.Queries;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayrollOtherDeduction
{
    [Authorize]
    [Secure("TrnPayrollOtherDeduction")]
    public class QuickEncodeModel : PageModel
    {
        private IMediator _mediator;
        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public QuickEncodeModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostReadODList([DataSourceRequest] DataSourceRequest request)
        {
            var allOtherDeductions = new GetSelectOtherDeductions()
            {
                Request = request
            };

            return new JsonResult(await _mediator.Send(allOtherDeductions));
        }
    }
}
