using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnLoanDto
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; } = null!;

        public int OtherDeductionId { get; set; }
        public string OtherDeduction { get; set; } = null!;

        public int? LoanNumber { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal LoanAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal MonthlyAmortization { get; set; }

        public int NumberOfMonths { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateStart { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalPayment { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Balance { get; set; }

        public string? Remarks { get; set; }

        public bool IsPaid { get; set; }

        public bool IsLocked { get; set; }
    }
}
