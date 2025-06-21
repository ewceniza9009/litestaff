using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Commands
{
    public class DeleteLeaveApplication : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteLeaveApplicationHandler : IRequestHandler<DeleteLeaveApplication, int>
        {
            private readonly HRISContext _context;
            public DeleteLeaveApplicationHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteLeaveApplication command, CancellationToken cancellationToken)
            {
                var deleteLeaveApplication = _context.TrnLeaveApplications.Find(command.Id) ?? new Data.Models.TrnLeaveApplication();

                _context.TrnLeaveApplications.Remove(deleteLeaveApplication);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
