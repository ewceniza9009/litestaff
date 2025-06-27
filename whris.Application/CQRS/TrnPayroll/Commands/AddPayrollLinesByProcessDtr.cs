using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class AddPayrollLinesByProcessDtr : IRequest<List<TrnPayrollLineDto>>
    {
        public int PayrollId { get; set; }
        public int PayrollGroupId { get; set; }
        public int? DtrId { get; set; }
        public int? EmployeeId { get; set; }

        public class AddPayrollLinesByProcessDtrHandler : IRequestHandler<AddPayrollLinesByProcessDtr, List<TrnPayrollLineDto>>
        {
            private readonly HRISContext _context;
            public AddPayrollLinesByProcessDtrHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<List<TrnPayrollLineDto>> Handle(AddPayrollLinesByProcessDtr command, CancellationToken cancellationToken)
            {
                var payrollLines = new List<TrnPayrollLineDto>();

                await Payroll.ProcessDtrLines(command, _context);

                return await Task.Run(() => payrollLines);
            }
        }
    }
}
