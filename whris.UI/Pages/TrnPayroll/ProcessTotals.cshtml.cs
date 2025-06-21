using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayroll
{
    public class ProcessTotalsModel : PageModel
    {
        private IMediator _mediator;

        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public ProcessTotalsModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void OnGet()
        {
        }
    }
}
