using MediatR;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstUser.Commands
{
    public class SaveUser : IRequest<int>
    {
        public MstUserDetailDto? User { get; set; }

        public class SaveUserHandler : IRequestHandler<SaveUser, int>
        {
            private readonly HRISContext _context;
            public SaveUserHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveUser command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var newUser = command?.User ?? new MstUserDetailDto();
                newUser.MstUserForms.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newUser);

                var mappingProfile = new MappingProfileForMstUserDetailReverse();

                if (newUser.Id == 0)
                {
                    var addedUser = mappingProfile.mapper.Map<Data.Models.MstUser>(newUser);
                    await _context.MstUsers.AddAsync(addedUser ?? new Data.Models.MstUser());

                    await _context.SaveChangesAsync();

                    resultId = addedUser?.Id ?? 0;
                }
                else 
                {
                    var oldUser = _context.MstUsers.Find(command?.User?.Id ?? 0);
                    mappingProfile.mapper.Map(newUser, oldUser);

                    await _context.SaveChangesAsync();

                    resultId = oldUser?.Id ?? 0;
                }

                var deletedUserFormIds = newUser.MstUserForms.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedUserFormRange = _context.MstUserForms.Where(x => deletedUserFormIds.Contains(x.Id)).ToList();

                _context.MstUserForms.RemoveRange(deletedUserFormRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
