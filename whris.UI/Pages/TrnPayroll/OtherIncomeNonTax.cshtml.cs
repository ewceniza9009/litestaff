using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.TrnPayroll.Queries;
using whris.Application.Dtos;
using whris.Data.Models;

namespace whris.UI.Pages.TrnPayroll
{
    public class OtherIncomeNonTaxModel : PageModel
    {
        private IMediator _mediator;

        public string? PayrollNo { get; set; }
        public string? Employee { get; set; }
        public string? OtherIncome { get; set; }

        public TrnPayrollLine PayrollLineDetail { get; set; }
        public List<TrnPayrollOtherIncomeLineDto> PayrollOtherIncomeLines { get; set; }

        public OtherIncomeNonTaxModel(IMediator mediator)
        {
            _mediator = mediator;

            PayrollLineDetail = new TrnPayrollLine();
            PayrollOtherIncomeLines = new List<TrnPayrollOtherIncomeLineDto>();
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
            OtherIncome = Lookup.GetPayrollOINumberById(payroll?.PayrollOtherIncomeId ?? 0);

            var otherIncomes = new GetPayrollOtherIncomesByPOIId()
            {
                POIId = payroll?.PayrollOtherIncomeId ?? 0,
                EmployeeId = PayrollLineDetail.EmployeeId,
                Taxable = false,
            };

            PayrollOtherIncomeLines = await _mediator.Send(otherIncomes);

            return await Task.Run(() => Page());
        }
    }
}
