using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstDivisionDto
    {

        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Division { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
