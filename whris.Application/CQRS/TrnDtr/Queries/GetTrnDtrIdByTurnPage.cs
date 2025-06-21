using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Queries
{
    public class GetTrnDtrIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int? PayrollGroupId { get; set; }
        public string? Action { get; set; }

        public class GetTrnDtrIdByTurnPageHandler : IRequestHandler<GetTrnDtrIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnDtrIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnDtrIdByTurnPage request, CancellationToken cancellationToken)
            {
                var dtrNumberString = _context.TrnDtrs
                   .Find(request.Id)?.Dtrnumber;

                var result = new Data.Models.TrnDtr();

                if (request.Action == "prev")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnDtrs
                            .Where(x => x.Dtrnumber.CompareTo(dtrNumberString) < 0)
                            .OrderByDescending(x => x.Dtrnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnDtrs
                            .Where(x => x.Dtrnumber.CompareTo(dtrNumberString) < 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderByDescending(x => x.Dtrnumber)
                            .FirstOrDefaultAsync();
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnDtrs
                            .Where(x => x.Dtrnumber.CompareTo(dtrNumberString) > 0)
                            .OrderBy(x => x.Dtrnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnDtrs
                            .Where(x => x.Dtrnumber.CompareTo(dtrNumberString) > 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderBy(x => x.Dtrnumber)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
