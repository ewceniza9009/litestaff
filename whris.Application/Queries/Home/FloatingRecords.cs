using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.Queries.Home
{
    public class FloatingRecords
    {
        public FloatingRecord Record 
        {
            get 
            {
                var record = new FloatingRecord();

                using (var ctx = new HRISContext()) 
                {
                    record.Leave = ctx.TrnLeaveApplications.Count(x => !x.IsLocked);
                    record.Overtime = ctx.TrnOverTimes.Count(x => !x.IsLocked);
                    record.Loans = ctx.MstEmployeeLoans.Count(x => !x.IsLocked);
                    record.OtherIncome = ctx.TrnPayrollOtherIncomes.Count(x => !x.IsLocked);
                    record.OtherDeduction = ctx.TrnPayrollOtherDeductions.Count(x => !x.IsLocked);
                }

                return record;
            }
        }

        public class FloatingRecord 
        {
            public int Leave { get; set; }
            public int Overtime { get; set; }
            public int Loans { get; set; }
            public int OtherIncome { get; set; }
            public int OtherDeduction { get; set; }
        }
    }
}
