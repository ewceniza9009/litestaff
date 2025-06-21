using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnDtrDto
    {

        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("DTRNumber")]
        [StringLength(50)]
        public string Dtrnumber { get; set; } = null!;

        [Column("DTRDate", TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Dtrdate { get; set; }

        public int PayrollGroupId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime DateStart { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime DateEnd { get; set; }

        public string Remarks { get; set; } = null!;

        public bool IsLocked { get; set; }
    }
}
