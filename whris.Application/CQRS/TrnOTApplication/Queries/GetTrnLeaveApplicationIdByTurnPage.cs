using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Queries
{
    public class GetTrnOTApplicationIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int? PayrollGroupId { get; set; }
        public string? Action { get; set; }

        public class GetTrnOTApplicationIdByTurnPageHandler : IRequestHandler<GetTrnOTApplicationIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnOTApplicationIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnOTApplicationIdByTurnPage request, CancellationToken cancellationToken)
            {
                var otNumberString = _context.TrnOverTimes
                   .Find(request.Id)?.Otnumber;

                var result = new Data.Models.TrnOverTime();

                if (request.Action == "prev")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnOverTimes
                            .Where(x => x.Otnumber.CompareTo(otNumberString) < 0)
                            .OrderByDescending(x => x.Otnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnOverTimes
                            .Where(x => x.Otnumber.CompareTo(otNumberString) < 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderByDescending(x => x.Otnumber)
                            .FirstOrDefaultAsync();
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnOverTimes
                            .Where(x => x.Otnumber.CompareTo(otNumberString) > 0)
                            .OrderBy(x => x.Otnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnOverTimes
                            .Where(x => x.Otnumber.CompareTo(otNumberString) > 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderBy(x => x.Otnumber)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
