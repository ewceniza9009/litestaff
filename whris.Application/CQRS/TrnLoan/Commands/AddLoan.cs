using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLoan.Commands
{
    public class AddLoan : IRequest<TrnLoanDetailDto>
    {
        public class AddLoanHandler : IRequestHandler<AddLoan, TrnLoanDetailDto>
        {
            private readonly HRISContext _context;
            public AddLoanHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnLoanDetailDto> Handle(AddLoan command, CancellationToken cancellationToken)
            {
                var newLoan = new TrnLoanDetailDto()
                {
                    Id = 0,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    OtherDeductionId = _context.MstOtherDeductions.FirstOrDefault()?.Id ?? 0,
                    LoanNumber = 0,
                    LoanAmount = 1000,
                    NumberOfMonths = 4,
                    MonthlyAmortization = 250,
                    DateStart = DateTime.Now.Date,
                    Balance= 1000,
                    Remarks = "NA",
                    IsPaid = false,
                    IsLocked = false
                };

                return await Task.Run(() => newLoan);
            }
        }
    }
}
