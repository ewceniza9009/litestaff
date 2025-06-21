using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Commands
{
    public class AddPayrollOtherDeductionsByQuickEncode : IRequest<int>
    {
        public int PODId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? EmployeeId { get; set; }
        public List<TmpOtherDeduction>? SelectedOtherDeductions { get; set; }

        public class AddPayrollOtherDeductionLinesHandler : IRequestHandler<AddPayrollOtherDeductionsByQuickEncode, int>
        {
            private readonly HRISContext _context;
            public AddPayrollOtherDeductionLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(AddPayrollOtherDeductionsByQuickEncode command, CancellationToken cancellationToken)
            {
                await PayrollOtherDeduction.EncodePayrollOtherDeductionLines(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
