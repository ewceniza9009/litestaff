using Azure.Core;
using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class EditPayrollLinesByOtherIncome : IRequest<int>
    {
        public int PayrollId { get; set; }
        public int PayrollOtherIncomeId { get; set; }
        public int? EmployeeId { get; set; }

        public class EditPayrollLinesByOtherIncomeHandler : IRequestHandler<EditPayrollLinesByOtherIncome, int>
        {
            private readonly HRISContext _context;
            public EditPayrollLinesByOtherIncomeHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(EditPayrollLinesByOtherIncome command, CancellationToken cancellationToken)
            {
                await Payroll.ProcessPayrollOtherIncome(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
