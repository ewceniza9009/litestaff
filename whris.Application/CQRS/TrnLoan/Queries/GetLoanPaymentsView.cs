using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLoan.Queries
{
    public class GetLoanPaymentsView : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int EmployeeLoanId { get; set; }

        public class GetTrnLoansByPayrollIdHandler : IRequestHandler<GetLoanPaymentsView, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnLoansByPayrollIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetLoanPaymentsView request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                var loanPayments = new Application.Queries.TrnLoan.PaymentList()
                {
                    EmployeeLoanId = request.EmployeeLoanId
                };

                result = loanPayments.Result()
                    .ToDataSourceResult(request.Request);

                return await Task.Run(() => result);

            }
        }
    }
}
