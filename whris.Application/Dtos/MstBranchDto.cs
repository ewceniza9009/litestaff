using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstBranchDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Branch { get; set; } = null!;

        public int CompanyId { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
