using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Queries
{
    public class GetTrnLeaveApplicationsByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnLeaveApplicationsByPayrollIdHandler : IRequestHandler<GetTrnLeaveApplicationsByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnLeaveApplicationsByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnLeaveApplicationsByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.TrnLeaveApplications
                    .Where(x => x.PayrollGroupId == request.PayrollGroupId)
                    .OrderByDescending(x => x.Ladate)
                    .ThenByDescending(x => x.Lanumber)
                    .Select(x => new TrnLeaveApplicationDto()
                    {
                        Id = x.Id,
                        Ladate = x.Ladate,
                        Lanumber = x.Lanumber,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
