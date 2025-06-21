using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class TmpImportOtherDeduction
    {
        public int EmployeeId { get; set; }
        public int OtherDeductionId { get; set; }    
        public int EmployeeLoanId { get; set; }    
        public decimal Amount { get; set; }
    }
}
