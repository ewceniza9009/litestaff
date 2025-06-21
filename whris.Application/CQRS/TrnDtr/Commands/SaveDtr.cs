using MediatR;
using System.Security.Cryptography.X509Certificates;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class SaveDtr : IRequest<int>
    {
        public TrnDtrDetailDto? Dtr { get; set; }

        public class SaveDtrHandler : IRequestHandler<SaveDtr, int>
        {
            private readonly HRISContext _context;
            public SaveDtrHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveDtr command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var dtrNumber = "NA";

                if (command?.Dtr?.Dtrnumber == "NA") 
                {
                    var strDtrNumber = _context.TrnDtrs
                        .Where(x => x.Dtrnumber != "NA" && x.PeriodId == command.Dtr.PeriodId)
                        .Max(x => x.Dtrnumber)
                        ?? "NA";

                    var period = _context.MstPeriods
                        .FirstOrDefault(x => x.Id == command.Dtr.PeriodId)
                        ?.Period;

                    if (strDtrNumber == "NA")
                    {
                        dtrNumber = $"{period}-000001";
                    }
                    else 
                    {
                        var trimmedStrNumber = Int64.Parse(strDtrNumber.Substring(5, 6)) + 1_000_001;

                        dtrNumber = $"{period}-{trimmedStrNumber.ToString().Substring(1, 6)}";
                    }

                    command.Dtr.Dtrnumber = dtrNumber;
                }

                var newDtr = command?.Dtr ?? new TrnDtrDetailDto();
                newDtr.TrnDtrlines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newDtr);

                var mappingProfile = new MappingProfileForTrnDtrDetailReverse();

                if (newDtr.Id == 0)
                {
                    var addedDtr = mappingProfile.mapper.Map<Data.Models.TrnDtr>(newDtr);
                    await _context.TrnDtrs.AddAsync(addedDtr ?? new Data.Models.TrnDtr());

                    await _context.SaveChangesAsync();

                    resultId = addedDtr?.Id ?? 0;
                }
                else 
                {
                    var oldDtr = await _context.TrnDtrs.FindAsync(command?.Dtr?.Id ?? 0);
                    mappingProfile.mapper.Map(newDtr, oldDtr);

                    await _context.SaveChangesAsync();

                    resultId = oldDtr?.Id ?? 0;
                }

                var deletedDtrFormIds = newDtr.TrnDtrlines.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedDtrFormRange = _context.TrnDtrlines.Where(x => deletedDtrFormIds.Contains(x.Id)).ToList();

                _context.TrnDtrlines.RemoveRange(deletedDtrFormRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
