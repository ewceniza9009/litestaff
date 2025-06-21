using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class PieChartDto
    {
        public PieChartDto()
        {
        }

        public PieChartDto(string source, decimal percentage)
        {
            Source = source;
            Percentage = percentage;
        }

        public string Source { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; }
        public bool Explode { get; set; }
    }
}
