using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstShiftCodeDayDto
    {
        public int Id { get; set; }

        public int ShiftCodeId { get; set; }

        public bool RestDay { get; set; }

        [StringLength(50)]
        public string Day { get; set; } = null!;

        [Column(TypeName = "datetime")]
        [DataType(DataType.Time)]
        public DateTime? TimeIn1 { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.Time)]
        public DateTime? TimeOut1 { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.Time)]
        public DateTime? TimeIn2 { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.Time)]
        public DateTime? TimeOut2 { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal NumberOfHours { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal LateFlexibility { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal LateGraceMinute { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal NightHours { get; set; }

        public bool IsTommorow { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
