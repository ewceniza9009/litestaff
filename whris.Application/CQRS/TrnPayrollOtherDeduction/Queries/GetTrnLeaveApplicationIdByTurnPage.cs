using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Queries
{
    public class GetTrnPayrollOtherDeductionIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int? PayrollGroupId { get; set; }
        public string? Action { get; set; }

        public class GetTrnPayrollOtherDeductionIdByTurnPageHandler : IRequestHandler<GetTrnPayrollOtherDeductionIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetTrnPayrollOtherDeductionIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetTrnPayrollOtherDeductionIdByTurnPage request, CancellationToken cancellationToken)
            {
                var podNumberString = _context.TrnPayrollOtherDeductions
                   .Find(request.Id)?.Podnumber;

                var result = new Data.Models.TrnPayrollOtherDeduction();

                if (request.Action == "prev")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnPayrollOtherDeductions
                            .Where(x => x.Podnumber.CompareTo(podNumberString) < 0)
                            .OrderByDescending(x => x.Podnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnPayrollOtherDeductions
                            .Where(x => x.Podnumber.CompareTo(podNumberString) < 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderByDescending(x => x.Podnumber)
                            .FirstOrDefaultAsync();
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.PayrollGroupId == 0)
                    {
                        result = await _context.TrnPayrollOtherDeductions
                            .Where(x => x.Podnumber.CompareTo(podNumberString) > 0)
                            .OrderBy(x => x.Podnumber)
                            .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.TrnPayrollOtherDeductions
                            .Where(x => x.Podnumber.CompareTo(podNumberString) > 0 && x.PayrollGroupId == request.PayrollGroupId)
                            .OrderBy(x => x.Podnumber)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
