using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstDayType.Queries
{
    public  class GetMstDayTypesBySearch : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public string? Search { get; set; }

        public class GetMstDayTypesBySearchHandler : IRequestHandler<GetMstDayTypesBySearch, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetMstDayTypesBySearchHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetMstDayTypesBySearch request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();
                var search = request?.Search?.ToLower() ?? "";

                if (request == null)
                {
                    return result;
                }

                result = await _context.MstDayTypes
                    .Where(x => x.DayType.ToLower().Contains(search))
                    .OrderBy(x => x.DayType)
                    .Select(x => new MstDayTypeDto()
                    {
                        Id = x.Id,
                        DayType = x.DayType,
                        RestdayDays = x.RestdayDays,
                        WorkingDays = x.WorkingDays,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
