using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Queries
{
    public class GetTrnDtrsByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnDtrsByPayrollIdHandler : IRequestHandler<GetTrnDtrsByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnDtrsByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnDtrsByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.TrnDtrs
                    .Where(x => x.PayrollGroupId == request.PayrollGroupId)
                    .OrderByDescending(x => x.Dtrdate)
                    .ThenByDescending(x => x.Dtrnumber)
                    .Select(x => new TrnDtrDto()
                    {
                        Id = x.Id,
                        Dtrdate = x.Dtrdate,
                        Dtrnumber = x.Dtrnumber,
                        PayrollGroupId = request.PayrollGroupId,
                        DateStart = x.DateStart,
                        DateEnd = x.DateEnd,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
