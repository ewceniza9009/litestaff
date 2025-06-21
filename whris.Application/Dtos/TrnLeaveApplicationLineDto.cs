using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Application.Common;

namespace whris.Application.Dtos
{
    public class TrnLeaveApplicationLineDto
    {
        [Key]
        public int Id { get; set; }

        public int LeaveApplicationId { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveId { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal NumberOfHours { get; set; }

        public bool WithPay { get; set; }

        public bool DebitToLedger { get; set; }

        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string? EmployeeName => Lookup.GetEmployeeNameById(EmployeeId);
    }
}
