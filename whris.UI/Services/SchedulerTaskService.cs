namespace whris.UI.Services
{
    using System.Linq;
    using Kendo.Mvc.UI;
    using System;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using whris.Data.Data;

    public class SchedulerTaskService : ISchedulerEventService<TaskViewModel>
    {
        private HRISCalendarContext db;

        public SchedulerTaskService(HRISCalendarContext context)
        {
            db = context;
        }
        public virtual IEnumerable<TaskViewModel> GetRange(DateTime start, DateTime end)
        {
            var result = GetAll().ToList().Where(t => (t.RecurrenceRule != null || (t.Start >= start && t.Start <= end) || (t.End >= start && t.End <= end)));

            return result;
        }
        public virtual IQueryable<TaskViewModel> GetAll()
        {
            IQueryable<TaskViewModel> result = db.CalendarTasks.Select(task => new TaskViewModel
            {
                TaskID = task.TaskId,
                Title = task.Title,
                Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),
                StartTimezone = task.StartTimezone,
                EndTimezone = task.EndTimezone,
                Description = task.Description,
                IsAllDay = task.IsAllDay,
                RecurrenceRule = task.RecurrenceRule,
                RecurrenceException = task.RecurrenceException,
                RecurrenceID = task.RecurrenceId,
                OwnerID = task.OwnerId
            });

            return result;
        }

        public virtual void Insert(TaskViewModel task, ModelStateDictionary modelState)
        {
            if (ValidateModel(task, modelState))
            {

                if (string.IsNullOrEmpty(task.Title))
                {
                    task.Title = "";
                }

                var entity = task.ToEntity();

                db.CalendarTasks.Add(entity);
                db.SaveChanges();

                task.TaskID = entity.TaskId;
            }
        }

        public virtual void Update(TaskViewModel task, ModelStateDictionary modelState)
        {
            if (ValidateModel(task, modelState))
            {

                if (string.IsNullOrEmpty(task.Title))
                {
                    task.Title = "";
                }

                var entity = task.ToEntity();
                db.CalendarTasks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public virtual void Delete(TaskViewModel task, ModelStateDictionary modelState)
        {

            var entity = task.ToEntity();
            db.CalendarTasks.Attach(entity);

            var recurrenceExceptions = db.CalendarTasks.Where(t => t.RecurrenceId == task.TaskID);

            foreach (var recurrenceException in recurrenceExceptions)
            {
                db.CalendarTasks.Remove(recurrenceException);
            }

            db.CalendarTasks.Remove(entity);
            db.SaveChanges();
        }

        private bool ValidateModel(TaskViewModel appointment, ModelStateDictionary modelState)
        {
            if (appointment.Start > appointment.End)
            {
                modelState.AddModelError("errors", "End date must be greater or equal to Start date.");
                return false;
            }

            return true;
        }

        public TaskViewModel One(Func<TaskViewModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
