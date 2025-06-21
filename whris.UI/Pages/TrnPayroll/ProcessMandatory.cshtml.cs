using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayroll
{
    public class ProcessMandatoryModel : PageModel
    {
        private IMediator _mediator;

        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public ProcessMandatoryModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void OnGet()
        {
        }
    }
}
