using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Permissions;
using whris.Data.Data;
using whris.UI.Authorization;
using whris.UI.Services;

namespace whris.UI.Controllers
{
    [Authorize]
    [Secure2]
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private ISchedulerEventService<TaskViewModel> taskService => new SchedulerTaskService(new HRISCalendarContext());
        
        public virtual JsonResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(taskService.GetAll().ToDataSourceResult(request));
        }

        public virtual JsonResult Destroy([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            taskService.Delete(task, ModelState);

            ModelState.Clear();

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Create([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            taskService.Insert(task, ModelState);

            ModelState.Clear();

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Update([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            taskService.Update(task, ModelState);

            ModelState.Clear();

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }
    }
}
