using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstEmployeeMemoDto
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "datetime")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime MemoDate { get; set; }

        [StringLength(255)]
        public string MemoSubject { get; set; } = null!;

        public string? MemoContent { get; set; }

        public int PreparedBy { get; set; }

        public int ApprovedBy { get; set; }

        public string? FilePath { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
