using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstEmployeeDetailDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string IdNumber { get; set; } = null!;

        [StringLength(50)]
        public string BiometricIdNumber { get; set; } = null!;

        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        public string MiddleName { get; set; } = null!;

        [StringLength(50)]
        public string? ExtensionName { get; set; }

        [StringLength(255)]
        public string FullName { get; set; } = null!;

        public string Address { get; set; } = null!;

        public int ZipCodeId { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? CellphoneNumber { get; set; }

        [StringLength(50)]
        public string? EmailAddress { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateOfBirth { get; set; }

        public string? PlaceOfBirth { get; set; }

        public int? PlaceOfBirthZipCodeId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateHired { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateResigned { get; set; }

        [StringLength(50)]
        public string Sex { get; set; } = null!;

        [StringLength(50)]
        public string CivilStatus { get; set; } = null!;

        public int CitizenshipId { get; set; }

        public int ReligionId { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal Height { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal Weight { get; set; }

        [Column("GSISNumber")]
        [StringLength(50)]
        public string? Gsisnumber { get; set; }

        [Column("SSSNumber")]
        [StringLength(50)]
        public string? Sssnumber { get; set; }

        [Column("HDMFNumber")]
        [StringLength(50)]
        public string? Hdmfnumber { get; set; }

        [Column("PHICNumber")]
        [StringLength(50)]
        public string? Phicnumber { get; set; }

        [Column("TIN")]
        [StringLength(50)]
        public string? Tin { get; set; }

        public int TaxCodeId { get; set; }

        [Column("ATMAccountNumber")]
        [StringLength(50)]
        public string? AtmaccountNumber { get; set; }

        public int CompanyId { get; set; }

        public int BranchId { get; set; }

        public int DepartmentId { get; set; }

        public int DivisionId { get; set; }

        public int PositionId { get; set; }

        public int PayrollGroupId { get; set; }

        public int AccountId { get; set; }

        public int PayrollTypeId { get; set; }
        public int ShiftCodeId { get; set; }

        public int FixNumberOfDays { get; set; }

        public int FixNumberOfHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal MonthlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal PayrollRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal DailyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal AbsentDailyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal HourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal NightHourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal OvertimeHourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal OvertimeNightHourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal TardyHourlyRate { get; set; }

        public int EntryUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime EntryDateTime { get; set; }

        public int UpdateUserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDateTime { get; set; }

        public bool IsLocked { get; set; }

        [StringLength(50)]
        public string? TaxTable { get; set; }

        [Column("HDMFAddOn", TypeName = "decimal(18, 5)")]
        public decimal? HdmfaddOn { get; set; }

        [Column("SSSAddOn", TypeName = "decimal(18, 5)")]
        public decimal? SssaddOn { get; set; }

        [Column("HDMFType")]
        [StringLength(50)]
        public string Hdmftype { get; set; } = null!;

        [Column("SSSIsGrossAmount")]
        public bool SssisGrossAmount { get; set; }

        public bool? IsMinimumWageEarner { get; set; }

        public string? PictureFilePath { get; set; }

        public string? OldPictureFilePath { get; set; }

        [StringLength(50)]
        public string? BloodType { get; set; }

        [StringLength(50)]
        public string? ShirtType { get; set; }

        [StringLength(50)]
        public string? DepBrnCode { get; set; }

        public bool? IsExemptedInMandatoryDeductions { get; set; }

        public bool? IsFlex { get; set; }

        public int? EmploymentType { get; set; }

        public decimal? Allowance { get; set; }

        public bool? IsLongShift { get; set; }

        public bool? IsSalaryConfidential { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewMonthlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewPayrollRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewDailyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewAbsentDailyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewHourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewNightHourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewOvertimeHourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewOvertimeNightHourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewTardyHourlyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? NewAllowance { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? LeaveBalance { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? LoanBalance { get; set; }

        public bool? IsFlexBreak { get; set; }

        //[JsonIgnore]
        //[IgnoreDataMember]
        public virtual List<MstEmployeeMemoDto> MstEmployeeMemos { get; set; } = new List<MstEmployeeMemoDto>();
        public virtual List<MstEmployeeShiftCodeDto> MstEmployeeShiftCodes { get; set; } = new List<MstEmployeeShiftCodeDto>();
        public virtual List<MstEmployeeSalaryHistoryDto> MstEmployeeSalaryHistories { get; } = new List<MstEmployeeSalaryHistoryDto>();
    }
}
