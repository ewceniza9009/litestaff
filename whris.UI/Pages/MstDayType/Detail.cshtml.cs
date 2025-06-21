using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstDayType.Commands;
using whris.Application.CQRS.MstDayType.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.MstDayType
{
    [Authorize]
    [Secure("MstDayType")]
    public class DetailModel : PageModel
    {
        private IMediator _mediator;

        public MstDayTypeDetailDto DayTypeDetail { get; set; } = new MstDayTypeDetailDto();
        public MstDayTypeComboboxDatasources ComboboxDataSources = MstDayTypeComboboxDatasources.Instance;

        public DetailModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var user = new GetMstDayTypeById()
            {
                Id = Id
            };

            DayTypeDetail = await _mediator.Send(user);
        }

        public async Task OnGetAdd()
        {
            var addDayType = new AddDayType();

            DayTypeDetail = await _mediator.Send(addDayType);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteDayType = new DeleteDayType()
            {
                Id = id
            };

            await _mediator.Send(deleteDayType);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(MstDayTypeDetailDto user)
        {
            var saveDayType = new SaveDayType()
            {
                DayType = user
            };

            var resultId = await _mediator.Send(saveDayType);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostTurnPage(int id, string action)
        {
            var getDayType = new GetMstDayTypeIdByTurnPage()
            {
                Id = id,
                Action = action
            };

            var userId = await _mediator.Send(getDayType);

            return new JsonResult(new { Id = userId });
        }

        public async Task<IActionResult> OnPostAddDayTypeDay(int userId)
        {
            var getDayTypeForm = new AddDayTypeDay()
            {
                DayTypeId = userId
            };

            return new JsonResult(await _mediator.Send(getDayTypeForm));
        }

        public async Task<IActionResult> OnGetBranches()
        {
            var result = ComboboxDataSources.Branches;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
