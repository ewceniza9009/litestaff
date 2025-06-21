using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.MstUser.Commands
{
    public class DeleteUser : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteUserHandler : IRequestHandler<DeleteUser, int>
        {
            private readonly HRISContext _context;
            public DeleteUserHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteUser command, CancellationToken cancellationToken)
            {
                var deleteUser = _context.MstUsers.Find(command.Id) ?? new Data.Models.MstUser();

                _context.MstUsers.Remove(deleteUser);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
