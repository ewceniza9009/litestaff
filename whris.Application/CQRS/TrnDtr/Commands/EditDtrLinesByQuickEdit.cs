using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class EditDtrLinesByQuickEdit : IRequest<int>
    {
        public int DTRId { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime? TimeIn1 { get; set; }
        public DateTime? TimeOut1 { get; set; }
        public DateTime? TimeIn2 { get; set; }
        public DateTime? TimeOut2 { get; set; }

        public class AddDtrLinesHandler : IRequestHandler<EditDtrLinesByQuickEdit, int>
        {
            private readonly HRISContext _context;
            public AddDtrLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(EditDtrLinesByQuickEdit command, CancellationToken cancellationToken)
            {
                var dtr = _context.TrnDtrs
                    .Include(x => x.TrnDtrlines)
                    .FirstOrDefault(x => x.Id == command.DTRId) ?? new Data.Models.TrnDtr();

                DTR.QuickEditLines(dtr, command, _context);

                _context.SaveChanges();

                return await Task.Run(() => 0);
            }
        }
    }
}
