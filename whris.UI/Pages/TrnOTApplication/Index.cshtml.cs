using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.TrnLeaveApplication.Queries;
using whris.Application.CQRS.TrnOTApplication.Queries;
using whris.Data.Models;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnOTApplication
{
    [Authorize]
    [Secure("TrnOverTime")]
    public class IndexModel : PageModel
    {
        private IMediator _mediator;

        public List<MstPayrollGroup> allPayrollGroups = new List<MstPayrollGroup>();

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            allPayrollGroups = TrnDtrComboboxDatasources.Instance.PayrollGroupCmbDs;

            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostReadOTList([DataSourceRequest] DataSourceRequest request, int payrollGroupId)
        {
            var allOts = new GetTrnOTApplicationsByPayrollGroupId()
            {
                Request = request,
                PayrollGroupId = payrollGroupId
            };

            return new JsonResult(await _mediator.Send(allOts));
        }
    }
}
