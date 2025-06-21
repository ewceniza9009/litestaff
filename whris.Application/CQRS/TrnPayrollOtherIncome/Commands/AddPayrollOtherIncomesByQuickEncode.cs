using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Commands
{
    public class AddPayrollOtherIncomesByQuickEncode : IRequest<int>
    {
        public int POIId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? EmployeeId { get; set; }
        public List<TmpOtherIncome>? SelectedOtherIncomes { get; set; }

        public class AddPayrollOtherIncomeLinesHandler : IRequestHandler<AddPayrollOtherIncomesByQuickEncode, int>
        {
            private readonly HRISContext _context;
            public AddPayrollOtherIncomeLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(AddPayrollOtherIncomesByQuickEncode command, CancellationToken cancellationToken)
            {
                await PayrollOtherIncome.EncodePayrollOtherIncomeLines(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
