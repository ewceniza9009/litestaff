using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Data.Models;

namespace whris.Application.Dtos
{
    public class TrnPayrollOtherDeductionDetailDto
    {
        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("PODNumber")]
        [StringLength(50)]
        public string Podnumber { get; set; } = null!;

        [Column("PODDate", TypeName = "datetime")]
        public DateTime Poddate { get; set; }

        public int PayrollGroupId { get; set; }

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

        public virtual List<TrnPayrollOtherDeductionLineDto> TrnPayrollOtherDeductionLines { get; set; } = new List<TrnPayrollOtherDeductionLineDto>();
    }
}
