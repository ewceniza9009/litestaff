using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Commands
{
    public class AddChangeShiftCode : IRequest<TrnChangeShiftCodeDetailDto>
    {
        public string AspUserId { get; set; } = string.Empty;
        public int PayrollGroupId { get; set; }

        public class AddChangeShiftCodeHandler : IRequestHandler<AddChangeShiftCode, TrnChangeShiftCodeDetailDto>
        {
            private readonly HRISContext _context;
            public AddChangeShiftCodeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnChangeShiftCodeDetailDto> Handle(AddChangeShiftCode command, CancellationToken cancellationToken)
            {
                var periodId = _context.MstPeriods
                    .OrderBy(x => x.Period)
                    .LastOrDefault()?.Id ?? 0;

                var newChangeShiftCode = new TrnChangeShiftCodeDetailDto()
                {
                    Id = 0,
                    PeriodId = periodId,
                    Csnumber = "NA",
                    Csdate = DateTime.Now,
                    PayrollGroupId = command.PayrollGroupId,
                    Remarks = "NA",
                    IsLocked = true,
                    PreparedBy = Lookup.GetUserIdByAspUserId(command.AspUserId)
                };

                return await Task.Run(() => newChangeShiftCode);
            }
        }
    }
}
