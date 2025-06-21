using MediatR;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Commands
{
    public class AddOTApplicationsByQuickEncode : IRequest<int>
    {
        public int OTId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? EmployeeId { get; set; }
        public decimal OverTimeHours { get; set; }
        public decimal OvertimeLimitHours { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public class AddOTApplicationLinesHandler : IRequestHandler<AddOTApplicationsByQuickEncode, int>
        {
            private readonly HRISContext _context;
            public AddOTApplicationLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(AddOTApplicationsByQuickEncode command, CancellationToken cancellationToken)
            {
                await Overtime.EncodeOTApplicationLines(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
