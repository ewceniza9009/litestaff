using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Commands
{
    public class AddLeaveApplication : IRequest<TrnLeaveApplicationDetailDto>
    {
        public string AspUserId { get; set; } = string.Empty;
        public int PayrollGroupId { get; set; }

        public class AddLeaveApplicationHandler : IRequestHandler<AddLeaveApplication, TrnLeaveApplicationDetailDto>
        {
            private readonly HRISContext _context;
            public AddLeaveApplicationHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnLeaveApplicationDetailDto> Handle(AddLeaveApplication command, CancellationToken cancellationToken)
            {
                var periodId = _context.MstPeriods
                    .OrderBy(x => x.Period)
                    .LastOrDefault()?.Id ?? 0;

                var newLeaveApplication = new TrnLeaveApplicationDetailDto()
                {
                    Id = 0,
                    PeriodId = periodId,
                    Lanumber = "NA",
                    Ladate = DateTime.Now,
                    PayrollGroupId = command.PayrollGroupId,
                    Remarks = "NA",
                    IsLocked = false,
                    PreparedBy = Lookup.GetUserIdByAspUserId(command.AspUserId)
                };

                return await Task.Run(() => newLeaveApplication);
            }
        }
    }
}
