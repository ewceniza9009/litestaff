using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Commands
{
    public class AddPayrollOtherDeduction : IRequest<TrnPayrollOtherDeductionDetailDto>
    {
        public string AspUserId { get; set; } = string.Empty;
        public int PayrollGroupId { get; set; }

        public class AddPayrollOtherDeductionHandler : IRequestHandler<AddPayrollOtherDeduction, TrnPayrollOtherDeductionDetailDto>
        {
            private readonly HRISContext _context;
            public AddPayrollOtherDeductionHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnPayrollOtherDeductionDetailDto> Handle(AddPayrollOtherDeduction command, CancellationToken cancellationToken)
            {
                var periodId = _context.MstPeriods
                    .OrderBy(x => x.Period)
                    .LastOrDefault()?.Id ?? 0;

                var newPayrollOtherDeduction = new TrnPayrollOtherDeductionDetailDto()
                {
                    Id = 0,
                    PeriodId = periodId,
                    Podnumber = "NA",
                    Poddate = DateTime.Now,
                    PayrollGroupId = command.PayrollGroupId,
                    Remarks = "NA",
                    IsLocked = false,
                    PreparedBy = Lookup.GetUserIdByAspUserId(command.AspUserId)
                };

                return await Task.Run(() => newPayrollOtherDeduction);
            }
        }
    }
}
