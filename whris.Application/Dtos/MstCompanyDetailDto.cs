using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstCompanyDetailDto
    {

        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Company { get; set; } = null!;

        [StringLength(50)]
        public string? ContactNumber { get; set; }

        [StringLength(255)]
        public string Address { get; set; } = null!;

        [Column("SSSNumber")]
        [StringLength(50)]
        public string Sssnumber { get; set; } = null!;

        [Column("PHICNumber")]
        [StringLength(50)]
        public string Phicnumber { get; set; } = null!;

        [Column("HDMFNumber")]
        [StringLength(50)]
        public string Hdmfnumber { get; set; } = null!;

        [Column("TIN")]
        [StringLength(50)]
        public string Tin { get; set; } = null!;

        public int EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        [Column("DTRNoField")]
        [StringLength(50)]
        public string? DtrnoField { get; set; }

        [Column("DTRDateTimeField")]
        [StringLength(50)]
        public string? DtrdateTimeField { get; set; }

        [Column("DTRLogTypeField")]
        [StringLength(50)]
        public string? DtrlogTypeField { get; set; }

        public bool IsLocked { get; set; }

        [StringLength(200)]
        public string? OldImageLogo { get; set; }

        [StringLength(200)]
        public string? ImageLogo { get; set; }

        [StringLength(100)]
        public string? CEO { get; set; }

        [StringLength(100)]
        public string? HRManager { get; set; }

        [StringLength(100)]
        public string? Staff { get; set; }

        public int MandatoryDeductionDivisor { get; set; }

		public bool IsComputeNightOvertimeOnNonRegularDays { get; set; }

        public bool IsComputePhicByPercentage { get; set; }

        public decimal PhicPercentage { get; set; }

        public bool IsHolidayPayLateDeducted { get; set; }

        [InverseProperty("Company")]
        public virtual List<MstBranchDto> MstBranches { get; } = new List<MstBranchDto>();
    }
}
