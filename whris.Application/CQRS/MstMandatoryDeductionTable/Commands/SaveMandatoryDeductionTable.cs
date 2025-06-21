using MediatR;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.MstMandatoryDeductionTable.Commands
{
    public class SaveMandatoryDeductionTable : IRequest<int>
    {
        public MstMandatoryDeductionTableDto? MandatoryDeductionTable { get; set; }

        public class SaveMandatoryDeductionTableHandler : IRequestHandler<SaveMandatoryDeductionTable, int>
        {
            private readonly HRISContext _context;
            public SaveMandatoryDeductionTableHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveMandatoryDeductionTable command, CancellationToken cancellationToken)
            {
                var mappingProfileSss = new MappingProfile<MstTableSss, MstTableSss>();
                var mappingProfileHdmf = new MappingProfile<MstTableHdmf, MstTableHdmf>();
                var mappingProfilePhic = new MappingProfile<MstTablePhic, MstTablePhic>();
                var mappingProfileSemi = new MappingProfile<MstTableWtaxSemiMonthly, MstTableWtaxSemiMonthly>();
                var mappingProfileMonthly = new MappingProfile<MstTableWtaxMonthlyDto, MstTableWtaxMonthly>();
                var mappingProfileYearly = new MappingProfile<MstTableWtaxYearly, MstTableWtaxYearly>();
                var mappingProfileTaxCode = new MappingProfile<MstTaxCodeDto, MstTaxCode>();

                foreach (var item in command.MandatoryDeductionTable?.MstTableSsses ?? new List<MstTableSss>()) 
                {
                    var oldItem = _context.MstTableSsses
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstTableSss();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstTableSsses.AddRangeAsync(item);
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileSss.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstTableSsses.Remove(oldItem);
                }

                foreach (var item in command.MandatoryDeductionTable?.MstTableHdmfs ?? new List<MstTableHdmf>())
                {
                    var oldItem = _context.MstTableHdmfs
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstTableHdmf();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstTableHdmfs.AddRangeAsync(item);
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileHdmf.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstTableHdmfs.Remove(oldItem);
                }

                foreach (var item in command.MandatoryDeductionTable?.MstTablePhics ?? new List<MstTablePhic>())
                {
                    var oldItem = _context.MstTablePhics
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstTablePhic();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstTablePhics.AddRangeAsync(item);
                    if (item.Id > 0 && !item.IsDeleted) mappingProfilePhic.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstTablePhics.Remove(oldItem);
                }

                foreach (var item in command.MandatoryDeductionTable?.MstTableWtaxSemiMonthlies ?? new List<MstTableWtaxSemiMonthly>())
                {
                    var oldItem = _context.MstTableWtaxSemiMonthlies
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstTableWtaxSemiMonthly();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstTableWtaxSemiMonthlies.AddRangeAsync(item);
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileSemi.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstTableWtaxSemiMonthlies.Remove(oldItem);
                }

                foreach (var item in command.MandatoryDeductionTable?.MstTableWtaxMonthlies ?? new List<MstTableWtaxMonthlyDto>())
                {
                    var oldItem = _context.MstTableWtaxMonthlies
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstTableWtaxMonthly();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstTableWtaxMonthlies.AddRangeAsync(mappingProfileMonthly.mapper.Map<MstTableWtaxMonthly>(item));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileMonthly.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstTableWtaxMonthlies.Remove(oldItem);
                }

                foreach (var item in command.MandatoryDeductionTable?.MstTableWtaxYearlies ?? new List<MstTableWtaxYearly>())
                {
                    var oldItem = _context.MstTableWtaxYearlies
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstTableWtaxYearly();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstTableWtaxYearlies.AddRangeAsync(item);
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileYearly.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstTableWtaxYearlies.Remove(oldItem);
                }

                foreach (var item in command.MandatoryDeductionTable?.MstTaxCodes ?? new List<MstTaxCodeDto>())
                {
                    var oldItem = _context.MstTaxCodes
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstTaxCode();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstTaxCodes.AddRangeAsync(mappingProfileTaxCode.mapper.Map<MstTaxCode>(item));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileTaxCode.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstTaxCodes.Remove(oldItem);
                }

                await _context.SaveChangesAsync();

                return 0;
            }
        }
    }
}
