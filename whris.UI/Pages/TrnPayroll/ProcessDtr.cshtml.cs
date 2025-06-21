using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayroll
{
    public class ProcessDtrModel : PageModel
    {
        private IMediator _mediator;

        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public ProcessDtrModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void OnGet()
        {
        }
    }
}
