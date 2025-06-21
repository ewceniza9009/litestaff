using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Models;

namespace whris.Application.Dtos
{
    public partial class MstEmployeeSalaryHistoryDto
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ChangeDate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal MonthlyRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal PayrollRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal DailyRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal AbsentDailyRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal NightHourlyRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal OvertimeHourlyRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal OvertimeNightHourlyRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal TardyHourlyRate { get; set; }
        [Column(TypeName = "decimal(18, 5)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal Allowance { get; set; }

        public MstEmployee MstEmployee { get; set; }
    }
}
