using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.MstOtherDeduction.Commands;
using whris.Application.CQRS.MstOtherDeduction.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstOtherDeduction
{
    [Authorize]
    [Secure("MstOtherDeduction")]
    public class IndexModel : PageModel
    {
        private IMediator _mediator;

        public List<MstOtherDeductionDto> OtherDeductions { get; set; } = new List<MstOtherDeductionDto>();

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            var otherDeductions = new GetMstOtherDeductions();

            OtherDeductions = await _mediator.Send(otherDeductions);

            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostSave(List<MstOtherDeductionDto> otherDeductions)
        {
            var saveOtherDeduction = new SaveMstOtherDeduction()
            {
                MstOtherDeductions = otherDeductions
            };

            var result = await _mediator.Send(saveOtherDeduction);

            return new JsonResult(new { Id = 0 });
        }

        public async Task<IActionResult> OnGetAccounts()
        {
            var result = Common.GetGLAccounts().Value;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
