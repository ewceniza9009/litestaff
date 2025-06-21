using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstShiftCode.Commands;
using whris.Application.CQRS.MstShiftCode.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.MstShiftCode
{
    [Authorize]
    [Secure("MstShiftCode")]
    public class DetailModel : PageModel
    {
        private IMediator _mediator;

        public MstShiftCodeDetailDto ShiftCodeDetail { get; set; } = new MstShiftCodeDetailDto();
        public MstShiftCodeComboboxDatasources ComboboxDataSources = MstShiftCodeComboboxDatasources.Instance;

        public DetailModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var user = new GetMstShiftCodeById()
            {
                Id = Id
            };

            ShiftCodeDetail = await _mediator.Send(user);
        }

        public async Task OnGetAdd()
        {
            var addShiftCode = new AddShiftCode();

            ShiftCodeDetail = await _mediator.Send(addShiftCode);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteShiftCode = new DeleteShiftCode()
            {
                Id = id
            };

            await _mediator.Send(deleteShiftCode);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(MstShiftCodeDetailDto user)
        {
            var saveShiftCode = new SaveShiftCode()
            {
                ShiftCode = user
            };

            var resultId = await _mediator.Send(saveShiftCode);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostTurnPage(int id, string action)
        {
            var getShiftCode = new GetMstShiftCodeIdByTurnPage()
            {
                Id = id,
                Action = action
            };

            var userId = await _mediator.Send(getShiftCode);

            return new JsonResult(new { Id = userId });
        }

        public async Task<IActionResult> OnPostAddShiftCodeDay(int userId)
        {
            var getShiftCodeForm = new AddShiftCodeDay()
            {
                ShiftCodeId = userId
            };

            return new JsonResult(await _mediator.Send(getShiftCodeForm));
        }

        public async Task<IActionResult> OnGetDays()
        {
            var result = ComboboxDataSources.Days;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
