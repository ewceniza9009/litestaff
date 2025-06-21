using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Application.Queries.TrnDtr;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLogs.Queries
{
    public class GetTrnLogs : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? EmployeeId { get; set; }


        public class GetTrnLogsHandler : IRequestHandler<GetTrnLogs, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnLogsHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnLogs request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                var logs = new Logs()
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    EmployeeId = request.EmployeeId
                };

                result = await logs.Result().ToDataSourceResultAsync(request.Request);

                return result;
            }
        }
    }
}
