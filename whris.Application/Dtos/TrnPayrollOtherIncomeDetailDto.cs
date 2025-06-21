using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Data.Models;

namespace whris.Application.Dtos
{
    public class TrnPayrollOtherIncomeDetailDto
    {
        [Key]
        public int Id { get; set; }

        public int PeriodId { get; set; }

        [Column("POINumber")]
        [StringLength(50)]
        public string Poinumber { get; set; } = null!;

        [Column("POIDate", TypeName = "datetime")]
        public DateTime Poidate { get; set; }

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

        public virtual List<TrnPayrollOtherIncomeLineDto> TrnPayrollOtherIncomeLines { get; set; } = new List<TrnPayrollOtherIncomeLineDto>();
    }
}
