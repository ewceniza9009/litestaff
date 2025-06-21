using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLoan.Queries
{
    public class GetTrnLoanIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int PayrollGroupId { get; set; }
        public string? Action { get; set; }

        public class GetTrnLoanIdByTurnPageHandler : IRequestHandler<GetTrnLoanIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnLoanIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnLoanIdByTurnPage request, CancellationToken cancellationToken)
            {
                //var empNameString = _context.MstEmployeeLoans
                //    .Include(x => x.Employee)
                //   .FirstOrDefault(x => x.Id == request.Id)?.Employee.FullName;

                var result = new Data.Models.MstEmployeeLoan();

                if (request.Action == "prev")
                {
                    result = await _context.MstEmployeeLoans
                            .Where(x => x.Id.CompareTo(request.Id) < 0)
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefaultAsync();
                }
                else if(request.Action == "next")
                {
                    result = await _context.MstEmployeeLoans
                          .Where(x => x.Id.CompareTo(request.Id) > 0)
                          .OrderBy(x => x.Id)
                          .FirstOrDefaultAsync();
                }

                return result?.Id ?? 0;
            }
        }
    }
}
