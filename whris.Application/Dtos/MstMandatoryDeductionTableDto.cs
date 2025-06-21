using whris.Data.Models;

namespace whris.Application.Dtos
{
    public class MstMandatoryDeductionTableDto
    {
        public int Id { get; set; }
        public List<MstTableSss> MstTableSsses { get; set; } = new List<MstTableSss>();
        public List<MstTableHdmf> MstTableHdmfs { get; set; } = new List<MstTableHdmf>();
        public List<MstTablePhic> MstTablePhics { get; set; } = new List<MstTablePhic>();
        public List<MstTableWtaxSemiMonthly> MstTableWtaxSemiMonthlies { get; set; } = new List<MstTableWtaxSemiMonthly>();
        public List<MstTableWtaxMonthlyDto> MstTableWtaxMonthlies { get; set; } = new List<MstTableWtaxMonthlyDto>();
        public List<MstTableWtaxYearly> MstTableWtaxYearlies { get; set; } = new List<MstTableWtaxYearly>();
        public List<MstTaxCodeDto> MstTaxCodes { get; set; } = new List<MstTaxCodeDto>();
    }
}
