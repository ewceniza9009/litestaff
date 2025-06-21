using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstOtherDeductionDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string OtherDeduction { get; set; } = null!;

        public bool LoanType { get; set; }

        public int AccountId { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public bool IsDeleted { get; set; }
    }
}
