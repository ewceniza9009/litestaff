using Azure.Core;
using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class EditPayrollLinesByOtherDeduction : IRequest<int>
    {
        public int PayrollId { get; set; }
        public int PayrollOtherDeductionId { get; set; }
        public int? EmployeeId { get; set; }

        public class EditPayrollLinesByOtherDeductionHandler : IRequestHandler<EditPayrollLinesByOtherDeduction, int>
        {
            private readonly HRISContext _context;
            public EditPayrollLinesByOtherDeductionHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(EditPayrollLinesByOtherDeduction command, CancellationToken cancellationToken)
            {
                await Payroll.ProcessPayrollOtherDeduction(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
