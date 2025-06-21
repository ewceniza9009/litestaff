using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Queries
{
    public class GetTrnOTApplicationsByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnOTApplicationsByPayrollIdHandler : IRequestHandler<GetTrnOTApplicationsByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnOTApplicationsByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnOTApplicationsByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.TrnOverTimes
                    .Where(x => x.PayrollGroupId == request.PayrollGroupId)
                    .OrderByDescending(x => x.Otdate)
                    .ThenByDescending(x => x.Otnumber)
                    .Select(x => new TrnOTApplicationDto()
                    {
                        Id = x.Id,
                        Otdate = x.Otdate,
                        Otnumber = x.Otnumber,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
