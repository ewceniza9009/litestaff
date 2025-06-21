using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using whris.Application.CQRS.TrnDtr.Queries;

namespace whris.UI.Pages.TrnDtr
{
    public class DtrController : Controller
    {
        private IMediator _mediator;

        public DtrController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> ReadDtrLines(int Id, [DataSourceRequest] DataSourceRequest request)
        {
            var dtrLines = new GetTrnDtrLinesByDtrId()
            {
                Id = Id,
                Request = request
            };

            var result = await _mediator.Send(dtrLines);

            return new JsonResult(result);
        }
    }
}
