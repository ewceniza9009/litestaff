using MediatR;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstOtherIncome.Queries
{
    public class GetMstOtherIncomes : IRequest<List<MstOtherIncomeDto>>
    {
        public class GetMstOtherIncomeHandler : IRequestHandler<GetMstOtherIncomes, List<MstOtherIncomeDto>>
        {
            private readonly HRISContext _context;
            public GetMstOtherIncomeHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<List<MstOtherIncomeDto>> Handle(GetMstOtherIncomes request, CancellationToken cancellationToken)
            {
                var result = _context.MstOtherIncomes
                    .Select(x => new MappingProfile<Data.Models.MstOtherIncome, MstOtherIncomeDto>().mapper.Map<MstOtherIncomeDto>(x))
                    .ToList();

                return await Task.Run(() => result);
            }
        }
    }
}
