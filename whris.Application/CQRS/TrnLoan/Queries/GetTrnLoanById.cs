using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.TrnLoan.Queries
{
    public class GetTrnLoanById : IRequest<TrnLoanDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnLoanByIdHandler : IRequestHandler<GetTrnLoanById, TrnLoanDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnLoanByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnLoanDetailDto> Handle(GetTrnLoanById request, CancellationToken cancellationToken)
            {
                var loan = await _context.MstEmployeeLoans
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnLoanDetailDto();

                var mappingProfile = new MappingProfile<MstEmployeeLoan, TrnLoanDetailDto>();
                mappingProfile.mapper.Map(loan, result);

                return result;
            }
        }
    }
}
