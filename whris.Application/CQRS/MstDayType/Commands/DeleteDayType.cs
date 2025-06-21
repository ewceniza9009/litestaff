using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.MstDayType.Commands
{
    public class DeleteDayType : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteDayTypeHandler : IRequestHandler<DeleteDayType, int>
        {
            private readonly HRISContext _context;
            public DeleteDayTypeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteDayType command, CancellationToken cancellationToken)
            {
                var deleteDayType = _context.MstDayTypes.Find(command.Id) ?? new Data.Models.MstDayType();

                _context.MstDayTypes.Remove(deleteDayType);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
