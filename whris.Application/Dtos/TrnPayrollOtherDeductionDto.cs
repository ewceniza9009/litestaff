using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnPayrollOtherDeductionDto
    {
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("PODNumber")]
        [StringLength(50)]
        public string Podnumber { get; set; } = null!;

        [Column("PODDate", TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Poddate { get; set; }

        public int PayrollGroupId { get; set; }

        public string? Remarks { get; set; }

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public bool IsLocked { get; set; }
    }
}
