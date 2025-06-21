using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Queries
{
    public class GetTrnPayrollOtherIncomesByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnPayrollOtherIncomesByPayrollIdHandler : IRequestHandler<GetTrnPayrollOtherIncomesByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollOtherIncomesByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnPayrollOtherIncomesByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.TrnPayrollOtherIncomes
                    .Where(x => x.PayrollGroupId == request.PayrollGroupId)
                    .OrderByDescending(x => x.Poidate)
                    .ThenByDescending(x => x.Poinumber)
                    .Select(x => new TrnPayrollOtherIncomeDto()
                    {
                        Id = x.Id,
                        Poidate = x.Poidate,
                        Poinumber = x.Poinumber,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
