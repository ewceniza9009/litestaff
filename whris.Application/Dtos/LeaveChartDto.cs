using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class LeaveChartDto
    {
        public LeaveChartDto(string Month,
            decimal WithPay,
            decimal NoPay,
            decimal noOfHours)
        {
            this.Month = Month;
            this.WithPay = WithPay;
            this.NoPay = NoPay;
            this.NoOfHours = noOfHours;
        }

        public string Month { get; set; }
        public decimal WithPay { get; set;}
        public decimal NoPay { get; set;}
        public decimal NoOfHours { get; set; }

    }
}
