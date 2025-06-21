using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstShiftCodeDetailDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string ShiftCode { get; set; } = null!;

        public string Remarks { get; set; } = null!;

        public int EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        public bool IsLocked { get; set; }

        public virtual List<MstShiftCodeDayDto> MstShiftCodeDays { get; set; } = new List<MstShiftCodeDayDto>();
    }
}
