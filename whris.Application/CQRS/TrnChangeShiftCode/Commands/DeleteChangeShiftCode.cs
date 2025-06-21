using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Commands
{
    public class DeleteChangeShiftCode : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteChangeShiftCodeHandler : IRequestHandler<DeleteChangeShiftCode, int>
        {
            private readonly HRISContext _context;
            public DeleteChangeShiftCodeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteChangeShiftCode command, CancellationToken cancellationToken)
            {
                var deleteChangeShiftCode = _context.TrnChangeShifts.Find(command.Id) ?? new Data.Models.TrnChangeShift();

                _context.TrnChangeShifts.Remove(deleteChangeShiftCode);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
