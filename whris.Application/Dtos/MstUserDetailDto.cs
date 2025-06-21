using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public  class MstUserDetailDto
    {

        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string UserName { get; set; } = null!;

        [StringLength(50)]
        public string Password { get; set; } = null!;

        [StringLength(255)]
        public string FullName { get; set; } = null!;

        [StringLength(450)]
        public string? ASPUserId { get; set; }

        public int EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        public bool IsLocked { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanEditDtrTime { get; set; }

        public virtual List<MstUserFormDto> MstUserForms { get; set; } = new List<MstUserFormDto>();
    }
}
