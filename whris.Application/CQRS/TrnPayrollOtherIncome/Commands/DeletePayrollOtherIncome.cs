using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Commands
{
    public class DeletePayrollOtherIncome : IRequest<int>
    {
        public int Id { get; set; }

        public class DeletePayrollOtherIncomeHandler : IRequestHandler<DeletePayrollOtherIncome, int>
        {
            private readonly HRISContext _context;
            public DeletePayrollOtherIncomeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeletePayrollOtherIncome command, CancellationToken cancellationToken)
            {
                var deletePayrollOtherIncome = _context.TrnPayrollOtherIncomes.Find(command.Id) ?? new Data.Models.TrnPayrollOtherIncome();

                _context.TrnPayrollOtherIncomes.Remove(deletePayrollOtherIncome);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
