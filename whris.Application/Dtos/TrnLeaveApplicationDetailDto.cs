using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnLeaveApplicationDetailDto
    {

        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("LANumber")]
        [StringLength(50)]
        public string Lanumber { get; set; } = null!;

        [Column("LADate", TypeName = "datetime")]
        public DateTime Ladate { get; set; }

        public int PayrollGroupId { get; set; }

        public string Remarks { get; set; } = null!;

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public int? EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        public bool IsLocked { get; set; }

        public virtual List<TrnLeaveApplicationLineDto> TrnLeaveApplicationLines { get; set; } = new List<TrnLeaveApplicationLineDto>();
    }
}
