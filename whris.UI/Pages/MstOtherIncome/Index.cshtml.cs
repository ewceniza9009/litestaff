using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.MstMstOtherIncome.Commands;
using whris.Application.CQRS.MstOtherIncome.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstOtherIncome
{
    [Authorize]
    [Secure("MstOtherIncome")]
    public class IndexModel : PageModel
    {
        private IMediator _mediator;

        public List<MstOtherIncomeDto> OtherIncomes { get; set; } = new List<MstOtherIncomeDto>();

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            var otherIncomes = new GetMstOtherIncomes();

            OtherIncomes = await _mediator.Send(otherIncomes);

            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostSave(List<MstOtherIncomeDto> otherIncomes)
        {
            var saveOtherIncome = new SaveMstOtherIncome()
            {
                MstOtherIncomes = otherIncomes
            };

            var result = await _mediator.Send(saveOtherIncome);

            return new JsonResult(new { Id = 0 });
        }

        public async Task<IActionResult> OnGetAccounts()
        {
            var result = Common.GetGLAccounts().Value;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
