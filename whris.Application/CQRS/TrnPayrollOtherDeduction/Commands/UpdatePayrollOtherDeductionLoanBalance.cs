using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Commands
{
    public class UpdatePayrollOtherDeductionLoanBalance : IRequest
    {
        public int PayrollOtherDeductionId { get; set; }

        public class UpdatePayrollOtherDeductionLoanBalanceHandler : IRequestHandler<UpdatePayrollOtherDeductionLoanBalance>
        {
            private readonly HRISContext _context;
            public UpdatePayrollOtherDeductionLoanBalanceHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(UpdatePayrollOtherDeductionLoanBalance command, CancellationToken cancellationToken)
            {                
                await Library.PayrollOtherDeduction.UpdateLoanBalance(command.PayrollOtherDeductionId, _context);

                return Unit.Value;
            }
        }
    }
}
