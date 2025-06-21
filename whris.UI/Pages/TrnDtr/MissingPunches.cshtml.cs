using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.TrnDtrMissingPunches.Queries;
using whris.Application.Queries.TrnDtr;

namespace whris.UI.Pages.TrnDtr
{
    public class MissingPunchesModel : PageModel
    {
        private IMediator _mediator;

        public int Id { get; set; }
        public string? DtrNumber { get; set; }
        public string? Remarks { get; set; }

        public MissingPunchesModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void OnGet(int dtrId)
        {
            Id = dtrId;
            DtrNumber = Lookup.GetDTRNumberById(dtrId);
            Remarks = Lookup.GetDTRRemarksById(dtrId);
        }

        public async Task<IActionResult> OnPostGetMissingPunches([DataSourceRequest] DataSourceRequest request, int dtrId)
        {
            var missingPunches = new GetTrnDtrMissingPunchesById()
            {
                Request = request,
                Id = dtrId
            };

            return new JsonResult(await _mediator.Send(missingPunches));
        }
    }
}
