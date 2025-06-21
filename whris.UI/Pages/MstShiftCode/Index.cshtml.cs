using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstShiftCode.Queries;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstShiftCode
{
    [Authorize]
    [Secure("MstShiftCode")]
    public class IndexModel : PageModel
    {
        private IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnGetShiftCodeDetail(int id)
        {
            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostReadShiftCodeList([DataSourceRequest] DataSourceRequest request, string search)
        {
            var allShiftCodes = new GetMstShiftCodesBySearch()
            {
                Request = request,
                Search = search
            };

            return new JsonResult(await _mediator.Send(allShiftCodes));
        }
    }
}
