using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.TrnLoan.Commands
{
    public class SaveLoan : IRequest<int>
    {
        public TrnLoanDetailDto? Loan { get; set; }

        public class SaveLoanHandler : IRequestHandler<SaveLoan, int>
        {
            private readonly HRISContext _context;
            public SaveLoanHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveLoan command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var newLoan = command?.Loan ?? new TrnLoanDetailDto();

                Utilities.UpdateEntityAuditFields(newLoan);

                var mappingProfile = new MappingProfile<TrnLoanDetailDto, MstEmployeeLoan>();

                if (newLoan.Id == 0)
                {
                    var addedLoan = mappingProfile.mapper.Map<MstEmployeeLoan>(newLoan);
                    await _context.MstEmployeeLoans.AddAsync(addedLoan ?? new Data.Models.MstEmployeeLoan());

                    await _context.SaveChangesAsync();

                    resultId = addedLoan?.Id ?? 0;
                }
                else 
                {
                    var oldLoan = await _context.MstEmployeeLoans.FindAsync(command?.Loan?.Id ?? 0);
                    mappingProfile.mapper.Map(newLoan, oldLoan);

                    await _context.SaveChangesAsync();

                    resultId = oldLoan?.Id ?? 0;
                }

                var employees = await _context.MstEmployees
                    .Where(x => x.IsLocked)
                    .ToListAsync();

                foreach (var emp in employees)
                {
                    var loanAmount = await _context.MstEmployeeLoans
                        .Where(x => x.EmployeeId == emp.Id && x.IsLocked)
                        .SumAsync(x => x.LoanAmount);

                    var loanPayment = await _context.TrnPayrollOtherDeductionLines
                        .Include(x => x.PayrollOtherDeduction)
                        .Where(x => x.EmployeeId == emp.Id && x.PayrollOtherDeduction.IsLocked)
                        .SumAsync(x => x.Amount);

                    emp.LoanBalance = loanAmount - loanPayment;
                }

                return resultId;
            }
        }
    }
}
