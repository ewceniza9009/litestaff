using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Commands
{
    public class AddPayrollOtherIncomeLine : IRequest<TrnPayrollOtherIncomeLineDto>
    {
        public int PayrollOtherIncomeId { get; set; }

        public class AddPayrollOtherIncomeLineHandler : IRequestHandler<AddPayrollOtherIncomeLine, TrnPayrollOtherIncomeLineDto>
        {
            private readonly HRISContext _context;

            public AddPayrollOtherIncomeLineHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnPayrollOtherIncomeLineDto> Handle(AddPayrollOtherIncomeLine command, CancellationToken cancellationToken)
            {
                var result = new TrnPayrollOtherIncomeLineDto()
                {
                    Id = 0,
                    PayrollOtherIncomeId = command.PayrollOtherIncomeId,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    OtherIncomeId = _context.MstOtherIncomes.FirstOrDefault()?.Id ?? 0,
                    Amount = 8,
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
