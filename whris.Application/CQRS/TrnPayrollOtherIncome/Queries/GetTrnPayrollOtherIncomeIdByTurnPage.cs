using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Queries
{
    public class GetTrnPayrollOtherIncomeIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int? PayrollGroupId { get; set; }
        public string? Action { get; set; }

        public class GetTrnPayrollOtherIncomeIdByTurnPageHandler : IRequestHandler<GetTrnPayrollOtherIncomeIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollOtherIncomeIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnPayrollOtherIncomeIdByTurnPage request, CancellationToken cancellationToken)
            {
                var poiNumberString = _context.TrnPayrollOtherIncomes
                   .Find(request.Id)?.Poinumber;

                var result = new Data.Models.TrnPayrollOtherIncome();

                if (request.Action == "prev")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnPayrollOtherIncomes
                            .Where(x => x.Poinumber.CompareTo(poiNumberString) < 0)
                            .OrderByDescending(x => x.Poinumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnPayrollOtherIncomes
                            .Where(x => x.Poinumber.CompareTo(poiNumberString) < 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderByDescending(x => x.Poinumber)
                            .FirstOrDefaultAsync();
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnPayrollOtherIncomes
                            .Where(x => x.Poinumber.CompareTo(poiNumberString) > 0)
                            .OrderBy(x => x.Poinumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnPayrollOtherIncomes
                            .Where(x => x.Poinumber.CompareTo(poiNumberString) > 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderBy(x => x.Poinumber)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
