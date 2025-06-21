using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstUser.Commands
{
    public class AddUser : IRequest<MstUserDetailDto>
    {
        public class AddUserHandler : IRequestHandler<AddUser, MstUserDetailDto>
        {
            private readonly HRISContext _context;
            public AddUserHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<MstUserDetailDto> Handle(AddUser command, CancellationToken cancellationToken)
            {
                var newUser = new MstUserDetailDto()
                {
                    Id = 0,
                    UserName = "NA",
                    Password = "1234",
                    FullName = "NA",
                    ASPUserId = null,
                    IsLocked = true
                };

                return await Task.Run(() => newUser);
            }
        }
    }
}
