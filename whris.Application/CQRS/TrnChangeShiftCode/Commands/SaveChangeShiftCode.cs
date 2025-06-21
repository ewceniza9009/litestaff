using MediatR;
using System.Security.Cryptography.X509Certificates;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Commands
{
    public class SaveChangeShiftCode : IRequest<int>
    {
        public TrnChangeShiftCodeDetailDto? ChangeShiftCode { get; set; }

        public class SaveChangeShiftCodeHandler : IRequestHandler<SaveChangeShiftCode, int>
        {
            private readonly HRISContext _context;
            public SaveChangeShiftCodeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveChangeShiftCode command, CancellationToken cancellationToken)
            {
                var csNumber = "NA";

                if (command?.ChangeShiftCode?.Csnumber == "NA") 
                {
                    var strChangeShiftCodeNumber = _context.TrnChangeShifts
                        .Where(x => x.Csnumber != "NA" && x.PeriodId == command.ChangeShiftCode.PeriodId)
                        .Max(x => x.Csnumber)
                        ?? "NA";

                    var period = _context.MstPeriods
                        .FirstOrDefault(x => x.Id == command.ChangeShiftCode.PeriodId)
                        ?.Period;

                    if (strChangeShiftCodeNumber == "NA")
                    {
                        csNumber = $"{period}-000001";
                    }
                    else 
                    {
                        var trimmedStrNumber = Int64.Parse(strChangeShiftCodeNumber.Substring(5, 6)) + 1_000_001;

                        csNumber = $"{period}-{trimmedStrNumber.ToString().Substring(1, 6)}";
                    }

                    command.ChangeShiftCode.Csnumber = csNumber;
                }

                var newChangeShiftCode = command?.ChangeShiftCode ?? new TrnChangeShiftCodeDetailDto();
                newChangeShiftCode.TrnChangeShiftLines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newChangeShiftCode);

                var mappingProfile = new MappingProfileForTrnChangeShiftCodeDetailReverse();

                if (newChangeShiftCode.Id == 0)
                {
                    var addedChangeShiftCode = mappingProfile.mapper.Map<Data.Models.TrnChangeShift>(newChangeShiftCode);
                    await _context.TrnChangeShifts.AddAsync(addedChangeShiftCode ?? new Data.Models.TrnChangeShift());
                }
                else 
                {
                    var oldChangeShiftCode = _context.TrnChangeShifts.Find(command?.ChangeShiftCode?.Id ?? 0);
                    mappingProfile.mapper.Map(newChangeShiftCode, oldChangeShiftCode);
                }

                await _context.SaveChangesAsync();

                var deletedChangeShiftCodeFormIds = newChangeShiftCode.TrnChangeShiftLines.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedChangeShiftCodeFormRange = _context.TrnChangeShiftLines.Where(x => deletedChangeShiftCodeFormIds.Contains(x.Id)).ToList();

                _context.TrnChangeShiftLines.RemoveRange(deletedChangeShiftCodeFormRange);

                await _context.SaveChangesAsync();

                return command?.ChangeShiftCode?.Id ?? 0;
            }
        }
    }
}
