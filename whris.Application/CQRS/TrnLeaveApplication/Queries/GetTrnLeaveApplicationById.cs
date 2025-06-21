using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Queries
{
    public class GetTrnLeaveApplicationById : IRequest<TrnLeaveApplicationDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnLeaveApplicationByIdHandler : IRequestHandler<GetTrnLeaveApplicationById, TrnLeaveApplicationDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnLeaveApplicationByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnLeaveApplicationDetailDto> Handle(GetTrnLeaveApplicationById request, CancellationToken cancellationToken)
            {
                var leaveApplication = await _context.TrnLeaveApplications
                    .Include(x => x.TrnLeaveApplicationLines)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnLeaveApplicationDetailDto();

                var mappingProfile = new MappingProfileForTrnLeaveApplicationDetail();
                mappingProfile.mapper.Map(leaveApplication, result);

                return result;
            }
        }
    }
}
