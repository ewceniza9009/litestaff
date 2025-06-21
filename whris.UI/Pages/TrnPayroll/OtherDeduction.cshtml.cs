using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.TrnPayroll.Queries;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Models;

namespace whris.UI.Pages.TrnPayroll
{
    public class OtherDeductionModel : PageModel
    {
        private IMediator _mediator;

        public string? PayrollNo { get; set; }
        public string? Employee { get; set; }
        public string? OtherDeduction { get; set; }

        public TrnPayrollLine PayrollLineDetail { get; set; }
        public List<TrnPayrollOtherDeductionLineDto> PayrollOtherDeductionLines { get; set; }

        public OtherDeductionModel(IMediator mediator)
        {
            _mediator = mediator;

            PayrollLineDetail = new TrnPayrollLine();
            PayrollOtherDeductionLines = new List<TrnPayrollOtherDeductionLineDto>();
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var payrollLine = new GetTrnPayrollLineByLineId()
            {
                Id = id,
            };

            PayrollLineDetail = await _mediator.Send(payrollLine);

            var payroll = Lookup.GetPayrollById(PayrollLineDetail.PayrollId);

            PayrollNo = Lookup.GetPayrollNoById(PayrollLineDetail.PayrollId);
            Employee = Lookup.GetEmployeeNameById(PayrollLineDetail.EmployeeId);
            OtherDeduction = Lookup.GetPayrollOINumberById(payroll?.PayrollOtherDeductionId ?? 0);

            var otherDeductions = new GetPayrollOtherDeductionsByPODId()
            {
                PODId = payroll?.PayrollOtherDeductionId ?? 0,
                EmployeeId = PayrollLineDetail.EmployeeId,
                Taxable = true,
            };

            PayrollOtherDeductionLines = await _mediator.Send(otherDeductions);

            return await Task.Run(() => Page());
        }
    }
}
