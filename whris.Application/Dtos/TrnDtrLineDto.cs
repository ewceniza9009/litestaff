using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Application.Common;

namespace whris.Application.Dtos
{
    public class TrnDtrLineDto
    {
        [Key]
        public int Id { get; set; }

        [Column("DTRId")]
        public int Dtrid { get; set; }

        public int EmployeeId { get; set; }

        public int ShiftCodeId { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        public DateTime? TimeIn1 { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        public DateTime? TimeOut1 { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        public DateTime? TimeIn2 { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime)]
        public DateTime? TimeOut2 { get; set; }

        public bool OfficialBusiness { get; set; }

        public bool OnLeave { get; set; }

        public bool Absent { get; set; }

        public bool HalfdayAbsent { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal RegularHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal NightHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal OvertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal OvertimeNightHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal GrossTotalHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal TardyLateHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal TardyUndertimeHours { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal NetTotalHours { get; set; }

        public int DayTypeId { get; set; }

        public bool RestDay { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal DayMultiplier { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal RatePerHour { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal RatePerNightHour { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal RatePerOvertimeHour { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal RatePerOvertimeNightHour { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal RegularAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal NightAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal OvertimeAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal OvertimeNightAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal RatePerHourTardy { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal RatePerAbsentDay { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N5}")]
        [DataType(DataType.Currency)]
        public decimal TardyAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal AbsentAmount { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal NetAmount { get; set; }

        [Column("DTRRemarksID")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public int? DtrremarksId { get; set; }

        [Column("DTRRemarks")]
        [StringLength(50)]
        public string? Dtrremarks { get; set; }

        [Column("ShiftDates")]
        [StringLength(255)]
        public string? ShiftDates { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsEdited { get; set; } = false;

        public string? EmployeeName => Lookup.GetEmployeeNameById(EmployeeId);
        public string? ShiftCodeName => Lookup.GetEmployeeShiftById(ShiftCodeId);

        public bool IsShiftCodeIsTommorow = false;

        public bool IsDateMoved = false;

        public bool IsSplitted = false;

        public int OldShiftCodeId = 0;

        public DateTime? OldTimeIn1 { get; set; }

        public DateTime? OldTimeOut1 { get; set; }

        public DateTime? OldTimeIn2 { get; set; }

        public DateTime? OldTimeOut2 { get; set; }

        public string? OldShiftDates { get; set; }

        public bool NoTimeIn1 = false;
        public bool NoTimeIn2 = false;

        public bool NoTimeOut2 = false;

        public bool Is2SwipesOnly = false;
    }
}
