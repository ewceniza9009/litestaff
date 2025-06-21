using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.TrnLeaveApplication.Commands;
using whris.Application.CQRS.TrnLeaveApplication.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnLeaveApplication
{
    [Authorize]
    [Secure("TrnLeaveApplication")]
    public class DetailModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private IMediator _mediator;

        public TrnLeaveApplicationDetailDto LeaveApplicationDetail { get; set; } = new TrnLeaveApplicationDetailDto();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;

        public DetailModel(IWebHostEnvironment environment, IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var leaveApp = new GetTrnLeaveApplicationById()
            {
                Id = Id
            };

            LeaveApplicationDetail = await _mediator.Send(leaveApp);
        }

        public async Task OnPostAdd(int payrollGroupId)
        {
            var aspUserId = string.Empty;

            if (User.Claims.Count() > 0)
            {
                aspUserId = User.Claims.ToList()[0].Value;
            }

            var addLA = new AddLeaveApplication()
            {
                AspUserId = aspUserId,
                PayrollGroupId = payrollGroupId
            };

            LeaveApplicationDetail = await _mediator.Send(addLA);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteLA = new DeleteLeaveApplication()
            {
                Id = id
            };

            await _mediator.Send(deleteLA);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnLeaveApplicationDetailDto leaveApp)
        {
            var saveLA = new SaveLeaveApplication()
            {
                LeaveApplication = leaveApp
            };

            var resultId = await _mediator.Send(saveLA);

            var updateLeaveLedger = new UpdateLeaveApplicationLeaveLedger()
            {
                LeaveApplicationId = resultId
            };

            _ = await _mediator.Send(updateLeaveLedger);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostAddLeaveApplicationLine(int LAId)
        {
            var addLaLine = new AddLeaveApplicationLine()
            {
                LeaveApplicationId = LAId
            };

            return new JsonResult(await _mediator.Send(addLaLine));
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getLA = new GetTrnLeaveApplicationIdByTurnPage()
            {
                Id = id,
                PayrollGroupId = payrollGroupId,
                Action = action
            };

            var laId = await _mediator.Send(getLA);

            return new JsonResult(new { Id = laId });
        }

        public async Task<IActionResult> OnPostQuickEncode(int laId, 
            int payrollGroupId, 
            DateTime dateStart, 
            DateTime dateEnd,
            int? employeeId,
            int leaveId,
            decimal numberOfHours,
            bool withPay, 
            bool debitToLedger, 
            string remarks)
        {
            var addLaLines = new AddLeaveApplicationsByQuickEncode()
            {
                LAId = laId,
                PayrollGroupId = payrollGroupId,
                DateStart = dateStart,
                DateEnd = dateEnd,
                EmployeeId = employeeId,
                LeaveId = leaveId,
                NumberOfHours = numberOfHours,
                WithPay = withPay,
                DebitToLedger = debitToLedger,
                Remarks = remarks,
            };

            var statusCode = await _mediator.Send(addLaLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostImportLeave()
        {
            IFormFile? file = Request.Form.Files[0];
            var strId = Request?.Form["Id"][0]?.ToString();

            var tmpLeaveImports = new List<TmpImportLeave>();

            if (file is not null && file.Length > 0)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "Uploads", file.FileName);
                var extension = Path.GetExtension(filePath)?.ToLower();
                string[] validFileTypes = { ".xls", ".xlsx", ".csv", ".txt" };

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (validFileTypes.Contains(extension))
                {
                    tmpLeaveImports = FileUtil.ProcessLeaveImports(filePath, extension);
                }
                else
                {
                    return new BadRequestObjectResult("File format is incorrect");
                }
            }
            else
            {
                return new BadRequestObjectResult("No file was uploaded.");
            }

            var imports = new ImportLeave()
            {
                Id = int.Parse(strId ?? "0"),
                TmpLeaveImports = tmpLeaveImports
            };

            _ = await _mediator.Send(imports);

            return new JsonResult("Ok");
        }

        public async Task<IActionResult> OnGetEmployees(int payrollGroupId)
        {
            var result = Common.GetEmployees().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetLeaves()
        {
            var result = Common.GetSetupLeaves().Value;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
