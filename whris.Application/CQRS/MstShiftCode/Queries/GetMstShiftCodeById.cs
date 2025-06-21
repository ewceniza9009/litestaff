using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstShiftCode.Queries
{
    public class GetMstShiftCodeById : IRequest<MstShiftCodeDetailDto>
    {
        public int? Id { get; set; }

        public class GetMstShiftCodeByIdHandler : IRequestHandler<GetMstShiftCodeById, MstShiftCodeDetailDto>
        {
            private readonly HRISContext _context;
            public GetMstShiftCodeByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstShiftCodeDetailDto> Handle(GetMstShiftCodeById request, CancellationToken cancellationToken)
            {
                var ShiftCode = await _context.MstShiftCodes
                    .Include(x => x.MstShiftCodeDays)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new MstShiftCodeDetailDto();

                var mappingProfile = new MappingProfileForMstShiftCodeDetail();
                mappingProfile.mapper.Map(ShiftCode, result);

                return result;
            }
        }
    }
}
