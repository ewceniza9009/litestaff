using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLoan.Queries
{
    public class GetTrnLoansByPayrollGroupId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int PayrollGroupId { get; set; }

        public class GetTrnLoansByPayrollIdHandler : IRequestHandler<GetTrnLoansByPayrollGroupId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnLoansByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnLoansByPayrollGroupId request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                result = await _context.MstEmployeeLoans
                    .Include(x => x.Employee)
                    .Include(x => x.OtherDeduction)
                    .Where(x => x.Employee.PayrollGroupId == request.PayrollGroupId)
                    .OrderBy(x => x.Employee.FullName)
                    .ThenByDescending(x => x.DateStart)
                    .Select(x => new TrnLoanDto()
                    {
                        Id = x.Id,
                        FullName = x.Employee.FullName,
                        DateStart = x.DateStart,
                        OtherDeduction = x.OtherDeduction.OtherDeduction,
                        Remarks = x.Remarks,
                        Balance = x.Balance,
                        IsPaid = x.IsPaid,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
