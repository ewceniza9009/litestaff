using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLoan.Commands
{
    public class DeleteLoan : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteLoanHandler : IRequestHandler<DeleteLoan, int>
        {
            private readonly HRISContext _context;
            public DeleteLoanHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteLoan command, CancellationToken cancellationToken)
            {
                var deleteLoan = _context.MstEmployeeLoans.Find(command.Id) ?? new Data.Models.MstEmployeeLoan();

                _context.MstEmployeeLoans.Remove(deleteLoan);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
