using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstDayTypeDayDto
    {
        [Key]
        public int Id { get; set; }

        public int DayTypeId { get; set; }

        public int BranchId { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DateAfter { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DateBefore { get; set; }

        public bool ExcludedInFixed { get; set; }

        [StringLength(50)]
        public string? Remarks { get; set; }

        public bool WithAbsentInFixed { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
