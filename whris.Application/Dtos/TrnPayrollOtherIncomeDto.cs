using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnPayrollOtherIncomeDto
    {

        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("POINumber")]
        [StringLength(50)]
        public string Poinumber { get; set; } = null!;

        [Column("POIDate", TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Poidate { get; set; }

        public int PayrollGroupId { get; set; }

        public string? Remarks { get; set; }

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public bool IsLocked { get; set; }
    }
}
