using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstUser.Commands
{
    public class AddUserForm : IRequest<MstUserFormDto>
    {
        public int UserId { get; set; }

        public class AddUserFormHandler : IRequestHandler<AddUserForm, MstUserFormDto>
        {
            private readonly HRISContext _context;

            public AddUserFormHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstUserFormDto> Handle(AddUserForm command, CancellationToken cancellationToken)
            {
                var result = new MstUserFormDto()
                {
                    Id = 0,
                    UserId = command.UserId,
                    FormId = 0,
                    CanAdd = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanLock = true,
                    CanPreview = true,
                    CanPrint = true,
                    CanUnlock = true,
                    CanView = true,
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
