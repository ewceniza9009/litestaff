using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstCompanyDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Company { get; set; } = null!;

        [StringLength(255)]
        public string Address { get; set; } = null!;

        public bool IsLocked { get; set; }
    }
}
