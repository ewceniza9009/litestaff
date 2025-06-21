using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using whris.Application.Common;
using whris.Application.CQRS.MstMandatoryDeductionTable.Commands;
using whris.Application.CQRS.MstMandatoryDeductionTable.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstMandatoryDeductionTable
{
    [Authorize]
    [Secure("MstMandatoryDeductionTable")]
    public class IndexModel : PageModel
    {
        private IMediator _mediator;

        public MstMandatoryDeductionTableDto DeductionTables { get; set; } = new MstMandatoryDeductionTableDto();

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            var deductionTables = new GetMstMandatoryTaxTables();

            DeductionTables = await _mediator.Send(deductionTables);

            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostSave(MstMandatoryDeductionTableDto mandatoryTaxTables)
        {
            var monthliesString = Request.Form["monthlies"].ToString() ?? "";
            var monthlies = JsonSerializer.Deserialize<List<MstTableWtaxMonthlyDto>>(monthliesString);

            mandatoryTaxTables.MstTableWtaxMonthlies = monthlies ?? new List<MstTableWtaxMonthlyDto>();

            var saveMandatoryTaxTables = new SaveMandatoryDeductionTable()
            {
                MandatoryDeductionTable = mandatoryTaxTables
            };

            var result = await _mediator.Send(saveMandatoryTaxTables);

            return new JsonResult(new { Id = result });
        }

        public async Task<IActionResult> OnGetTaxCodes()
        {
            var result = Common.GetTaxCodes().Value;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
