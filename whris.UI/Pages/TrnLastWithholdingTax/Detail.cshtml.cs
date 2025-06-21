using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.TrnLastWithholdingTax.Commands;
using whris.Application.CQRS.TrnLastWithholdingTax.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnLastWithholdingTax
{
    [Authorize]
    [Secure("TrnLastWithholdingTax")]
    public class DetailModel : PageModel
    {
        private IMediator _mediator;

        public TrnLastWithholdingTaxDetailDto LastWithholdingTaxDetail { get; set; } = new TrnLastWithholdingTaxDetailDto();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;

        public DetailModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var lastWithTax = new GetTrnLastWithholdingTaxById()
            {
                Id = Id
            };

            LastWithholdingTaxDetail = await _mediator.Send(lastWithTax);
        }

        public async Task OnPostAdd(int payrollGroupId)
        {
            var addLWT = new AddLastWithholdingTax()
            {
                PayrollGroupId = payrollGroupId
            };

            LastWithholdingTaxDetail = await _mediator.Send(addLWT);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteLWT = new DeleteLastWithholdingTax()
            {
                Id = id
            };

            await _mediator.Send(deleteLWT);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnLastWithholdingTaxDetailDto lastWithTax)
        {
            var saveLWT = new SaveLastWithholdingTax()
            {
                LastWithholdingTax = lastWithTax
            };

            var resultId = await _mediator.Send(saveLWT);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostAddLastWithholdingTaxLine(int LWTId)
        {
            var addLwtLine = new AddLastWithholdingTaxLine()
            {
                LastWithholdingTaxId = LWTId
            };

            return new JsonResult(await _mediator.Send(addLwtLine));
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getLWT = new GetTrnLastWithholdingTaxIdByTurnPage()
            {
                Id = id,
                PayrollGroupId = payrollGroupId,
                Action = action
            };

            var laId = await _mediator.Send(getLWT);

            return new JsonResult(new { Id = laId });
        }

        public async Task<IActionResult> OnPostProcess(int lwtId,
            int periodId,
            int payrollGroupId,
            int? employeeId)
        {
            var addLwtLines = new AddLastWithholdingTaxLinesByProcess()
            {
                LWTId = lwtId,
                PeriodId = periodId,
                PayrollGroupId = payrollGroupId,                
                EmployeeId = employeeId
            };

            var statusCode = await _mediator.Send(addLwtLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnGetEmployees(int payrollGroupId)
        {
            var result = Common.GetEmployees().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetTaxes()
        {
            var result = Common.GetTaxCodes().Value;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
