using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Queries
{
    public class GetTrnPayrollOtherDeductionById : IRequest<TrnPayrollOtherDeductionDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnPayrollOtherDeductionByIdHandler : IRequestHandler<GetTrnPayrollOtherDeductionById, TrnPayrollOtherDeductionDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollOtherDeductionByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnPayrollOtherDeductionDetailDto> Handle(GetTrnPayrollOtherDeductionById request, CancellationToken cancellationToken)
            {
                var pod = await _context.TrnPayrollOtherDeductions
                    .Include(x => x.TrnPayrollOtherDeductionLines)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnPayrollOtherDeductionDetailDto();

                var mappingProfile = new MappingProfileForTrnPayrollOtherDeductionDetail();
                mappingProfile.mapper.Map(pod, result);

                return result;
            }
        }
    }
}
