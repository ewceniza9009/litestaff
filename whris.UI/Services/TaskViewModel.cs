using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using whris.Data.Models;

namespace whris.UI.Services
{
    public class TaskViewModel : ISchedulerEvent
    {
        public int TaskID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        private DateTime start;
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.ToUniversalTime();
            }
        }

        public string StartTimezone { get; set; }

        private DateTime end;
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.ToUniversalTime();
            }
        }

        public string EndTimezone { get; set; }

        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public bool IsAllDay { get; set; }
        public int? OwnerID { get; set; }
        public CalendarTask ToEntity()
        {
            return new CalendarTask
            {
                TaskId = TaskID,
                Title = Title,
                Start = Start,
                StartTimezone = StartTimezone,
                End = End,
                EndTimezone = EndTimezone,
                Description = Description,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceId = RecurrenceID,
                IsAllDay = IsAllDay,
                OwnerId = OwnerID
            };
        }
    }
}
