using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstReligionDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Religion { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
