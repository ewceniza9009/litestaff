using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class TmpOtherIncome
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string OtherIncome { get; set; } = null!;

        public bool Taxable { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TaxCeiling { get; set; }

        public bool IncludeMandatory { get; set; }

        public int AccountId { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        public bool IsSelected { get; set; }
    }
}
