using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Commands
{
    public class AddPayrollOtherDeductionLine : IRequest<TrnPayrollOtherDeductionLineDto>
    {
        public int PayrollOtherDeductionId { get; set; }

        public class AddPayrollOtherDeductionLineHandler : IRequestHandler<AddPayrollOtherDeductionLine, TrnPayrollOtherDeductionLineDto>
        {
            private readonly HRISContext _context;

            public AddPayrollOtherDeductionLineHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnPayrollOtherDeductionLineDto> Handle(AddPayrollOtherDeductionLine command, CancellationToken cancellationToken)
            {
                var result = new TrnPayrollOtherDeductionLineDto()
                {
                    Id = 0,
                    PayrollOtherDeductionId = command.PayrollOtherDeductionId,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    OtherDeductionId = _context.MstOtherDeductions.FirstOrDefault()?.Id ?? 0,
                    Amount = 8,
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
