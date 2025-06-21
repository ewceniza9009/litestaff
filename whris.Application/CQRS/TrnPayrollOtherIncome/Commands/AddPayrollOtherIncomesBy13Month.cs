using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Commands
{
    public class AddPayrollOtherIncomesBy13Month : IRequest<int>
    {
        public int POIId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? OtherIncomeId { get; set; }
        public int? EmployeeId { get; set; }
        public int? StartPayNo { get; set; }
        public int? EndPayNo { get; set; }
        public decimal? NoOfPayroll { get; set; }

        public class AddPayrollOtherIncomeLinesHandler : IRequestHandler<AddPayrollOtherIncomesBy13Month, int>
        {
            private readonly HRISContext _context;
            public AddPayrollOtherIncomeLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(AddPayrollOtherIncomesBy13Month command, CancellationToken cancellationToken)
            {
                await PayrollOtherIncome.Post13MonthPayrollOtherIncomeLines(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
