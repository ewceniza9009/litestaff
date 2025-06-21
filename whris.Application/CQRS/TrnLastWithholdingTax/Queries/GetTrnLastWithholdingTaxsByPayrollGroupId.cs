using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLastWithholdingTax.Queries
{
    public class GetTrnLastWithholdingTaxesByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnLastWithholdingTaxesByPayrollIdHandler : IRequestHandler<GetTrnLastWithholdingTaxesByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnLastWithholdingTaxesByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnLastWithholdingTaxesByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.TrnLastWithholdingTaxes
                    .Where(x => x.PayrollGroupId == request.PayrollGroupId)
                    .OrderByDescending(x => x.Lwtdate)
                    .ThenByDescending(x => x.Lwtnumber)
                    .Select(x => new TrnLastWithholdingTaxDto()
                    {
                        Id = x.Id,
                        Lwtdate = x.Lwtdate,
                        Lwtnumber = x.Lwtnumber,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
