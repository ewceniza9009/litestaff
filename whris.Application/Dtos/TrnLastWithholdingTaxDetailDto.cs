using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnLastWithholdingTaxDetailDto
    {

        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("LWTNumber")]
        [StringLength(50)]
        public string Lwtnumber { get; set; } = null!;

        [Column("LWTDate", TypeName = "datetime")]
        public DateTime Lwtdate { get; set; }

        public int PayrollGroupId { get; set; }

        public string? Remarks { get; set; }

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public int EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        public bool IsLocked { get; set; }

        public virtual List<TrnLastWithholdingTaxLineDto> TrnLastWithholdingTaxLines { get; set; } = new List<TrnLastWithholdingTaxLineDto>();
    }
}
