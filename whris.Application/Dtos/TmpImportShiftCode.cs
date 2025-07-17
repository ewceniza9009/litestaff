using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class TmpImportShiftCode
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        //public int ShiftCodeId { get; set; }
        public string ShiftCode { get; set; } = "NA";
        public string Remarks { get; set; } = "NA";
        public string EmployeeName { get; set; } = "NA";
        public string BiometricId { get; set; } = "NA";
    }
}
