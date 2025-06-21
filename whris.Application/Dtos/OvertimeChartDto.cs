using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class OvertimeChartDto
    {
        public OvertimeChartDto(string Month,
            decimal noOfHours)
        {
            this.Month = Month;
            this.NoOfHours = noOfHours;
        }

        public string Month { get; set; }
        public decimal NoOfHours { get; set; }

    }
}
