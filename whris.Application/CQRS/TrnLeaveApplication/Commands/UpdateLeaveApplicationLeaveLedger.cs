using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Commands
{
    public class UpdateLeaveApplicationLeaveLedger : IRequest
    {
        public int LeaveApplicationId { get; set; }

        public class UpdateLeaveApplicationLeaveLedgerHandler : IRequestHandler<UpdateLeaveApplicationLeaveLedger>
        {
            private readonly HRISContext _context;
            public UpdateLeaveApplicationLeaveLedgerHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(UpdateLeaveApplicationLeaveLedger command, CancellationToken cancellationToken)
            {
                await Library.Leave.LeaveLedgerLA(command.LeaveApplicationId, _context);

                return Unit.Value;
            }
        }
    }
}
