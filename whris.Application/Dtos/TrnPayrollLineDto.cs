using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Application.Common;
using whris.Data.Models;

namespace whris.Application.Dtos
{
    public class TrnPayrollLineDto
    {
        [Key]
        public int Id { get; set; }

        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public int PayrollTypeId { get; set; }

        public int TaxCodeId { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularWorkingHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayWorkingHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayWorkingHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularRestdayHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayRestdayHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayRestdayHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularOvertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayOvertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayOvertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularNightHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayNightHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayNightHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularNightOvertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayNightOvertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayNighOvertimetHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalTardyLateHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalTardyUndertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularWorkingAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayWorkingAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayWorkingAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularRestdayAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayRestdayAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayRestdayAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularOvertimeAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayOvertimeAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayOvertimeAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularNightAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayNightAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayNightAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalRegularNightOvertimeAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalLegalHolidayNightOvertimeAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSpecialHolidayNightOvertimeAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalSalaryAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalTardyAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalAbsentAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalNetSalaryAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalOtherIncomeTaxable { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal GrossIncome { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalOtherIncomeNonTaxable { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal GrossIncomeWithNonTaxable { get; set; }

        [Column("SSSContribution", TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Ssscontribution { get; set; }

        [Column("SSSECContribution", TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Ssseccontribution { get; set; }

        [Column("PHICContribution", TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Phiccontribution { get; set; }

        [Column("HDMFContribution", TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Hdmfcontribution { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal Tax { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal TotalOtherDeduction { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal NetIncome { get; set; }

        [Column("SSSContributionEmployer", TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal SsscontributionEmployer { get; set; }

        [Column("SSSECContributionEmployer", TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal SsseccontributionEmployer { get; set; }

        [Column("PHICContributionEmployer", TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal PhiccontributionEmployer { get; set; }

        [Column("HDMFContributionEmployer", TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal HdmfcontributionEmployer { get; set; }

        public int? AccountId { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? DailyRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal? PayrollRate { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal OrigSSSContribution { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal SSSWISP { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        //public decimal? Basic => TotalSalaryAmount -
        //		   TotalLegalHolidayWorkingAmount -
        //		   TotalSpecialHolidayWorkingAmount -
        //		   TotalRegularRestdayAmount -
        //		   TotalLegalHolidayRestdayAmount -
        //		   TotalSpecialHolidayRestdayAmount -
        //		   TotalRegularOvertimeAmount -
        //		   TotalLegalHolidayOvertimeAmount -
        //		   TotalSpecialHolidayOvertimeAmount -
        //		   TotalRegularNightAmount -
        //		   TotalLegalHolidayNightAmount -
        //		   TotalSpecialHolidayNightAmount -
        //		   TotalRegularNightOvertimeAmount -
        //		   TotalLegalHolidayNightOvertimeAmount -
        //		   TotalSpecialHolidayNightOvertimeAmount;

        public decimal? Basic => Lookup.GetEmployeeBasic(EmployeeId);

		public string? EmployeeName => Lookup.GetEmployeeNameById(EmployeeId);
    }
}
