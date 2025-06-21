using MediatR;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Commands
{
    public class AddLeaveApplicationsByQuickEncode : IRequest<int>
    {
        public int LAId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? EmployeeId { get; set; }
        public int LeaveId { get; set; }
        public decimal NumberOfHours { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool WithPay { get; set; }
        public bool DebitToLedger { get; set; }
        public string? Remarks { get; set; }

        public class AddLeaveApplicationLinesHandler : IRequestHandler<AddLeaveApplicationsByQuickEncode, int>
        {
            private readonly HRISContext _context;
            public AddLeaveApplicationLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(AddLeaveApplicationsByQuickEncode command, CancellationToken cancellationToken)
            {
                await Leave.EncodeLeaveApplicationLines(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
