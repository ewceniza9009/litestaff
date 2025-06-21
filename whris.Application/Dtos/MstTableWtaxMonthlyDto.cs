using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstTableWtaxMonthlyDto
    {
        [Key]
        public int Id { get; set; }

        public int TaxCodeId { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal TaxCodeValue { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal Tax { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal Percentage { get; set; }

        public bool IsDeleted { get; set; }
    }
}
