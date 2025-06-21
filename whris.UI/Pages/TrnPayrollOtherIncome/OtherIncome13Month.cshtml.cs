using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayrollOtherIncome
{
    [Authorize]
    [Secure("TrnPayrollOtherIncome")]
    public class OtherIncome13Month : PageModel
    {
        private IMediator _mediator;

        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public OtherIncome13Month(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            return await Task.Run(() => Page());
        }
    }
}
