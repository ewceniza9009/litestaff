using MediatR;
using System.Security.Cryptography.X509Certificates;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Commands
{
    public class SaveOTApplication : IRequest<int>
    {
        public TrnOTApplicationDetailDto? OTApplication { get; set; }

        public class SaveOTApplicationHandler : IRequestHandler<SaveOTApplication, int>
        {
            private readonly HRISContext _context;
            public SaveOTApplicationHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveOTApplication command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var otNumber = "NA";

                if (command?.OTApplication?.Otnumber == "NA") 
                {
                    var strOTApplicationNumber = _context.TrnOverTimes
                        .Where(x => x.Otnumber != "NA" && x.PeriodId == command.OTApplication.PeriodId)
                        .Max(x => x.Otnumber)
                        ?? "NA";

                    var period = _context.MstPeriods
                        .FirstOrDefault(x => x.Id == command.OTApplication.PeriodId)
                        ?.Period;

                    if (strOTApplicationNumber == "NA")
                    {
                        otNumber = $"{period}-000001";
                    }
                    else 
                    {
                        var trimmedStrNumber = Int64.Parse(strOTApplicationNumber.Substring(5, 6)) + 1_000_001;

                        otNumber = $"{period}-{trimmedStrNumber.ToString().Substring(1, 6)}";
                    }

                    command.OTApplication.Otnumber = otNumber;
                }

                var newOTApplication = command?.OTApplication ?? new TrnOTApplicationDetailDto();
                newOTApplication.TrnOverTimeLines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newOTApplication);

                var mappingProfile = new MappingProfileForTrnOTApplicationDetailReverse();

                if (newOTApplication.Id == 0)
                {
                    var addedOTApplication = mappingProfile.mapper.Map<Data.Models.TrnOverTime>(newOTApplication);
                    await _context.TrnOverTimes.AddAsync(addedOTApplication ?? new Data.Models.TrnOverTime());

                    await _context.SaveChangesAsync();

                    resultId = addedOTApplication?.Id ?? 0;
                }
                else 
                {
                    var oldOTApplication = await _context.TrnOverTimes.FindAsync(command?.OTApplication?.Id ?? 0);
                    mappingProfile.mapper.Map(newOTApplication, oldOTApplication);

                    await _context.SaveChangesAsync();

                    resultId = oldOTApplication?.Id ?? 0;
                }

                var deletedOTApplicationFormIds = newOTApplication.TrnOverTimeLines.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedOTApplicationFormRange = _context.TrnOverTimeLines.Where(x => deletedOTApplicationFormIds.Contains(x.Id)).ToList();

                _context.TrnOverTimeLines.RemoveRange(deletedOTApplicationFormRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
