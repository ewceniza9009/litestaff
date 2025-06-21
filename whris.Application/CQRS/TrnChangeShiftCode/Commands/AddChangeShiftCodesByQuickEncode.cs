using MediatR;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Commands
{
    public class AddChangeShiftCodesByQuickEncode : IRequest<int>
    {
        public int CSId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? EmployeeId { get; set; }
        public int ShiftCodeId { get; set; }
        public decimal NumberOfHours { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public class AddChangeShiftCodeLinesHandler : IRequestHandler<AddChangeShiftCodesByQuickEncode, int>
        {
            private readonly HRISContext _context;
            public AddChangeShiftCodeLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(AddChangeShiftCodesByQuickEncode command, CancellationToken cancellationToken)
            {
                await ChangeShift.EncodeChangeShiftApplicationLines(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
