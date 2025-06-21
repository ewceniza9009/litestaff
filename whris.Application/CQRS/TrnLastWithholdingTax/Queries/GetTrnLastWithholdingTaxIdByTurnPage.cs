using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLastWithholdingTax.Queries
{
    public class GetTrnLastWithholdingTaxIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int? PayrollGroupId { get; set; }
        public string? Action { get; set; }

        public class GetTrnLastWithholdingTaxIdByTurnPageHandler : IRequestHandler<GetTrnLastWithholdingTaxIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnLastWithholdingTaxIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnLastWithholdingTaxIdByTurnPage request, CancellationToken cancellationToken)
            {
                var lwtNumberString = _context.TrnLastWithholdingTaxes
                   .Find(request.Id)?.Lwtnumber;

                var result = new Data.Models.TrnLastWithholdingTax();

                if (request.Action == "prev")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnLastWithholdingTaxes
                            .Where(x => x.Lwtnumber.CompareTo(lwtNumberString) < 0)
                            .OrderByDescending(x => x.Lwtnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnLastWithholdingTaxes
                            .Where(x => x.Lwtnumber.CompareTo(lwtNumberString) < 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderByDescending(x => x.Lwtnumber)
                            .FirstOrDefaultAsync();
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnLastWithholdingTaxes
                            .Where(x => x.Lwtnumber.CompareTo(lwtNumberString) > 0)
                            .OrderBy(x => x.Lwtnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnLastWithholdingTaxes
                            .Where(x => x.Lwtnumber.CompareTo(lwtNumberString) > 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderBy(x => x.Lwtnumber)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
