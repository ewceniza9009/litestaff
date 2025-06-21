using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.TrnOtherIncomeApplication.Commands;
using whris.Application.CQRS.TrnPayrollOtherIncome.Commands;
using whris.Application.CQRS.TrnPayrollOtherIncome.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayrollOtherIncome
{
    [Authorize]
    [Secure("TrnPayrollOtherIncome")]
    public class DetailModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private IMediator _mediator;

        public TrnPayrollOtherIncomeDetailDto PayrollOtherIncomeDetail { get; set; } = new TrnPayrollOtherIncomeDetailDto();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;

        public DetailModel(IWebHostEnvironment environment, IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var poi = new GetTrnPayrollOtherIncomeById()
            {
                Id = Id
            };

            PayrollOtherIncomeDetail = await _mediator.Send(poi);
        }

        public async Task OnPostAdd(int payrollGroupId)
        {
            var aspUserId = string.Empty;

            if (User.Claims.Count() > 0)
            {
                aspUserId = User.Claims.ToList()[0].Value;
            }

            var addPOI = new AddPayrollOtherIncome()
            {
                AspUserId = aspUserId,
                PayrollGroupId = payrollGroupId
            };

            PayrollOtherIncomeDetail = await _mediator.Send(addPOI);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deletePOI = new DeletePayrollOtherIncome()
            {
                Id = id
            };

            await _mediator.Send(deletePOI);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnPayrollOtherIncomeDetailDto poi)
        {
            var savePOI = new SavePayrollOtherIncome()
            {
                PayrollOtherIncome = poi
            };

            var resultId = await _mediator.Send(savePOI);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostAddPayrollOtherIncomeLine(int POIId)
        {
            var addPoiLine = new AddPayrollOtherIncomeLine()
            {
                PayrollOtherIncomeId = POIId
            };

            return new JsonResult(await _mediator.Send(addPoiLine));
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getPOI = new GetTrnPayrollOtherIncomeIdByTurnPage()
            {
                Id = id,
                PayrollGroupId = payrollGroupId,
                Action = action
            };

            var poiId = await _mediator.Send(getPOI);

            return new JsonResult(new { Id = poiId });
        }

        public async Task<IActionResult> OnPostOI13Month(int poiId, int payrollGroupId, int otherIncomeId, int? employeeId, int? startPayNo, int? endPayNo, decimal noOfPayroll) 
        {
            var addPoiLines = new AddPayrollOtherIncomesBy13Month()
            {
                POIId = poiId,
                PayrollGroupId = payrollGroupId,
                OtherIncomeId = otherIncomeId,
                EmployeeId = employeeId,
                StartPayNo = startPayNo,
                EndPayNo = endPayNo,
                NoOfPayroll = noOfPayroll,
            };

            var statusCode = await _mediator.Send(addPoiLines);

            return new JsonResult(new { Id = 0 });
        }

        public async Task<IActionResult> OnPostQuickEncode(int poiId, int payrollGroupId, int? employeeId, List<TmpOtherIncome> tmpOtherIncomes)
        {
            var addPoiLines = new AddPayrollOtherIncomesByQuickEncode()
            {
                POIId = poiId,
                PayrollGroupId = payrollGroupId,
                EmployeeId = employeeId,
                SelectedOtherIncomes = tmpOtherIncomes
            };

            var statusCode = await _mediator.Send(addPoiLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostImportOtherIncome()
        {
            IFormFile? file = Request.Form.Files[0];
            var strId = Request?.Form["Id"][0]?.ToString();

            var tmpOtherIncomeImports = new List<TmpImportOtherIncome>();

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
                    tmpOtherIncomeImports = FileUtil.ProcessOtherIncomeImports(filePath, extension);
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

            var imports = new ImportOtherIncome()
            {
                Id = int.Parse(strId ?? "0"),
                TmpOtherIncomeImports = tmpOtherIncomeImports
            };

            _ = await _mediator.Send(imports);

            return new JsonResult("Ok");
        }


        public async Task<IActionResult> OnGetEmployees(int payrollGroupId)
        {
            var result = Common.GetEmployees().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetIncomes()
        {
            var result = Common.GetIncomes().Value;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
