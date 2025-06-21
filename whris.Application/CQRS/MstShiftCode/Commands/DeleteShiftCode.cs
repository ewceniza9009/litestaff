using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.MstShiftCode.Commands
{
    public class DeleteShiftCode : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteShiftCodeHandler : IRequestHandler<DeleteShiftCode, int>
        {
            private readonly HRISContext _context;
            public DeleteShiftCodeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteShiftCode command, CancellationToken cancellationToken)
            {
                var deleteShiftCode = _context.MstShiftCodes.Find(command.Id) ?? new Data.Models.MstShiftCode();

                _context.MstShiftCodes.Remove(deleteShiftCode);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
