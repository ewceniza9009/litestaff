using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstOtherIncomeDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string OtherIncome { get; set; } = null!;

        public bool Taxable { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal TaxCeiling { get; set; }

        public bool IncludeMandatory { get; set; }

        public int AccountId { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public bool IsDeleted { get; set; }
    }
}
