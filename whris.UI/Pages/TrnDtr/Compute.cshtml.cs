using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Dtos;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnDtr
{
    public class ComputeModel : PageModel
    {
        private IMediator _mediator;

        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public ComputeModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void OnGet(int payrollGroupId)
        {
            ComboboxDatasources.EmployeeCmbDs = ComboboxDatasources.EmployeeCmbDs
                .Where(x => x.PayrollGroupId == payrollGroupId)
                .ToList();

        }
    }
}
