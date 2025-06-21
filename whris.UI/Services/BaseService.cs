using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using whris.Data;
using whris.Data.Data;

namespace whris.UI.Services
{
    public abstract class BaseService
    {
        public HRISCalendarContext GetContext()
        {
            return new HRISCalendarContext();
        }
    }
}
