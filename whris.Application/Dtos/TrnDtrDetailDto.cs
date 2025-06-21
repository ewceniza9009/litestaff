using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnDtrDetailDto
    {

        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("DTRNumber")]
        [StringLength(50)]
        public string Dtrnumber { get; set; } = null!;

        [Column("DTRDate", TypeName = "datetime")]
        public DateTime Dtrdate { get; set; }

        public int PayrollGroupId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateStart { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateEnd { get; set; }

        public int? OvertTimeId { get; set; }

        public int? LeaveApplicationId { get; set; }

        public int? ChangeShiftId { get; set; }

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

        public bool IsComputeRestDay { get; set; }

        public bool IsApproved { get; set; }

        public virtual List<TrnDtrLineDto> TrnDtrlines { get; set; } = new List<TrnDtrLineDto>();
    }
}
