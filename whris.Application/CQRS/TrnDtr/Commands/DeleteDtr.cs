using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class DeleteDtr : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteDtrHandler : IRequestHandler<DeleteDtr, int>
        {
            private readonly HRISContext _context;
            public DeleteDtrHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteDtr command, CancellationToken cancellationToken)
            {
                var deleteDtr = _context.TrnDtrs.Find(command.Id) ?? new Data.Models.TrnDtr();

                _context.TrnDtrs.Remove(deleteDtr);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
