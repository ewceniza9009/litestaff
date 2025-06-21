using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstPayrollGroupDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string PayrollGroup { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
