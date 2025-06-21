using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class LoanChartDto
    {
        public LoanChartDto(string Month,
            decimal CashAdvances,
            decimal SSSLoans,
            decimal amount)
        {
            this.Month = Month;
            this.CashAdvances = CashAdvances;
            this.SSSLoans = SSSLoans;
            this.Amount = amount;
        }

        public string Month { get; set; }
        public decimal CashAdvances { get; set;}
        public decimal SSSLoans { get; set;}
        public decimal Amount { get; set; }

    }
}
