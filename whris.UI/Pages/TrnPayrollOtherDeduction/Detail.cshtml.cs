using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.TrnOtherDeductionApplication.Commands;
using whris.Application.CQRS.TrnPayrollOtherDeduction.Commands;
using whris.Application.CQRS.TrnPayrollOtherDeduction.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayrollOtherDeduction
{
    [Authorize]
    [Secure("TrnPayrollOtherDeduction")]
    public class DetailModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private IMediator _mediator;

        public TrnPayrollOtherDeductionDetailDto PayrollOtherDeductionDetail { get; set; } = new TrnPayrollOtherDeductionDetailDto();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;

        public DetailModel(IWebHostEnvironment environment, IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var pod = new GetTrnPayrollOtherDeductionById()
            {
                Id = Id
            };

            PayrollOtherDeductionDetail = await _mediator.Send(pod);
        }

        public async Task OnPostAdd(int payrollGroupId)
        {
            var aspUserId = string.Empty;

            if (User.Claims.Count() > 0)
            {
                aspUserId = User.Claims.ToList()[0].Value;
            }

            var addPOD = new AddPayrollOtherDeduction()
            {
                AspUserId = aspUserId,
                PayrollGroupId = payrollGroupId
            };

            PayrollOtherDeductionDetail = await _mediator.Send(addPOD);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deletePOD = new DeletePayrollOtherDeduction()
            {
                Id = id
            };

            await _mediator.Send(deletePOD);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnPayrollOtherDeductionDetailDto pod)
        {
            var savePOD = new SavePayrollOtherDeduction()
            {
                PayrollOtherDeduction = pod
            };

            var resultId = await _mediator.Send(savePOD);

            //_ = new UpdatePayrollOtherDeductionLoanBalance()
            //{
            //    PayrollOtherDeductionId = resultId
            //};

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostAddPayrollOtherDeductionLine(int PODId)
        {
            var addPodLine = new AddPayrollOtherDeductionLine()
            {
                PayrollOtherDeductionId = PODId
            };

            return new JsonResult(await _mediator.Send(addPodLine));
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getPOD = new GetTrnPayrollOtherDeductionIdByTurnPage()
            {
                Id = id,
                PayrollGroupId = payrollGroupId,
                Action = action
            };

            var dtrId = await _mediator.Send(getPOD);

            return new JsonResult(new { Id = dtrId });
        }

        public async Task<IActionResult> OnPostLoans(int podId, int payrollGroupId, int? loanNumber)
        {
            var addPodLines = new AddPayrollOtherDeductionsByLoans()
            {
                PODId = podId,
                PayrollGroupId = payrollGroupId,
                LoanNumber = loanNumber
            };

            var statusCode = await _mediator.Send(addPodLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostQuickEncode(int podId, int payrollGroupId, int? employeeId, List<TmpOtherDeduction> tmpOtherDeductions)
        {
            var addPodLines = new AddPayrollOtherDeductionsByQuickEncode()
            {
                PODId = podId,
                PayrollGroupId = payrollGroupId,
                EmployeeId = employeeId,
                SelectedOtherDeductions = tmpOtherDeductions
            };

            var statusCode = await _mediator.Send(addPodLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostImportOtherDeduction()
        {
            IFormFile? file = Request.Form.Files[0];
            var strId = Request?.Form["Id"][0]?.ToString();

            var tmpOtherDeductionImports = new List<TmpImportOtherDeduction>();

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
                    tmpOtherDeductionImports = FileUtil.ProcessOtherDeductionImports(filePath, extension);
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

            var imports = new ImportOtherDeduction()
            {
                Id = int.Parse(strId ?? "0"),
                TmpOtherDeductionImports = tmpOtherDeductionImports
            };

            _ = await _mediator.Send(imports);

            return new JsonResult("Ok");
        }

        public async Task<IActionResult> OnGetEmployees(int payrollGroupId)
        {
            var result = Common.GetEmployees(payrollGroupId).Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetEmployees2()
        {
            var result = Common.GetEmployees().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetDeductions()
        {
            var result = Common.GetDeductions().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetLoans(int employeeId)
        {
            var result = Common.GetLoans(employeeId).Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetLoans2()
        {
            var result = Common.GetLoans().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public IActionResult OnGetLoanText(int loanId)
        {
            var result = Lookup.GetLoanDateTextByLoanId(loanId);

            return new JsonResult(result);
        }
    }
}
