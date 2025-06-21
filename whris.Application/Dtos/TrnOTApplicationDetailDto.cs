using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnOTApplicationDetailDto
    {

        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("OTNumber")]
        [StringLength(50)]
        public string Otnumber { get; set; } = null!;

        [Column("OTDate", TypeName = "datetime")]
        public DateTime Otdate { get; set; }

        public int PayrollGroupId { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; } = null!;

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

        public virtual List<TrnOTApplicationLineDto> TrnOverTimeLines { get; set; } = new List<TrnOTApplicationLineDto>();
    }
}
