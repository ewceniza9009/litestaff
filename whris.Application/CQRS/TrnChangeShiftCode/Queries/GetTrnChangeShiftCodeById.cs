using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Queries
{
    public class GetTrnChangeShiftCodeById : IRequest<TrnChangeShiftCodeDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnChangeShiftCodeByIdHandler : IRequestHandler<GetTrnChangeShiftCodeById, TrnChangeShiftCodeDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnChangeShiftCodeByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnChangeShiftCodeDetailDto> Handle(GetTrnChangeShiftCodeById request, CancellationToken cancellationToken)
            {
                var cs = await _context.TrnChangeShifts
                    .Include(x => x.TrnChangeShiftLines)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnChangeShiftCodeDetailDto();

                var mappingProfile = new MappingProfileForTrnChangeShiftCodeDetail();
                mappingProfile.mapper.Map(cs, result);

                return result;
            }
        }
    }
}
