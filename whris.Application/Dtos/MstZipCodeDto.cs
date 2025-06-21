using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstZipCodeDto
    {


        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string ZipCode { get; set; } = null!;

        [StringLength(255)]
        public string Location { get; set; } = null!;

        [StringLength(255)]
        public string Area { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
