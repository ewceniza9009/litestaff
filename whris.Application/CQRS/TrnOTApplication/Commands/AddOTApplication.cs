using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Commands
{
    public class AddOTApplication : IRequest<TrnOTApplicationDetailDto>
    {
        public string AspUserId { get; set; } = string.Empty;
        public int PayrollGroupId { get; set; }

        public class AddOTApplicationHandler : IRequestHandler<AddOTApplication, TrnOTApplicationDetailDto>
        {
            private readonly HRISContext _context;
            public AddOTApplicationHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnOTApplicationDetailDto> Handle(AddOTApplication command, CancellationToken cancellationToken)
            {
                var periodId = _context.MstPeriods
                    .OrderBy(x => x.Period)
                    .LastOrDefault()?.Id ?? 0;

                var newOTApplication = new TrnOTApplicationDetailDto()
                {
                    Id = 0,
                    PeriodId = periodId,
                    Otnumber = "NA",
                    Otdate = DateTime.Now,
                    PayrollGroupId = command.PayrollGroupId,
                    Remarks = "NA",
                    IsLocked = false,
                    PreparedBy = Lookup.GetUserIdByAspUserId(command.AspUserId)
                };

                return await Task.Run(() => newOTApplication);
            }
        }
    }
}
