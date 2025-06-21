using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Queries
{
    public class GetTrnPayrollOtherDeductionsByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnPayrollOtherDeductionsByPayrollIdHandler : IRequestHandler<GetTrnPayrollOtherDeductionsByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollOtherDeductionsByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnPayrollOtherDeductionsByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.TrnPayrollOtherDeductions
                    .Where(x => x.PayrollGroupId == request.PayrollGroupId)
                    .OrderByDescending(x => x.Poddate)
                    .ThenByDescending(x => x.Podnumber)
                    .Select(x => new TrnPayrollOtherDeductionDto()
                    {
                        Id = x.Id,
                        Poddate = x.Poddate,
                        Podnumber = x.Podnumber,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
