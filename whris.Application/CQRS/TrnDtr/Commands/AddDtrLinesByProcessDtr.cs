using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class AddDtrLinesByProcessDtr : IRequest<List<TrnDtrLineDto>>
    {
        public int DTRId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int? ChangeShiftId { get; set; }
        public List<TmpDtrLogs>? TmpDtrLogs { get; set; }

        public class AddDtrLinesHandler : IRequestHandler<AddDtrLinesByProcessDtr, List<TrnDtrLineDto>>
        {
            private readonly HRISContext _context;
            public AddDtrLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<List<TrnDtrLineDto>> Handle(AddDtrLinesByProcessDtr command, CancellationToken cancellationToken)
            {
                var dtrLines = new List<TrnDtrLineDto>();
                
                if (command?.TmpDtrLogs is null)
                {
                    return dtrLines;
                }

                DTR.ProcessDtrLog(command, dtrLines, _context);
                DTR.ProcessDtrLines(command.ChangeShiftId ?? 0, command.TmpDtrLogs, dtrLines, command.DateStart, command.DateEnd, _context);

                return await Task.Run(() => dtrLines);
            }
        }
    }
}
