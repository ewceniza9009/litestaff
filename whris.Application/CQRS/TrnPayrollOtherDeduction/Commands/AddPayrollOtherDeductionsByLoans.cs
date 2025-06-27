using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Commands
{
    public class AddPayrollOtherDeductionsByLoans : IRequest<int>
    {
        public int PODId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? LoanNumber { get; set; }
        public DateTime? DateFilter { get; set; }
        public int? EmployeeIdFilter { get; set; }
        public List<TmpOtherDeduction>? SelectedOtherDeductions { get; set; }

        public class AddPayrollOtherDeductionLinesHandler : IRequestHandler<AddPayrollOtherDeductionsByLoans, int>
        {
            private readonly HRISContext _context;
            public AddPayrollOtherDeductionLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(AddPayrollOtherDeductionsByLoans command, CancellationToken cancellationToken)
            {
                await PayrollOtherDeduction.EncodeLoansPayrollOtherDeductionLines(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
