using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstUser.Queries;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstUser
{
    [Authorize]
    [Secure("MstUser")]
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

        public async Task<IActionResult> OnPostReadUserList([DataSourceRequest] DataSourceRequest request, string search)
        {
            var allUsers = new GetMstUsersBySearch()
            {
                Request = request,
                Search = search
            };

            return new JsonResult(await _mediator.Send(allUsers));
        }
    }
}
