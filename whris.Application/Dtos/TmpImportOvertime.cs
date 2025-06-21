using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class TmpImportOvertime
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public decimal OvertimeHours { get; set; }     
        public decimal OvertimeNightHours { get; set; }     
        public decimal OvertimeLimitHours { get; set; }     
        public string Remarks { get; set; } = "NA";
        public string EmployeeName { get; set; } = "NA";
        public string BiometricId { get; set; } = "NA";
        public string BranchName { get; set; } = "NA";
    }
}
