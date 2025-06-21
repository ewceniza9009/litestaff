using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstShiftCode.Queries
{
    public  class GetMstShiftCodesBySearch : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public string? Search { get; set; }

        public class GetMstShiftCodesBySearchHandler : IRequestHandler<GetMstShiftCodesBySearch, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetMstShiftCodesBySearchHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetMstShiftCodesBySearch request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();
                var search = request?.Search?.ToLower() ?? "";

                if (request == null)
                {
                    return result;
                }

                result = await _context.MstShiftCodes
                    .Where(x => x.ShiftCode.ToLower().Contains(search))
                    .OrderBy(x => x.ShiftCode)
                    .Select(x => new MstShiftCodeDto()
                    {
                        Id = x.Id,
                        ShiftCode = x.ShiftCode,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
