using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.TrnPayroll.Queries
{
    public class GetPayrollOtherIncomesByPOIId : IRequest<List<TrnPayrollOtherIncomeLineDto>>
    {
        public int POIId { get; set; }
        public int EmployeeId { get; set; }
        public bool Taxable { get; set; }

        public class GetPayrollOtherIncomesByIdHandler : IRequestHandler<GetPayrollOtherIncomesByPOIId, List<TrnPayrollOtherIncomeLineDto>>
        {
            private readonly HRISContext _context;
            public GetPayrollOtherIncomesByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<List<TrnPayrollOtherIncomeLineDto>> Handle(GetPayrollOtherIncomesByPOIId request, CancellationToken cancellationToken)
            {
                var result = await _context.TrnPayrollOtherIncomeLines
                    .Include(x => x.OtherIncome)
                    .Where(x => x.PayrollOtherIncomeId == request.POIId && 
                        x.EmployeeId == request.EmployeeId &&
                        x.OtherIncome.Taxable == request.Taxable)
                    .Select(x => new TrnPayrollOtherIncomeLineDto() 
                    {
                        Id= x.Id,
                        PayrollOtherIncomeId= x.PayrollOtherIncomeId,
                        EmployeeId= x.EmployeeId,
                        OtherIncomeId = x.OtherIncomeId,
                        Amount= x.Amount,
                    })
                    .ToListAsync()
                    ?? new List<TrnPayrollOtherIncomeLineDto>();

                return result;
            }
        }
    }
}
