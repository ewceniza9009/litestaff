using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Mappers;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.TrnPayroll.Queries
{
    public class GetTrnPayrollLineByLineId : IRequest<TrnPayrollLine>
    {
        public int? Id { get; set; }

        public class GetTrnPayrollLineByLineIdHandler : IRequestHandler<GetTrnPayrollLineByLineId, TrnPayrollLine>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollLineByLineIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnPayrollLine> Handle(GetTrnPayrollLineByLineId request, CancellationToken cancellationToken)
            {
                var result = await _context.TrnPayrollLines
                    .FirstOrDefaultAsync(x => x.Id == request.Id)
                    ?? new TrnPayrollLine();

                return result;
            }
        }
    }
}
