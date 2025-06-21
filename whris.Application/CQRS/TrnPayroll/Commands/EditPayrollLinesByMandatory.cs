using Azure.Core;
using MediatR;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class EditPayrollLinesByMandatory : IRequest<int>
    {
        public int MandatoryType { get; set; }
        public int PayrollId { get; set; }
        public int? EmployeeId { get; set; }

        public bool IsProcessInMonth { get; set; }

        public class EditPayrollLinesByMandatoryHandler : IRequestHandler<EditPayrollLinesByMandatory, int>
        {
            private readonly HRISContext _context;
            public EditPayrollLinesByMandatoryHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(EditPayrollLinesByMandatory command, CancellationToken cancellationToken)
            {
                switch (command.MandatoryType) 
                {
                    case 1:
                        await Payroll.ProcessSSS(command);
                        break;
                    case 2:
                        await Payroll.ProcessPHIC(command);
                        break;
                    case 3:
                        await Payroll.ProcessHDMF(command);
                        break;
                }

                return await Task.Run(() => 0);
            }
        }
    }
}
