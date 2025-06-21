using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnLastWithholdingTaxDto
    {

        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("LWTNumber")]
        [StringLength(50)]
        public string Lwtnumber { get; set; } = null!;

        [Column("LWTDate", TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Lwtdate { get; set; }

        public int PayrollGroupId { get; set; }

        public string? Remarks { get; set; }

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public bool IsLocked { get; set; }
    }
}
