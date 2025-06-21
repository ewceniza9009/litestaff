using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Commands
{
    public class AddPayrollOtherIncome : IRequest<TrnPayrollOtherIncomeDetailDto>
    {
        public string AspUserId { get; set; } = string.Empty;
        public int PayrollGroupId { get; set; }

        public class AddPayrollOtherIncomeHandler : IRequestHandler<AddPayrollOtherIncome, TrnPayrollOtherIncomeDetailDto>
        {
            private readonly HRISContext _context;
            public AddPayrollOtherIncomeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnPayrollOtherIncomeDetailDto> Handle(AddPayrollOtherIncome command, CancellationToken cancellationToken)
            {
                var periodId = _context.MstPeriods
                    .OrderBy(x => x.Period)
                    .LastOrDefault()?.Id ?? 0;

                var newPayrollOtherIncome = new TrnPayrollOtherIncomeDetailDto()
                {
                    Id = 0,
                    PeriodId = periodId,
                    Poinumber = "NA",
                    Poidate = DateTime.Now,
                    PayrollGroupId = command.PayrollGroupId,
                    Remarks = "NA",
                    IsLocked = false,
                    PreparedBy = Lookup.GetUserIdByAspUserId(command.AspUserId)
                };

                return await Task.Run(() => newPayrollOtherIncome);
            }
        }
    }
}
