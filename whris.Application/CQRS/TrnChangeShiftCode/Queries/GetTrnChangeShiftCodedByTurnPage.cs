using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Queries
{
    public class GetTrnChangeShiftCodeIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int? PayrollGroupId { get; set; }
        public string? Action { get; set; }

        public class GetTrnChangeShiftCodeIdByTurnPageHandler : IRequestHandler<GetTrnChangeShiftCodeIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnChangeShiftCodeIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnChangeShiftCodeIdByTurnPage request, CancellationToken cancellationToken)
            {
                var csNumberString = _context.TrnChangeShifts
                   .Find(request.Id)?.Csnumber;

                var result = new Data.Models.TrnChangeShift();

                if (request.Action == "prev")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnChangeShifts
                            .Where(x => x.Csnumber.CompareTo(csNumberString) < 0)
                            .OrderByDescending(x => x.Csnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnChangeShifts
                            .Where(x => x.Csnumber.CompareTo(csNumberString) < 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderByDescending(x => x.Csnumber)
                            .FirstOrDefaultAsync();
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnChangeShifts
                            .Where(x => x.Csnumber.CompareTo(csNumberString) > 0)
                            .OrderBy(x => x.Csnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnChangeShifts
                            .Where(x => x.Csnumber.CompareTo(csNumberString) > 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderBy(x => x.Csnumber)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
