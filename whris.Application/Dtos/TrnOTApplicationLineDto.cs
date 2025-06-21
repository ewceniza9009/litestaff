using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Application.Common;

namespace whris.Application.Dtos
{
    public class TrnOTApplicationLineDto
    {
        [Key]
        public int Id { get; set; }

        public int OverTimeId { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal OvertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal OvertimeRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal OvertimeAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal OvertimeNightHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal OvertimeLimitHours { get; set; }

        public string? Remarks { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string? EmployeeName => Lookup.GetEmployeeNameById(EmployeeId);
    }
}
