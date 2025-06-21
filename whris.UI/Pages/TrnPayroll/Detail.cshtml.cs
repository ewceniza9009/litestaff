using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using whris.Application.Common;
using whris.Application.CQRS.TrnDtr.Commands;
using whris.Application.CQRS.TrnPayroll.Commands;
using whris.Application.CQRS.TrnPayroll.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnPayroll
{
    [Authorize]
    [Secure("TrnPayroll")]
    public class DetailModel : PageModel
    {
        private IMediator _mediator;
        private IMemoryCache _cache;

        public TrnPayrollDetailDto PayrollDetail { get; set; } = new TrnPayrollDetailDto();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;
        public TrnPayrollComboboxDatasources PayrollComboboxDataSources = TrnPayrollComboboxDatasources.Instance;

        public DetailModel(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        public async Task OnGetAsync(int Id)
        {
            var dtr = new GetTrnPayrollById()
            {
                Id = Id
            };

            PayrollDetail = await _mediator.Send(dtr);

            PayrollComboboxDataSources.DTRCmbDs = PayrollComboboxDataSources.DTRCmbDs
               .Where(x => x.PayrollGroupId == PayrollDetail.PayrollGroupId)
               .ToList();

            PayrollComboboxDataSources.PayrollOtherIncomeCmbDs = PayrollComboboxDataSources.PayrollOtherIncomeCmbDs
               .Where(x => x.PayrollGroupId == PayrollDetail.PayrollGroupId)
               .ToList();

            PayrollComboboxDataSources.PayrollOtherDeductionCmbDs = PayrollComboboxDataSources.PayrollOtherDeductionCmbDs
               .Where(x => x.PayrollGroupId == PayrollDetail.PayrollGroupId)
               .ToList();

            PayrollComboboxDataSources.LastWithholdingTaxCmbDs = PayrollComboboxDataSources.LastWithholdingTaxCmbDs
               .Where(x => x.PayrollGroupId == PayrollDetail.PayrollGroupId)
               .ToList();
        }

        public async Task OnPostAdd(int payrollGroupId)
        {
            var aspUserId = string.Empty;

            if (User.Claims.Count() > 0)
            {
                aspUserId = User.Claims.ToList()[0].Value;
            }

            var addPayroll = new AddPayroll()
            {
                AspUserId = aspUserId,
                PayrollGroupId = payrollGroupId
            };

            PayrollDetail = await _mediator.Send(addPayroll);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deletePayroll = new DeletePayroll()
            {
                Id = id
            };

            await _mediator.Send(deletePayroll);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnPayrollDetailDto payroll)
        {
            var savePayroll = new SavePayroll()
            {
                Payroll = payroll
            };

            var resultId = await _mediator.Send(savePayroll);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostAddPayrollLine(int payrollId)
        {
            var addPayroll = new AddPayrollLine()
            {
                PayrollId = payrollId
            };

            return new JsonResult(await _mediator.Send(addPayroll));
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getPayroll = new GetTrnPayrollIdByTurnPage()
            {
                Id = id,
                PayrollGroupId = payrollGroupId,
                Action = action
            };

            var dtrId = await _mediator.Send(getPayroll);

            return new JsonResult(new { Id = dtrId });
        }

        public async Task<IActionResult> OnPostProcessDtr(int payrollId, int payrollGroupId, int dtrId, int? employeeId)
        {
            var addPayrollLines = new AddPayrollLinesByProcessDtr()
            {
                PayrollId = payrollId,
                PayrollGroupId = payrollGroupId,
                DtrId = dtrId,
                EmployeeId = employeeId
            };

            var statusCode = await _mediator.Send(addPayrollLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostProcessOtherIncome(int payrollId, int payrollOtherIncomeId, int? employeeId)
        {
            var editPayrollLines = new EditPayrollLinesByOtherIncome()
            {
                PayrollId = payrollId,
                PayrollOtherIncomeId = payrollOtherIncomeId,
                EmployeeId = employeeId
            };

            var statusCode = await _mediator.Send(editPayrollLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostProcessOtherDeduction(int payrollId, int payrollOtherDeductionId, int? employeeId)
        {
            var editPayrollLines = new EditPayrollLinesByOtherDeduction()
            {
                PayrollId = payrollId,
                PayrollOtherDeductionId = payrollOtherDeductionId,
                EmployeeId = employeeId
            };

            var statusCode = await _mediator.Send(editPayrollLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostProcessMandatory(int mandatoryType, int payrollId, int? employeeId, bool isProcessInMonth)
        {
            var editPayrollLines = new EditPayrollLinesByMandatory()
            {
                MandatoryType = mandatoryType,
                PayrollId = payrollId,
                EmployeeId = employeeId,
                IsProcessInMonth = isProcessInMonth
            };

            var statusCode = await _mediator.Send(editPayrollLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostProcessWithholding(int payrollId, int? employeeId)
        {
            var editPayrollLines = new EditPayrollLinesByWithholding()
            {
                PayrollId = payrollId,
                EmployeeId = employeeId
            };

            var statusCode = await _mediator.Send(editPayrollLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostProcessTotals(int payrollId, int? employeeId)
        {
            var editPayrollLines = new EditPayrollLinesByTotals()
            {
                PayrollId = payrollId,
                EmployeeId = employeeId
            };

            var statusCode = await _mediator.Send(editPayrollLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnGetEmployees(int payrollGroupId)
        {
            //var result = Common.GetEmployees(payrollGroupId).Value;
            var result = _cache.Get(Caching.EmployeeCmbDsCacheKey);

            if (result is null)
            {
                _cache.Set(Caching.EmployeeCmbDsCacheKey, Common.GetEmployees().Value, Caching.cacheEntryOptions);
                result = _cache.Get(Caching.EmployeeCmbDsCacheKey);
            }

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetPayrollTypes()
        {
            var result = Common.GetPayrollTypes().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetTaxCodes()
        {
            var result = Common.GetTaxCodes().Value;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
