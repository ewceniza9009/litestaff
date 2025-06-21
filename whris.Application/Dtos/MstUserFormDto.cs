using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Application.Common;

namespace whris.Application.Dtos
{
    public class MstUserFormDto
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int FormId { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanLock { get; set; }

        public bool CanUnlock { get; set; }

        public bool CanPreview { get; set; }

        public bool CanPrint { get; set; }

        public bool CanView { get; set; }

        [NotMapped]
        public string FormName => Lookup.GetFormNameById(FormId) ?? "";

        [NotMapped]
        public bool IsDeleted { get; set; } = false;
    }
}
