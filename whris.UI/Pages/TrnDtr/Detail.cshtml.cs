using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using whris.Application.Common;
using whris.Application.CQRS.TrnDtr.Commands;
using whris.Application.CQRS.TrnDtr.Queries;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Application.Queries.TrnDtr;
using whris.UI.Authorization;
using whris.UI.Services;
using whris.UI.Services.Datasources;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace whris.UI.Pages.TrnDtr
{
    [Authorize]
    [Secure("TrnDTR")]
    public class DetailModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private IMediator _mediator;
        private IMemoryCache _cache;

        public TrnDtrDetailDto DtrDetail { get; set; } = new TrnDtrDetailDto();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;
        public bool IsAdmin = false;
        public bool CanEditDtrTime = false;

        public DetailModel(IWebHostEnvironment environment, IMediator mediator, IMemoryCache cache)
        {
            _environment = environment;
            _mediator = mediator;
            _cache = cache;
        }

        public async Task OnGetAsync(int Id)
        {
            var dtr = new GetTrnDtrById()
            {
                Id = Id
            };

            var aspUserId = string.Empty;

            if (User.Claims.Count() > 0)
            {
                aspUserId = User.Claims.ToList()[0].Value;
            }

            //Security.AspUserId = aspUserId;

            DtrDetail = await _mediator.Send(dtr);
            IsAdmin = Security.IsUserAdmin();
            CanEditDtrTime = Security.CanEditDtrTime();

            ComboboxDataSources.OvertimeCmbDs = ComboboxDataSources.OvertimeCmbDs
                .Where(x => x.PayrollGroupId == DtrDetail.PayrollGroupId)
                .ToList();

            ComboboxDataSources.LeaveCmbDs = ComboboxDataSources.LeaveCmbDs
                .Where(x => x.PayrollGroupId == DtrDetail.PayrollGroupId)
                .ToList();

            Trace();
        }

        public async Task OnPostAdd(int payrollGroupId)
        {
            var aspUserId = string.Empty;

            if (User.Claims.Count() > 0)
            {
                aspUserId = User.Claims.ToList()[0].Value;
            }

            var addDtr = new AddDtr() 
            {
                AspUserId = aspUserId,
                PayrollGroupId = payrollGroupId
            };

            DtrDetail = await _mediator.Send(addDtr);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteDtr = new DeleteDtr()
            {
                Id = id
            };

            await _mediator.Send(deleteDtr);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnDtrDetailDto dtr)
        {
            var editedLines = dtr.TrnDtrlines.Where(x => x.IsEdited);

            //foreach (var item in editedLines) 
            //{
            //    var date = DateOnly.FromDateTime(item.Date);
            //    var timeIn1 = new TimeOnly();

            //    if (item.TimeIn1 is not null)
            //    {
            //        timeIn1 = TimeOnly.FromDateTime(item.TimeIn1 ?? new DateTime());
            //        item.TimeIn1 = DateTime.Parse($"{date} {timeIn1}");
            //    }

            //    if (item.TimeOut1 is not null)
            //    {
            //        var timeOut1 = TimeOnly.FromDateTime(item.TimeOut1 ?? new DateTime());
            //        item.TimeOut1 = DateTime.Parse($"{date} {timeOut1}");

            //        if (timeOut1 < timeIn1)
            //        {
            //            item.TimeOut1 = DateTime.Parse($"{date.AddDays(1)} {timeOut1}");
            //        }
            //    }

            //    if (item.TimeIn2 is not null)
            //    {
            //        var timeIn2 = TimeOnly.FromDateTime(item.TimeIn2 ?? new DateTime());
            //        item.TimeIn2 = DateTime.Parse($"{date} {timeIn2}");

            //        if (timeIn2 < timeIn1)
            //        {
            //            item.TimeIn2 = DateTime.Parse($"{date.AddDays(1)} {timeIn2}");
            //        }
            //    }

            //    if (item.TimeOut2 is not null)
            //    {
            //        var timeOut2 = TimeOnly.FromDateTime(item.TimeOut2 ?? new DateTime());
            //        item.TimeOut2 = DateTime.Parse($"{date} {timeOut2}");

            //        if (timeOut2 < timeIn1)
            //        {
            //            item.TimeOut2 = DateTime.Parse($"{date.AddDays(1)} {timeOut2}");
            //        }
            //    }
            //}

            var saveDtr = new SaveDtr()
            {
                Dtr = dtr
            };

            var resultId = await _mediator.Send(saveDtr);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostAddDtrLine(int dtrId) 
        {
            var addDtr = new AddDtrLine()
            {
                DtrId = dtrId
            };

            return new JsonResult(await _mediator.Send(addDtr));
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getDtr = new GetTrnDtrIdByTurnPage()
            {
                Id = id,
                PayrollGroupId = payrollGroupId,
                Action = action
            };

            var dtrId = await _mediator.Send(getDtr);

            return new JsonResult(new { Id = dtrId });
        }

        public async Task<IActionResult> OnPostQuickEdit(int dtrId, 
            int? departmentId, 
            int? employeeId,
            DateTime dateStart, 
            DateTime dateEnd, 
            DateTime? timeIn1, 
            DateTime? timeOut1, 
            DateTime? timeIn2,
            DateTime? timeOut2)
        {
            var editDTRLines = new EditDtrLinesByQuickEdit()
            {
                DTRId = dtrId,
                DepartmentId = departmentId,
                EmployeeId = employeeId,
                DateStart = dateStart,
                DateEnd = dateEnd,
                TimeIn1 = timeIn1,
                TimeOut1 = timeOut1,
                TimeIn2 = timeIn2,  
                TimeOut2 = timeOut2
            };

            var statusCode = await _mediator.Send(editDTRLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostQuickChangeShift(int dtrId)
        {
            var editDTRLines = new EditDtrLinesByQuickChange()
            {
                DTRId = dtrId
            };

            var statusCode = await _mediator.Send(editDTRLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostDtrProcess()
        {
            IFormFile? file;
            int? dtrId, payrollGroupId, changeShiftId, departmentId, employeeId;
            DateTime startDate, endDate;

            AssignValuesFromForm(out file, out dtrId, out payrollGroupId, out changeShiftId, out departmentId, out employeeId, out startDate, out endDate);

            var tmplLogs = new List<TmpDtrLogs>();

            if (file is not null && file.Length > 0)
            {
                var fileName = file.FileName;
                var filePath = Path.Combine(_environment.WebRootPath, "Uploads", fileName);
                var extension = Path.GetExtension(filePath)?.ToLower();

                var isConverTextToDat = true;

                if (isConverTextToDat && extension == ".txt")
                {
                    fileName = file.FileName.Replace(".txt", ".dat");
                    filePath = Path.Combine(_environment.WebRootPath, "Uploads", fileName);
                    extension = ".dat";
                }                

                string[] validFileTypes = { ".xls", ".xlsx", ".csv", ".txt" , ".dat"};

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (validFileTypes.Contains(extension))
                {
                    tmplLogs = FileUtil.ProcessLogs(departmentId, employeeId, startDate, endDate, filePath, extension);
                }
                else
                {
                    return new BadRequestObjectResult("File format is incorrect");
                }
            }
            else
            {
                tmplLogs = DTR.ProcessLogsFromDb(departmentId, employeeId, startDate, endDate);

                if ((tmplLogs?.Count() ?? 0) == 0) 
                {
                    return new BadRequestObjectResult("No records found.");
                }
            }

            var dtrLines = new AddDtrLinesByProcessDtr()
            {
                DTRId = dtrId ?? 0,
                PayrollGroupId = payrollGroupId ?? 0,
                DepartmentId = departmentId,
                EmployeeId = employeeId,
                DateStart = startDate,
                DateEnd = endDate,
                ChangeShiftId = changeShiftId,
                TmpDtrLogs = tmplLogs
            };

            var generatedDtrLines = await _mediator.Send(dtrLines);

            DtrDetail.TrnDtrlines = generatedDtrLines;

            var saveDtr = new SaveDtrByProcess()
            {
                DtrDetail = DtrDetail
            };

            _ = await _mediator.Send(saveDtr);

            return new JsonResult("Ok");
        }

        public async Task<IActionResult> OnPostDtrCompute(int dtrId, int? empId, DateTime startDate, DateTime endDate)
        {
            var editDTRLines = new EditDtrLinesByComputeDtr()
            {
                DTRId = dtrId,
                EmployeeId = empId,
                DateStart = startDate,
                DateEnd = endDate                
            };

            var statusCode = await _mediator.Send(editDTRLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostUpdateMissingPunches(List<MissingPunches.Record> records)
        {
            var updateDTRMissingPunchesLines = new UpdateDtrMissingPunches()
            {
                MissingPunches = records
            };

            var statusCode = await _mediator.Send(updateDTRMissingPunchesLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnGetEmployees(int payrollGroupId)
        {
            //var result = Common.GetEmployees(payrollGroupId).Value;
            var result = _cache.Get(Caching.EmployeeCmbDsCacheKey);

            if (result is null) 
            {
                //_cache.Set(Caching.EmployeeCmbDsCacheKey, Common.GetEmployees(payrollGroupId).Value, Caching.cacheEntryOptions);
                _cache.Set(Caching.EmployeeCmbDsCacheKey, Common.GetEmployees().Value, Caching.cacheEntryOptions);
                result = _cache.Get(Caching.EmployeeCmbDsCacheKey);
            }

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetShiftCodes()
        {
            var result = Common.GetShiftCodes().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetDayTypes()
        {
            var result = Common.GetDayTypes().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public async Task<IActionResult> OnGetPayrollGroupChange(int payrollGroupId) 
        {
            //_cache.Set(Caching.EmployeeCmbDsCacheKey, Common.GetEmployees(payrollGroupId).Value, Caching.cacheEntryOptions);
            //_cache.Remove(Caching.EmployeeCmbDsCacheKey);

            return new JsonResult(await Task.Run(() => 0));
        } 

        private void AssignValuesFromForm(out IFormFile? file, out int? dtrId, out int? payrollGroupId, out int? changeShiftId, out int? departmentId, out int? employeeId, out DateTime startDate, out DateTime endDate)
        {
            file = null;

            if (Request.Form.Files.Count > 0) 
            {
                file = Request.Form.Files[0];
            }            

            var dtrIdStr = Request?.Form["DTRId"][0]?.ToString();
            var periodIdStr = Request?.Form["PeriodId"][0]?.ToString();
            var dtrNumberStr = Request?.Form["Dtrnumber"][0]?.ToString();
            var dtrDateStr = Request?.Form["Dtrdate"][0]?.ToString();
            var payrollGroupIdStr = Request?.Form["PayrollGroupId"][0]?.ToString();
            var remarksStr = Request?.Form["Remarks"][0]?.ToString();

            var overtimeIdStr = Request?.Form["OvertTimeId"][0]?.ToString();
            var leaveApplicationIdStr = Request?.Form["LeaveApplicationId"][0]?.ToString();
            var changeShiftIdStr = Request?.Form["ChangeShiftId"][0]?.ToString();
            var preparedByStr = Request?.Form["PreparedBy"][0]?.ToString();
            var checkedByStr = Request?.Form["CheckedBy"][0]?.ToString();
            var approvedByStr = Request?.Form["ApprovedBy"][0]?.ToString();

            var departmentIdStr = Request?.Form["DepartmentId"][0]?.ToString();
            var employeeIdStr = Request?.Form["EmployeeId"][0]?.ToString();
            var startDateStr = Request?.Form["StartDate"][0]?.ToString();
            var endDateStr = Request?.Form["EndDate"][0]?.ToString();

            dtrId = !string.IsNullOrEmpty(dtrIdStr) ? int.Parse(dtrIdStr) : null;
            int? periodId = !string.IsNullOrEmpty(periodIdStr) ? int.Parse(periodIdStr) : null;
            string? dtrNumber = !string.IsNullOrEmpty(dtrNumberStr) ? dtrNumberStr : null;
            var dtrDate = !string.IsNullOrEmpty(dtrDateStr) ? DateTime.Parse(dtrDateStr) : DateTime.Now.Date;
            payrollGroupId = !string.IsNullOrEmpty(payrollGroupIdStr) ? int.Parse(payrollGroupIdStr) : null;
            string? remarks = !string.IsNullOrEmpty(remarksStr) ? remarksStr : null;

            int? overtimeId = !string.IsNullOrEmpty(overtimeIdStr) ? int.Parse(overtimeIdStr) : null;
            int? leaveApplicationId = !string.IsNullOrEmpty(leaveApplicationIdStr) ? int.Parse(leaveApplicationIdStr) : null;
            changeShiftId = !string.IsNullOrEmpty(changeShiftIdStr) ? int.Parse(changeShiftIdStr) : null;
            int? preparedBy = !string.IsNullOrEmpty(preparedByStr) ? int.Parse(preparedByStr) : null;
            int? checkedBy = !string.IsNullOrEmpty(checkedByStr) ? int.Parse(checkedByStr) : null;
            int? approvedBy = !string.IsNullOrEmpty(approvedByStr) ? int.Parse(approvedByStr) : null;

            departmentId = !string.IsNullOrEmpty(departmentIdStr) ? int.Parse(departmentIdStr) : null;
            employeeId = !string.IsNullOrEmpty(employeeIdStr) ? int.Parse(employeeIdStr) : null;
            startDate = !string.IsNullOrEmpty(startDateStr) ? DateTime.Parse(startDateStr) : DateTime.Now.Date;
            endDate = !string.IsNullOrEmpty(endDateStr) ? DateTime.Parse(endDateStr) : DateTime.Now.Date;

            DtrDetail.Id = dtrId ?? 0;
            DtrDetail.PeriodId = periodId ?? 0;
            DtrDetail.Dtrnumber = dtrNumber ?? "";
            DtrDetail.Dtrdate = dtrDate;
            DtrDetail.PayrollGroupId = payrollGroupId ?? 0;
            DtrDetail.DateStart = startDate;
            DtrDetail.DateEnd = endDate;
            DtrDetail.Remarks = remarks ?? "";

            DtrDetail.OvertTimeId = overtimeId;
            DtrDetail.LeaveApplicationId = leaveApplicationId;
            DtrDetail.ChangeShiftId = changeShiftId;
            DtrDetail.PreparedBy = preparedBy;
            DtrDetail.CheckedBy = checkedBy;
            DtrDetail.ApprovedBy = approvedBy;
        }

        public void Trace([CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            Debug.WriteLine($"Info => Caller Member: {memberName}, File Path: {filePath}, Line Number: {lineNumber}");
        }
    }
}
