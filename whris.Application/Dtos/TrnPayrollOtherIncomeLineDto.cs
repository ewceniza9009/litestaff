using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Application.Common;
using whris.Data.Data;

namespace whris.Application.Dtos
{
    public class TrnPayrollOtherIncomeLineDto
    {
        [Key]
        public int Id { get; set; }

        public int PayrollOtherIncomeId { get; set; }

        public int EmployeeId { get; set; }

        public int OtherIncomeId { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string? EmployeeName => Lookup.GetEmployeeNameById(EmployeeId);
        public string OtherIncomeText => new HRISContext().MstOtherIncomes.FirstOrDefault(x => x.Id == OtherIncomeId)?.OtherIncome ?? "NA";
    }
}
