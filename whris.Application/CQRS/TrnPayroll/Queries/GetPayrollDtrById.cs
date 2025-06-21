using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Queries
{
    public class GetTrnPayrollById : IRequest<TrnPayrollDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnPayrollByIdHandler : IRequestHandler<GetTrnPayrollById, TrnPayrollDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnPayrollDetailDto> Handle(GetTrnPayrollById request, CancellationToken cancellationToken)
            {
                var dtr = await _context.TrnPayrolls
                    .Include(x => x.TrnPayrollLines)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnPayrollDetailDto();

                var mappingProfile = new MappingProfileForTrnPayrollDetail();
                mappingProfile.mapper.Map(dtr, result);

                return result;
            }
        }
    }
}
