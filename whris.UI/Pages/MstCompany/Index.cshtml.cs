using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstCompany.Queries;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstCompany
{
    [Authorize]
    [Secure("MstCompany")]
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

        public async Task<IActionResult> OnPostReadCompanyList([DataSourceRequest] DataSourceRequest request, string search)
        {
            var allUsers = new GetMstCompaniesBySearch()
            {
                Request = request,
                Search = search
            };

            return new JsonResult(await _mediator.Send(allUsers));
        }
    }
}
