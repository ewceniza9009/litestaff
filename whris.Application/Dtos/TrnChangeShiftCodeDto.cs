using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnChangeShiftCodeDto
    {
        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("CSNumber")]
        [StringLength(50)]
        public string Csnumber { get; set; } = null!;

        [Column("CSDate", TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Csdate { get; set; }

        public int PayrollGroupId { get; set; }

        public string Remarks { get; set; } = null!;

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public bool IsLocked { get; set; }
    }
}
