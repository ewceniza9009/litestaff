using MediatR;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstOtherDeduction.Queries
{
    public class GetMstOtherDeductions : IRequest<List<MstOtherDeductionDto>>
    {
        public class GetMstOtherDeductionHandler : IRequestHandler<GetMstOtherDeductions, List<MstOtherDeductionDto>>
        {
            private readonly HRISContext _context;
            public GetMstOtherDeductionHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<List<MstOtherDeductionDto>> Handle(GetMstOtherDeductions request, CancellationToken cancellationToken)
            {
                var result = _context.MstOtherDeductions
                    .Select(x => new MappingProfile<Data.Models.MstOtherDeduction, MstOtherDeductionDto>().mapper.Map<MstOtherDeductionDto>(x))
                    .ToList();

                return await Task.Run(() => result);
            }
        }
    }
}
