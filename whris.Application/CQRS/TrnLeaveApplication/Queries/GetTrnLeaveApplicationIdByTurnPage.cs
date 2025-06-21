using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Queries
{
    public class GetTrnLeaveApplicationIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int? PayrollGroupId { get; set; }
        public string? Action { get; set; }

        public class GetTrnLeaveApplicationIdByTurnPageHandler : IRequestHandler<GetTrnLeaveApplicationIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnLeaveApplicationIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnLeaveApplicationIdByTurnPage request, CancellationToken cancellationToken)
            {
                var laNumberString = _context.TrnLeaveApplications
                   .Find(request.Id)?.Lanumber;

                var result = new Data.Models.TrnLeaveApplication();

                if (request.Action == "prev")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnLeaveApplications
                            .Where(x => x.Lanumber.CompareTo(laNumberString) < 0)
                            .OrderByDescending(x => x.Lanumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnLeaveApplications
                            .Where(x => x.Lanumber.CompareTo(laNumberString) < 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderByDescending(x => x.Lanumber)
                            .FirstOrDefaultAsync();
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnLeaveApplications
                            .Where(x => x.Lanumber.CompareTo(laNumberString) > 0)
                            .OrderBy(x => x.Lanumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnLeaveApplications
                            .Where(x => x.Lanumber.CompareTo(laNumberString) > 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderBy(x => x.Lanumber)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
