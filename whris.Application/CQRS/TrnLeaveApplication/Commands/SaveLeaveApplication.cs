using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Commands
{
    public class SaveLeaveApplication : IRequest<int>
    {
        public TrnLeaveApplicationDetailDto? LeaveApplication { get; set; }

        public class SaveLeaveApplicationHandler : IRequestHandler<SaveLeaveApplication, int>
        {
            private readonly HRISContext _context;
            public SaveLeaveApplicationHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveLeaveApplication command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var laNumber = "NA";

                if (command?.LeaveApplication?.Lanumber == "NA") 
                {
                    var strLeaveApplicationNumber = _context.TrnLeaveApplications
                        .Where(x => x.Lanumber != "NA" && x.PeriodId == command.LeaveApplication.PeriodId)
                        .Max(x => x.Lanumber)
                        ?? "NA";

                    var period = _context.MstPeriods
                        .FirstOrDefault(x => x.Id == command.LeaveApplication.PeriodId)
                        ?.Period;

                    if (strLeaveApplicationNumber == "NA")
                    {
                        laNumber = $"{period}-000001";
                    }
                    else 
                    {
                        var trimmedStrNumber = Int64.Parse(strLeaveApplicationNumber.Substring(5, 6)) + 1_000_001;

                        laNumber = $"{period}-{trimmedStrNumber.ToString().Substring(1, 6)}";
                    }

                    command.LeaveApplication.Lanumber = laNumber;
                }

                var newLeaveApplication = command?.LeaveApplication ?? new TrnLeaveApplicationDetailDto();
                newLeaveApplication.TrnLeaveApplicationLines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newLeaveApplication);

                var mappingProfile = new MappingProfileForTrnLeaveApplicationDetailReverse();

                if (newLeaveApplication.Id == 0)
                {
                    var addedLeaveApplication = mappingProfile.mapper.Map<Data.Models.TrnLeaveApplication>(newLeaveApplication);
                    await _context.TrnLeaveApplications.AddAsync(addedLeaveApplication ?? new Data.Models.TrnLeaveApplication());

                    await _context.SaveChangesAsync();

                    resultId = addedLeaveApplication?.Id ?? 0;
                }
                else 
                {
                    var oldLeaveApplication = await _context.TrnLeaveApplications.FindAsync(command?.LeaveApplication?.Id ?? 0);
                    mappingProfile.mapper.Map(newLeaveApplication, oldLeaveApplication);

                    await _context.SaveChangesAsync();

                    resultId = oldLeaveApplication?.Id ?? 0;
                }

                var deletedLeaveApplicationFormIds = newLeaveApplication.TrnLeaveApplicationLines.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedLeaveApplicationFormRange = await _context.TrnLeaveApplicationLines.Where(x => deletedLeaveApplicationFormIds.Contains(x.Id)).ToListAsync();

                _context.TrnLeaveApplicationLines.RemoveRange(deletedLeaveApplicationFormRange);               

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
