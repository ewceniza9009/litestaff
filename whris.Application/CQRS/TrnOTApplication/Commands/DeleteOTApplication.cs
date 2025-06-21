using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Commands
{
    public class DeleteOTApplication : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteOTApplicationHandler : IRequestHandler<DeleteOTApplication, int>
        {
            private readonly HRISContext _context;
            public DeleteOTApplicationHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteOTApplication command, CancellationToken cancellationToken)
            {
                var deleteOTApplication = _context.TrnOverTimes.Find(command.Id) ?? new Data.Models.TrnOverTime();

                _context.TrnOverTimes.Remove(deleteOTApplication);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
