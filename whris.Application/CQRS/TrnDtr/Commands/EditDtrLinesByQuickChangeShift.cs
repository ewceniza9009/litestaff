using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class EditDtrLinesByQuickChange : IRequest<int>
    {
        public int DTRId { get; set; }

        public class AddDtrLinesHandler : IRequestHandler<EditDtrLinesByQuickChange, int>
        {
            private readonly HRISContext _context;
            public AddDtrLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(EditDtrLinesByQuickChange command, CancellationToken cancellationToken)
            {
                var dtr = _context.TrnDtrs
                    .Include(x => x.TrnDtrlines)
                    .FirstOrDefault(x => x.Id == command.DTRId);

                DTR.QuickChangeLines(dtr);

                _context.SaveChanges();

                return await Task.Run(() => 0);
            }
        }
    }
}
