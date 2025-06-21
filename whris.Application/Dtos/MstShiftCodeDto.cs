using System.ComponentModel.DataAnnotations;

namespace whris.Application.Dtos
{
    public class MstShiftCodeDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string ShiftCode { get; set; } = null!;
        public string Remarks { get; set; } = null!;
        public bool IsLocked { get; set; }
    }
}
