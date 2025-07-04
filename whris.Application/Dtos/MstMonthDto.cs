using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace whris.Application.Dtos
{
    public class MstMonthDto
    {
        public int Id { get; set; }
        public string ? Month { get; set; } 
        public int Quarter { get; set; }
    }
}
