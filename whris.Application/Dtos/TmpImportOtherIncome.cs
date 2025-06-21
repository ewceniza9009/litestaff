using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class TmpImportOtherIncome
    {
        public int EmployeeId { get; set; }
        public int OtherIncomeId { get; set; }
        public decimal Amount { get; set; }
    }
}
