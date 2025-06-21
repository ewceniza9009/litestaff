using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Queries
{
    public class GetTrnChangeShiftCodesByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnChangeShiftCodesByPayrollIdHandler : IRequestHandler<GetTrnChangeShiftCodesByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnChangeShiftCodesByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnChangeShiftCodesByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.TrnChangeShifts
                    .Where(x => x.PayrollGroupId == request.PayrollGroupId)
                    .OrderByDescending(x => x.Csdate)
                    .ThenByDescending(x => x.Csnumber)
                    .Select(x => new TrnChangeShiftCodeDto()
                    {
                        Id = x.Id,
                        Csdate = x.Csdate,
                        Csnumber = x.Csnumber,
                        Remarks = x.Remarks,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
