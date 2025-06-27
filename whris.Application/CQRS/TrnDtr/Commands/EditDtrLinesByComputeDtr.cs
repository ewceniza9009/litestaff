using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.CQRS.MstEmployee.Queries;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class EditDtrLinesByComputeDtr : IRequest<int>
    {
        public int DTRId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public class AddDtrLinesHandler : IRequestHandler<EditDtrLinesByComputeDtr, int>
        {
            private readonly HRISContext _context;
            public AddDtrLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(EditDtrLinesByComputeDtr command, CancellationToken cancellationToken)
            {
                var dtr = _context.TrnDtrs
                    .Include(x => x.TrnDtrlines)
                    .FirstOrDefault(x => x.Id == command.DTRId);

                DTR.ComputeDtrLines(dtr, command, _context);

                var toBeDeletedLines = dtr.TrnDtrlines.Where(x =>
                        (DateOnly.FromDateTime(x.Date) == DateOnly.FromDateTime(command.DateStart) ||
                        DateOnly.FromDateTime(x.Date) == DateOnly.FromDateTime(command.DateEnd)));

                _context.TrnDtrlines.RemoveRange(toBeDeletedLines);
                _context.SaveChanges();

                return await Task.Run(() => 0);
            }
        }
    }
}
