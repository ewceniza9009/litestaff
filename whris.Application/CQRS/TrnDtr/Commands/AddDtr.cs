using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class AddDtr : IRequest<TrnDtrDetailDto>
    {
        public string AspUserId { get; set; } = string.Empty;
        public int PayrollGroupId { get; set; }

        public class AddDtrHandler : IRequestHandler<AddDtr, TrnDtrDetailDto>
        {
            private readonly HRISContext _context;
            public AddDtrHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnDtrDetailDto> Handle(AddDtr command, CancellationToken cancellationToken)
            {
                var periodId = _context.MstPeriods
                    .OrderBy(x => x.Period)
                    .LastOrDefault()?.Id ?? 0;

                var newDtr = new TrnDtrDetailDto()
                {
                    Id = 0,
                    PeriodId = periodId,
                    Dtrnumber = "NA",
                    Dtrdate = DateTime.Now,
                    PayrollGroupId = command.PayrollGroupId,
                    DateStart = DateTime.Now,
                    DateEnd = DateTime.Now,
                    Remarks = "NA",
                    IsLocked = false,
                    PreparedBy = Lookup.GetUserIdByAspUserId(command.AspUserId)
                };

                return await Task.Run(() => newDtr);
            }
        }
    }
}
