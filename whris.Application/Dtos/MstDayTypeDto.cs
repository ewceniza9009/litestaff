using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstDayTypeDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string DayType { get; set; } = null!;

        [Column(TypeName = "decimal(18, 4)")]
        public decimal WorkingDays { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal RestdayDays { get; set; }

        public bool IsLocked { get; set; }
    }
}
