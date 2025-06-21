using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnOTApplicationDto
    {
        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("OTNumber")]
        [StringLength(50)]
        public string Otnumber { get; set; } = null!;

        [Column("OTDate", TypeName = "datetime")]

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Otdate { get; set; }

        public int PayrollGroupId { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; } = null!;

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public bool IsLocked { get; set; }
    }
}
