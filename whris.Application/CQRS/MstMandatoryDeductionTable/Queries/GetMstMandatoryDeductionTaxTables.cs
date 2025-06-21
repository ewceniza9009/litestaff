using MediatR;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstMandatoryDeductionTable.Queries
{
    public class GetMstMandatoryTaxTables : IRequest<MstMandatoryDeductionTableDto>
    {
        public class GetMstMandatoryTaxTableHandler : IRequestHandler<GetMstMandatoryTaxTables, MstMandatoryDeductionTableDto>
        {
            private readonly HRISContext _context;
            public GetMstMandatoryTaxTableHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstMandatoryDeductionTableDto> Handle(GetMstMandatoryTaxTables request, CancellationToken cancellationToken)
            {
                var result =  new MstMandatoryDeductionTableDto();

                var mappingProfileMonthly = new MappingProfileForMstTableWtaxMonthly();
                var mappingProfileTaxCode = new MappingProfileForMstTaxCode();

                result.MstTableSsses = _context.MstTableSsses.ToList();
                result.MstTableHdmfs = _context.MstTableHdmfs.ToList();
                result.MstTablePhics = _context.MstTablePhics.ToList();
                result.MstTableWtaxSemiMonthlies = _context.MstTableWtaxSemiMonthlies.ToList();

                result.MstTableWtaxMonthlies = _context.MstTableWtaxMonthlies
                    .Select(x => mappingProfileMonthly.mapper.Map<MstTableWtaxMonthlyDto>(x))
                    .ToList();

                result.MstTableWtaxYearlies = _context.MstTableWtaxYearlies.ToList();

                result.MstTaxCodes = _context.MstTaxCodes
                    .Select(x => mappingProfileTaxCode.mapper.Map<MstTaxCodeDto>(x))
                    .ToList();

                return await Task.Run(() => result);
            }
        }
    }
}
