using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.TrnPayroll.Queries
{
    public class GetPayrollOtherDeductionsByPODId : IRequest<List<TrnPayrollOtherDeductionLineDto>>
    {
        public int PODId { get; set; }
        public int EmployeeId { get; set; }
        public bool Taxable { get; set; }

        public class GetPayrollOtherDeductionsByIdHandler : IRequestHandler<GetPayrollOtherDeductionsByPODId, List<TrnPayrollOtherDeductionLineDto>>
        {
            private readonly HRISContext _context;
            public GetPayrollOtherDeductionsByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<List<TrnPayrollOtherDeductionLineDto>> Handle(GetPayrollOtherDeductionsByPODId request, CancellationToken cancellationToken)
            {
                var result = await _context.TrnPayrollOtherDeductionLines
                    .Include(x => x.OtherDeduction)
                    .Where(x => x.PayrollOtherDeductionId == request.PODId && 
                        x.EmployeeId == request.EmployeeId)
                    .Select(x => new TrnPayrollOtherDeductionLineDto() 
                    {
                        Id= x.Id,
                        PayrollOtherDeductionId = x.PayrollOtherDeductionId,
                        EmployeeId= x.EmployeeId,
                        OtherDeductionId = x.OtherDeductionId,
                        Amount= x.Amount,
                    })
                    .ToListAsync()
                    ?? new List<TrnPayrollOtherDeductionLineDto>();

                return result;
            }
        }
    }
}
