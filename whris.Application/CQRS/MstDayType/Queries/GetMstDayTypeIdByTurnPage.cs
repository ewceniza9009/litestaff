using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.MstDayType.Queries
{
    public class GetMstDayTypeIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public string? Action { get; set; }

        public class GetMstDayTypeIdByTurnPageHandler : IRequestHandler<GetMstDayTypeIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetMstDayTypeIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetMstDayTypeIdByTurnPage request, CancellationToken cancellationToken)
            {
                var dayTypeString = _context.MstDayTypes
                    .Find(request.Id)?.DayType;

                var result = new Data.Models.MstDayType();

                if (request.Action == "prev")
                {
                    result = await _context.MstDayTypes
                        .Where(x => x.DayType.CompareTo(dayTypeString) < 0)
                        .OrderByDescending(x => x.DayType)
                        .FirstOrDefaultAsync(); ;
                }
                else if (request.Action == "next")
                {
                    result = await _context.MstDayTypes
                        .Where(x => x.DayType.CompareTo(dayTypeString) > 0)
                        .OrderBy(x => x.DayType)
                        .FirstOrDefaultAsync();
                }

                return result?.Id ?? 0;
            }
        }
    }
}
