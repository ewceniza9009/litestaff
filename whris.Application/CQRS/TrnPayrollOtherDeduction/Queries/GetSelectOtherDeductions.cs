using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Queries
{
    public class GetSelectOtherDeductions : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public class GetSelectedOtherDeductionsIdHandler : IRequestHandler<GetSelectOtherDeductions, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetSelectedOtherDeductionsIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetSelectOtherDeductions request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.MstOtherDeductions
                    .OrderBy(x => x.OtherDeduction)
                    .Select(x => new TmpOtherDeduction()
                    {
                        Id = x.Id,
                        OtherDeduction = x.OtherDeduction.ToUpper(),
                        Amount = x.Amount,
                        IsSelected = false,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
