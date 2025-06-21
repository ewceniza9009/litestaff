using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Queries
{
    public class GetTrnDtrById : IRequest<TrnDtrDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnDtrByIdHandler : IRequestHandler<GetTrnDtrById, TrnDtrDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnDtrByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnDtrDetailDto> Handle(GetTrnDtrById request, CancellationToken cancellationToken)
            {
                var dtr = await _context.TrnDtrs
                    //.Include(x => x.TrnDtrlines)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnDtrDetailDto();

                var mappingProfile = new MappingProfileForTrnDtrDetail();
                mappingProfile.mapper.Map(dtr, result);

                return result;
            }
        }
    }
}
