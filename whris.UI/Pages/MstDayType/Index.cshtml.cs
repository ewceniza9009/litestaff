using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstDayType.Queries;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstDayType
{
    [Authorize]
    [Secure("MstDayType")]
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

        public async Task<IActionResult> OnPostReadDayTypeList([DataSourceRequest] DataSourceRequest request, string search)
        {
            var allDayTypes = new GetMstDayTypesBySearch()
            {
                Request = request,
                Search = search
            };

            return new JsonResult(await _mediator.Send(allDayTypes));
        }
    }
}
