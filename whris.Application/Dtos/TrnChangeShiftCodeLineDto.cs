using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Application.Common;

namespace whris.Application.Dtos
{
    public class TrnChangeShiftCodeLineDto
    {
        [Key]
        public int Id { get; set; }

        public int ChangeShiftId { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        public int ShiftCodeId { get; set; }

        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string? EmployeeName => Lookup.GetEmployeeNameById(EmployeeId);
    }
}
