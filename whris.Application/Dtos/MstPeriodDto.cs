﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whris.Application.Dtos
{
    public class MstPeriodDto
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Period { get; set; } = null!;
    }
}
