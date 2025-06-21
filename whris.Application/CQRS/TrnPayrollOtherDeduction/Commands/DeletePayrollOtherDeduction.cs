using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Commands
{
    public class DeletePayrollOtherDeduction : IRequest<int>
    {
        public int Id { get; set; }

        public class DeletePayrollOtherDeductionHandler : IRequestHandler<DeletePayrollOtherDeduction, int>
        {
            private readonly HRISContext _context;
            public DeletePayrollOtherDeductionHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeletePayrollOtherDeduction command, CancellationToken cancellationToken)
            {
                var deletePayrollOtherDeduction = _context.TrnPayrollOtherDeductions.Find(command.Id) ?? new Data.Models.TrnPayrollOtherDeduction();

                _context.TrnPayrollOtherDeductions.Remove(deletePayrollOtherDeduction);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
