using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Queries
{
    public class GetSelectOtherIncomes : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public class GetSelectedOtherIncomesHandler : IRequestHandler<GetSelectOtherIncomes, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetSelectedOtherIncomesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetSelectOtherIncomes request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.MstOtherIncomes
                    .OrderBy(x => x.OtherIncome)
                    .Select(x => new TmpOtherIncome()
                    {
                        Id = x.Id,
                        OtherIncome = x.OtherIncome.ToUpper(),
                        Amount = x.Amount,
                        IsSelected = false,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
