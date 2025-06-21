using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstPayrollTypeDto
    {

        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string PayrollType { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
