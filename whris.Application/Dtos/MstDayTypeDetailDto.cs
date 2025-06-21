using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstDayTypeDetailDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string DayType { get; set; } = null!;

        [Column(TypeName = "decimal(18, 4)")]
        public decimal WorkingDays { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal RestdayDays { get; set; }

        public int EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        public bool IsLocked { get; set; }

        [InverseProperty("DayType")]
        public virtual List<MstDayTypeDayDto> MstDayTypeDays { get; } = new List<MstDayTypeDayDto>();

    }
}
