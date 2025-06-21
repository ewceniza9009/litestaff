using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using whris.Application.Common;
using whris.Application.CQRS.TrnChangeShiftCode.Commands;
using whris.Application.CQRS.TrnChangeShiftCode.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnChangeShiftCode
{
    [Authorize]
    [Secure("TrnChangeShift")]
    public class DetailModel : PageModel
    {
        private IMediator _mediator;

        public TrnChangeShiftCodeDetailDto ChangeShiftCodeDetail { get; set; } = new TrnChangeShiftCodeDetailDto();
        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public DetailModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var changeShift = new GetTrnChangeShiftCodeById()
            {
                Id = Id
            };

            ChangeShiftCodeDetail = await _mediator.Send(changeShift);
        }

        public async Task OnPostAdd(int payrollGroupId)
        {
            var aspUserId = string.Empty;

            if (User.Claims.Count() > 0)
            {
                aspUserId = User.Claims.ToList()[0].Value;
            }

            var addCS = new AddChangeShiftCode()
            {
                AspUserId = aspUserId,
                PayrollGroupId = payrollGroupId
            };

            ChangeShiftCodeDetail = await _mediator.Send(addCS);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteCS = new DeleteChangeShiftCode()
            {
                Id = id
            };

            await _mediator.Send(deleteCS);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnChangeShiftCodeDetailDto changeShift)
        {
            var saveCS = new SaveChangeShiftCode()
            {
                ChangeShiftCode = changeShift
            };

            var resultId = await _mediator.Send(saveCS);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostAddChangeShiftCodeLine(int CSId)
        {
            var addLaLine = new AddChangeShiftCodeLine()
            {
                ChangeShiftCodeId = CSId
            };

            return new JsonResult(await _mediator.Send(addLaLine));
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getCS = new GetTrnChangeShiftCodeIdByTurnPage()
            {
                Id = id,
                PayrollGroupId = payrollGroupId,
                Action = action
            };

            var csId = await _mediator.Send(getCS);

            return new JsonResult(new { Id = csId });
        }

        public async Task<IActionResult> OnPostQuickEncode(int csId,
            int payrollGroupId,
            DateTime dateStart,
            DateTime dateEnd,
            int? employeeId,
            int shiftId)
        {
            var addCsLines = new AddChangeShiftCodesByQuickEncode()
            {
                CSId = csId,
                PayrollGroupId = payrollGroupId,
                DateStart = dateStart,
                DateEnd = dateEnd,
                EmployeeId = employeeId,
                ShiftCodeId = shiftId
            };

            var statusCode = await _mediator.Send(addCsLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnGetEmployees(int payrollGroupId)
        {
            var result = Common.GetEmployees().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetShiftCodes()
        {
            var result = ComboboxDatasources.ShiftCodeCmbDs;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
