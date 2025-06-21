using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Queries
{
    public class GetTrnPayrollOtherIncomeById : IRequest<TrnPayrollOtherIncomeDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnPayrollOtherIncomeByIdHandler : IRequestHandler<GetTrnPayrollOtherIncomeById, TrnPayrollOtherIncomeDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollOtherIncomeByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnPayrollOtherIncomeDetailDto> Handle(GetTrnPayrollOtherIncomeById request, CancellationToken cancellationToken)
            {
                var pod = await _context.TrnPayrollOtherIncomes
                    .Include(x => x.TrnPayrollOtherIncomeLines)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnPayrollOtherIncomeDetailDto();

                var mappingProfile = new MappingProfileForTrnPayrollOtherIncomeDetail();
                mappingProfile.mapper.Map(pod, result);

                return result;
            }
        }
    }
}
