using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using whris.Application.Common;
using whris.UI.Authorization;

namespace whris.UI.Controllers
{
    [Authorize]
    [Secure2]
    public class AutoCompleteController : Controller
    {
        public IActionResult GetEmployees(int? payrollGroupId)
        {
            var result = Common.GetEmployees(payrollGroupId);
            
            if (result.Value is List<whris.Application.Dtos.MstEmployeeDto> employees)
            {
                var data = employees.Select(x => new
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Text = x.FullName
                }).ToList();

                return Json(data);
            }

            return Json(new List<object>());
        }
    }
}
