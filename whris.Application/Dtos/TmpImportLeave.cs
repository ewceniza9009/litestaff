using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class TmpImportLeave
    {
        public int EmployeeId { get; set; }
        public int LeaveId { get; set; }
        public DateTime Date { get; set; }
        public decimal NumberOfHours { get; set; }
        public bool WithPay { get; set; }
        public bool DebitToLedger { get; set; }
        public string Remarks { get; set; } = "NA";
    }
}
