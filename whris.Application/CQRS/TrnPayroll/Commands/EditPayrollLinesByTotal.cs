using Azure.Core;
using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class EditPayrollLinesByTotals : IRequest<int>
    {
        public int PayrollId { get; set; }
        public int? EmployeeId { get; set; }

        public class EditPayrollLinesByTotalsHandler : IRequestHandler<EditPayrollLinesByTotals, int>
        {
            private readonly HRISContext _context;
            public EditPayrollLinesByTotalsHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(EditPayrollLinesByTotals command, CancellationToken cancellationToken)
            {
                await Payroll.ProcessTotals(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
