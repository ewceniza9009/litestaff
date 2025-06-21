using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Data.Models;

namespace whris.Application.Dtos
{
    public class TrnPayrollDetailDto
    {
        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        public int MonthId { get; set; }

        [StringLength(50)]
        public string PayrollNumber { get; set; } = null!;

        [Column(TypeName = "datetime")]
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

        public int EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        public bool IsLocked { get; set; }

        public int? LastWithholdingTaxId { get; set; }

        public bool IsApproved { get; set; }

        public virtual List<TrnPayrollLineDto> TrnPayrollLines { get; set; } = new List<TrnPayrollLineDto>();
    }
}
