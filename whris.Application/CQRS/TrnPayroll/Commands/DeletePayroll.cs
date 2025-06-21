using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class DeletePayroll : IRequest<int>
    {
        public int Id { get; set; }

        public class DeletePayrollHandler : IRequestHandler<DeletePayroll, int>
        {
            private readonly HRISContext _context;
            public DeletePayrollHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeletePayroll command, CancellationToken cancellationToken)
            {
                var deletePayroll = _context.TrnPayrolls.Find(command.Id) ?? new Data.Models.TrnPayroll();

                _context.TrnPayrolls.Remove(deletePayroll);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
