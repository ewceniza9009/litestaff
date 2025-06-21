using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class TmpUploadEmployee
    {
        [StringLength(50)]
        public string BiometricIdNumber { get; set; } = null!;

        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        public string MiddleName { get; set; } = null!;

        public string AdditionalName { get; set; } = null!;

        [StringLength(255)]
        public string FullName => $"{LastName}, {FirstName} {MiddleName} {AdditionalName}";

        [Column(TypeName = "decimal(18, 5)")]
        public decimal DailyRate { get; set; }
    }
}
