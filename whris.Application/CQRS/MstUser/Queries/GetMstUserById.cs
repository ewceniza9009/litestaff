using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstUser.Queries
{
    public class GetMstUserById : IRequest<MstUserDetailDto>
    {
        public int? Id { get; set; }

        public class GetMstUserByIdHandler : IRequestHandler<GetMstUserById, MstUserDetailDto>
        {
            private readonly HRISContext _context;
            public GetMstUserByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstUserDetailDto> Handle(GetMstUserById request, CancellationToken cancellationToken)
            {
                var users = await _context.MstUsers
                    .Include(x => x.MstUserForms)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new MstUserDetailDto();

                var mappingProfile = new MappingProfileForMstUserDetail();
                mappingProfile.mapper.Map(users, result);

                return result;
            }
        }
    }
}
