using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLastWithholdingTax.Queries
{
    public class GetTrnLastWithholdingTaxById : IRequest<TrnLastWithholdingTaxDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnLastWithholdingTaxByIdHandler : IRequestHandler<GetTrnLastWithholdingTaxById, TrnLastWithholdingTaxDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnLastWithholdingTaxByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnLastWithholdingTaxDetailDto> Handle(GetTrnLastWithholdingTaxById request, CancellationToken cancellationToken)
            {
                var lastWithholdingtTax = await _context.TrnLastWithholdingTaxes
                    .Include(x => x.TrnLastWithholdingTaxLines)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnLastWithholdingTaxDetailDto();

                var mappingProfile = new MappingProfileForTrnLastWithholdingTaxDetail();
                mappingProfile.mapper.Map(lastWithholdingtTax, result);

                return result;
            }
        }
    }
}
