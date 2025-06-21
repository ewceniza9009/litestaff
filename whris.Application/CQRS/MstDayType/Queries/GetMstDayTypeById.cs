using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstDayType.Queries
{
    public class GetMstDayTypeById : IRequest<MstDayTypeDetailDto>
    {
        public int? Id { get; set; }

        public class GetMstDayTypeByIdHandler : IRequestHandler<GetMstDayTypeById, MstDayTypeDetailDto>
        {
            private readonly HRISContext _context;
            public GetMstDayTypeByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstDayTypeDetailDto> Handle(GetMstDayTypeById request, CancellationToken cancellationToken)
            {
                var dayType = await _context.MstDayTypes
                    .Include(x => x.MstDayTypeDays)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new MstDayTypeDetailDto();

                var mappingProfile = new MappingProfileForMstDayTypeDetail();
                mappingProfile.mapper.Map(dayType, result);

                return result;
            }
        }
    }
}
