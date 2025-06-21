using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayrollOtherDeduction
{
    [Authorize]
    [Secure("TrnPayrollOtherDeduction")]
    public class LoansModel : PageModel
    {
        private IMediator _mediator;
        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public LoansModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void OnGet()
        {
        }
    }
}
