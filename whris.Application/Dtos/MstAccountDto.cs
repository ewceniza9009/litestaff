using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstAccountDto
    {

        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string AccountCode { get; set; } = null!;

        [StringLength(255)]
        public string Account { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
