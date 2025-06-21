using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstEmployee.Commands;
using whris.Application.Dtos;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.MstEmployee
{
    [Authorize]
    public class MemoModel : PageModel
    {
        private readonly IMediator _mediator;

        public MstEmployeeMemoDto EmployeeMemo { get; set; } = new MstEmployeeMemoDto();
        public MstEmployeeComboboxDatasources ComboboxDatasources = MstEmployeeComboboxDatasources.Instance;

        public MemoModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnPostAdd(int employeeId)
        {
            var addEmployeeMemo = new AddEmployeeMemo()
            {
                EmployeeId = employeeId
            };

            return new JsonResult(await _mediator.Send(addEmployeeMemo));
        }

        public async Task OnPostLoad(MstEmployeeMemoDto memo)
        {
            await Task.Run(() => EmployeeMemo = memo);
        }
    }
}
