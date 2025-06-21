using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class AddPayrollLine : IRequest<TrnPayrollLineDto>
    {
        public int PayrollId { get; set; }

        public class AddPayrollLineHandler : IRequestHandler<AddPayrollLine, TrnPayrollLineDto>
        {
            private readonly HRISContext _context;

            public AddPayrollLineHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnPayrollLineDto> Handle(AddPayrollLine command, CancellationToken cancellationToken)
            {
                var result = new TrnPayrollLineDto()
                {
                    Id = 0,
                    PayrollId = command.PayrollId,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    PayrollTypeId = _context.MstPayrollTypes.FirstOrDefault()?.Id ?? 0,
                    TaxCodeId = _context.MstTaxCodes.FirstOrDefault()?.Id ?? 0,
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
