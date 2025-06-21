using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Commands
{
    public class DeleteEmployee : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployee, int>
        {
            private readonly HRISContext _context;
            public DeleteEmployeeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteEmployee command, CancellationToken cancellationToken)
            {
                var deleteEmployee = _context.MstEmployees.Find(command.Id) ?? new Data.Models.MstEmployee();

                var deleteEmployeeRecordsInPayroll = _context.TrnPayrollLines.Where(x => x.EmployeeId == command.Id);
                var deleteEmployeeRecordsInDtr = _context.TrnDtrlines.Where(x => x.EmployeeId == command.Id);

                _context.TrnPayrollLines.RemoveRange(deleteEmployeeRecordsInPayroll);
                _context.TrnDtrlines.RemoveRange(deleteEmployeeRecordsInDtr);
                _context.MstEmployees.Remove(deleteEmployee);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
