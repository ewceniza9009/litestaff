using MediatR;
using System.Security.Cryptography.X509Certificates;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLastWithholdingTax.Commands
{
    public class SaveLastWithholdingTax : IRequest<int>
    {
        public TrnLastWithholdingTaxDetailDto? LastWithholdingTax { get; set; }

        public class SaveLastWithholdingTaxHandler : IRequestHandler<SaveLastWithholdingTax, int>
        {
            private readonly HRISContext _context;
            public SaveLastWithholdingTaxHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveLastWithholdingTax command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var lwtNumber = "NA";

                if (command?.LastWithholdingTax?.Lwtnumber == "NA") 
                {
                    var strLastWithholdingTaxNumber = _context.TrnLastWithholdingTaxes
                        .Where(x => x.Lwtnumber != "NA" && x.PeriodId == command.LastWithholdingTax.PeriodId)
                        .Max(x => x.Lwtnumber)
                        ?? "NA";

                    var period = _context.MstPeriods
                        .FirstOrDefault(x => x.Id == command.LastWithholdingTax.PeriodId)
                        ?.Period;

                    if (strLastWithholdingTaxNumber == "NA")
                    {
                        lwtNumber = $"{period}-000001";
                    }
                    else 
                    {
                        var trimmedStrNumber = Int64.Parse(strLastWithholdingTaxNumber.Substring(5, 6)) + 1_000_001;

                        lwtNumber = $"{period}-{trimmedStrNumber.ToString().Substring(1, 6)}";
                    }

                    command.LastWithholdingTax.Lwtnumber = lwtNumber;
                }

                var newLastWithholdingTax = command?.LastWithholdingTax ?? new TrnLastWithholdingTaxDetailDto();
                newLastWithholdingTax.TrnLastWithholdingTaxLines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newLastWithholdingTax);

                var mappingProfile = new MappingProfileForTrnLastWithholdingTaxDetailReverse();

                if (newLastWithholdingTax.Id == 0)
                {
                    var addedLastWithholdingTax = mappingProfile.mapper.Map<Data.Models.TrnLastWithholdingTax>(newLastWithholdingTax);
                    await _context.TrnLastWithholdingTaxes.AddAsync(addedLastWithholdingTax ?? new Data.Models.TrnLastWithholdingTax());

                    await _context.SaveChangesAsync();

                    resultId = addedLastWithholdingTax?.Id ?? 0;
                }
                else 
                {
                    var oldLastWithholdingTax = await _context.TrnLastWithholdingTaxes.FindAsync(command?.LastWithholdingTax?.Id ?? 0);
                    mappingProfile.mapper.Map(newLastWithholdingTax, oldLastWithholdingTax);

                    await _context.SaveChangesAsync();

                    resultId = oldLastWithholdingTax?.Id ?? 0;
                }

                var deletedLastWithholdingTaxFormIds = newLastWithholdingTax.TrnLastWithholdingTaxLines.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedLastWithholdingTaxFormRange = _context.TrnLastWithholdingTaxLines.Where(x => deletedLastWithholdingTaxFormIds.Contains(x.Id)).ToList();

                _context.TrnLastWithholdingTaxLines.RemoveRange(deletedLastWithholdingTaxFormRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
