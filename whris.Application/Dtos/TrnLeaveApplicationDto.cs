using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnLeaveApplicationDto
    {

        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("LANumber")]
        [StringLength(50)]
        public string Lanumber { get; set; } = null!;

        [Column("LADate", TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Ladate { get; set; }

        public int PayrollGroupId { get; set; }

        public string Remarks { get; set; } = null!;

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public bool IsLocked { get; set; }
    }
}
