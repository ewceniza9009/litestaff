using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.TrnPayroll.Queries;
using whris.Data.Models;

namespace whris.UI.Pages.TrnPayroll
{
    public class ExpandSalaryModel : PageModel
    {
        private IMediator _mediator;

        public string? PayrollNo { get; set; }
        public string? Employee { get; set; }
        public string? PayrollType { get; set; }

        public TrnPayrollLine PayrollLineDetail { get; set; }

        public ExpandSalaryModel(IMediator mediator)
        {
            _mediator = mediator;

            PayrollLineDetail = new TrnPayrollLine();
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var payrollLine = new GetTrnPayrollLineByLineId()
            {
                Id = id,
            };

            PayrollLineDetail = await _mediator.Send(payrollLine);

            PayrollNo = Lookup.GetPayrollNoById(PayrollLineDetail.PayrollId);
            Employee = Lookup.GetEmployeeNameById(PayrollLineDetail.EmployeeId);
            PayrollType = Lookup.GetPayrollTypeById(PayrollLineDetail.PayrollTypeId);

            return await Task.Run(() => Page());
        }
    }
}
