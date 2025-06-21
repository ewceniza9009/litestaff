using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class TrnLoanDetailDto
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int OtherDeductionId { get; set; }

        public int? LoanNumber { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal LoanAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal MonthlyAmortization { get; set; }

        public int NumberOfMonths { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateStart { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal TotalPayment { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal Balance { get; set; }

        public bool IsPaid { get; set; }

        public int EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        public bool IsLocked { get; set; }
        public string? Remarks { get; set; }

        [Column("SSSLoanAmount", TypeName = "decimal(18, 5)")]
        public decimal? SssloanAmount { get; set; }


    }
}
