using Azure.Core;
using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class EditPayrollLinesByWithholding : IRequest<int>
    {
        public int PayrollId { get; set; }
        public int? EmployeeId { get; set; }

        public class EditPayrollLinesByWithholdingHandler : IRequestHandler<EditPayrollLinesByWithholding, int>
        {
            private readonly HRISContext _context;
            public EditPayrollLinesByWithholdingHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(EditPayrollLinesByWithholding command, CancellationToken cancellationToken)
            {
                await Payroll.ProcessWithholdingTax(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
