using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Queries
{
    public class GetTrnPayrollIdByTurnPage : IRequest<int>
    {
        public int Id { get; set; }
        public int PayrollGroupId { get; set; }
        public string Action { get; set; }

        public class GetTrnPayrollIdByTurnPageHandler : IRequestHandler<GetTrnPayrollIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnPayrollIdByTurnPage request, CancellationToken cancellationToken)
            {
                var payrollNumberString = _context.TrnPayrolls
                   .Find(request.Id)?.PayrollNumber;

                var result = new Data.Models.TrnPayroll();

                if (request.Action == "prev")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnPayrolls
                            .Where(x => x.PayrollNumber.CompareTo(payrollNumberString) < 0)
                            .OrderByDescending(x => x.PayrollNumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnPayrolls
                            .Where(x => x.PayrollNumber.CompareTo(payrollNumberString) < 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderByDescending(x => x.PayrollNumber)
                            .FirstOrDefaultAsync();
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnPayrolls
                            .Where(x => x.PayrollNumber.CompareTo(payrollNumberString) > 0)
                            .OrderBy(x => x.PayrollNumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnPayrolls
                            .Where(x => x.PayrollNumber.CompareTo(payrollNumberString) > 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderBy(x => x.PayrollNumber)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
