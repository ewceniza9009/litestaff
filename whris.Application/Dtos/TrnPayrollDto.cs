using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnPayrollDto
    {
        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        public int MonthId { get; set; }

        [StringLength(50)]
        public string PayrollNumber { get; set; } = null!;

        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PayrollDate { get; set; }

        public int PayrollGroupId { get; set; }

        [Column("DTRId")]
        public int? Dtrid { get; set; }

        public int? PayrollOtherIncomeId { get; set; }

        public int? PayrollOtherDeductionId { get; set; }

        public string? Remarks { get; set; }

        public int? PreparedBy { get; set; }

        public int? CheckedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public bool IsLocked { get; set; }
    }
}
