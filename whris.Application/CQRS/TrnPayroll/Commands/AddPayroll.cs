using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class AddPayroll : IRequest<TrnPayrollDetailDto>
    {
        public string AspUserId { get; set; } = string.Empty;
        public int PayrollGroupId { get; set; }

        public class AddPayrollHandler : IRequestHandler<AddPayroll, TrnPayrollDetailDto>
        {
            private readonly HRISContext _context;
            public AddPayrollHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnPayrollDetailDto> Handle(AddPayroll command, CancellationToken cancellationToken)
            {
                var periodId = _context.MstPeriods
                    .OrderBy(x => x.Period)
                    .LastOrDefault()?.Id ?? 0;

                var newPayroll = new TrnPayrollDetailDto()
                {
                    Id = 0,
                    PeriodId = periodId,
                    PayrollNumber = "NA",
                    PayrollDate = DateTime.Now,
                    PayrollGroupId = command.PayrollGroupId,
                    MonthId = DateTime.Now.Month,
                    Remarks = "NA",
                    IsLocked = false,
                    PreparedBy = Lookup.GetUserIdByAspUserId(command.AspUserId)
                };

                return await Task.Run(() => newPayroll);
            }
        }
    }
}
