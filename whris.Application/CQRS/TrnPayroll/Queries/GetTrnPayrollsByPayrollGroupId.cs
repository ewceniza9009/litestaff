using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Queries
{
    public class GetTrnPayrollsByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnPayrollsByPayrollIdHandler : IRequestHandler<GetTrnPayrollsByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollsByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnPayrollsByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.TrnPayrolls
                    .Where(x => x.PayrollGroupId == request.PayrollGroupId)
                    .OrderByDescending(x => x.PayrollDate)
                    .ThenByDescending(x => x.PayrollNumber)
                    .Select(x => new TrnPayrollDto()
                    {
                        Id = x.Id,
                        PayrollDate = x.PayrollDate,
                        PayrollNumber = x.PayrollNumber,
                        PayrollGroupId = request.PayrollGroupId,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
