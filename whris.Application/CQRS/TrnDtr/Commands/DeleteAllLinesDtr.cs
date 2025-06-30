using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class DeleteAllLinesDtr : IRequest<int>
    {
        public int DtrHeaderId { get; set; }

        public class DeleteAllLinesDtrHandler : IRequestHandler<DeleteAllLinesDtr, int>
        {
            private readonly HRISContext _context;

            public DeleteAllLinesDtrHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(DeleteAllLinesDtr command, CancellationToken cancellationToken)
            {
                Console.WriteLine($" Handler hit");

                var deleteAllLines = await _context.TrnDtrlines
                .Where(l => l.Dtrid == command.DtrHeaderId)
                .ToListAsync(cancellationToken);


                if (deleteAllLines.Count == 0)
                    return 0;

                _context.TrnDtrlines.RemoveRange(deleteAllLines);
                //await _context.SaveChangesAsync(cancellationToken);
                _context.SaveChanges();

                return deleteAllLines.Count;
            }
        }
    }
}

