using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System;
using whris.Application.Common;
using whris.Application.CQRS.MstEmployee.Commands;
using whris.Application.CQRS.MstEmployee.Queries;
using whris.Application.Dtos;
using whris.Data.Data;
using whris.UI.Authorization;
using whris.UI.Services;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.MstEmployee
{
    [Authorize]
    [Secure("MstEmployee")]
    //[IgnoreAntiforgeryToken]
    public class DetailModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private IMediator _mediator;
        private IMemoryCache _cache;

        public MstEmployeeDetailDto EmployeeDetail { get; set; } = new MstEmployeeDetailDto();
        public MstEmployeeComboboxDatasources ComboboxDatasources = MstEmployeeComboboxDatasources.Instance;

        public DetailModel(IWebHostEnvironment environment, IMediator mediator, IMemoryCache cache)
        {
            _environment = environment;
            _mediator= mediator;
            _cache= cache;
        }

        public async Task OnGetAsync(int Id)
        {
            if (User.Claims.Count() > 0)
            {
                var aspUserId = User.Claims.ToList()[0].Value;
                var email = User.Claims.ToList()[1].Value;
                var userId = Lookup.GetUserIdByAspUserId(aspUserId);

                var sysCurrent = new SysCurrentDto
                {
                    AspUserId = aspUserId,
                    Email = email,
                    CurrentUserId = userId,
                    AdminSwitch = false
                };

                //Application.Library.Security.AspUserId = aspUserId;
            }

            var employee = new GetMstEmployeeById()
            {
                Id = Id
            };

            EmployeeDetail = await _mediator.Send(employee);
        }

        public async Task OnGetAdd()
        {
            var addEmployee = new AddEmployee();

            EmployeeDetail = await _mediator.Send(addEmployee);
        }

        public async Task<IActionResult> OnPostSaveEmployeeNewSalary(MstEmployeeDetailDto employee)
        {
            var saveEmployeeNewSalary = new SaveEmployeeNewSalary()
            {
                Employee = employee
            };

            var resultId = await _mediator.Send(saveEmployeeNewSalary);

            _cache.Remove(Caching.EmployeeCmbDsCacheKey);
            _cache.Set(Caching.EmployeeCmbDsCacheKey, Common.GetEmployees().Value, Caching.cacheEntryOptions);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostSave(MstEmployeeDetailDto employee) 
        {
            if (employee.LastName == null)
            {
                employee.LastName = string.Empty;
            }

            if (employee.FirstName == null)
            {
                employee.FirstName = string.Empty;
            }

            if (employee.MiddleName == null)
            {
                employee.MiddleName = string.Empty;
            }

            if (employee.ExtensionName == null)
            {
                employee.ExtensionName = string.Empty;
            }

            var saveEmployee = new SaveEmployee()
            {
                Employee = employee
            };

            var resultId = await _mediator.Send(saveEmployee);

            _cache.Remove(Caching.EmployeeCmbDsCacheKey);
            _cache.Set(Caching.EmployeeCmbDsCacheKey, Common.GetEmployees().Value, Caching.cacheEntryOptions);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostDelete(int id) 
        {
            if (!Application.Library.Security.IsUserAdmin()) 
            {
                throw new Exception("User does not have rights to delete, \nPlease contact Administrator!");
            }

            var deleteEmployee = new DeleteEmployee() 
            {
                Id= id
            };

            var resultId = await _mediator.Send(deleteEmployee);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int departmentId, string action) 
        {
            var getEmployee = new GetMstEmployeeIdByTurnPage()
            {
                Id = id,
                DepartmentId = departmentId,
                Action = action
            };

            var employeeId = await _mediator.Send(getEmployee);

            return new JsonResult(new { Id = employeeId });
        }

        public async Task<IActionResult> OnGetShiftCodes()
        {
            var result = ComboboxDatasources.ShiftCodeCmbDs;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnPostApproveEmployeeShiftCodes(MstEmployeeDetailDto employee) 
        {
            var approveEmployeeShiftCodes = new ApproveEmployeeShiftCodes()
            {
                Id = employee.Id,
                EmployeeCodes = employee.MstEmployeeShiftCodes
            };

            await _mediator.Send(approveEmployeeShiftCodes);

            return new JsonResult("Ok");
        }

        public async Task<IActionResult> OnPostUploadImage(IFormFile files)
        {
            var fileName = "";

            string photosFolder = Path.Combine(_environment.WebRootPath, "photos");
            var employeeId = int.Parse(Request?.Form["Id"][0] ?? "0");
            var oldPictureFilePath = Lookup.GetOldPictureFilePathByEmployeeId(employeeId);

            if (files != null)
            {
                fileName = files.FileName;
            }
            else
            {
                fileName = oldPictureFilePath;
            }

            if (oldPictureFilePath != fileName && oldPictureFilePath != null)
            {
                System.IO.File.Delete(Path.Combine(photosFolder, oldPictureFilePath));
            }        


            if (files != null)
            {
                string itemImagePath = Path.Combine(photosFolder, files.FileName);

                using (var fileStream = new FileStream(itemImagePath, FileMode.Create))
                {
                    files.CopyTo(fileStream);
                }
                return new ObjectResult(new { status = "success" });
            }

            await Task.Run(() => { });

            return new JsonResult("Ok");
        }
    }
}
