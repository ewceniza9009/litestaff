using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Globalization;
using System.Text.Json;
using whris.Application.Common;
using whris.Application.CQRS.TrnOTApplication.Commands;
using whris.Application.CQRS.TrnOTApplication.Queries;
using whris.Application.Dtos;
using whris.Data.Models;
using whris.UI.Authorization;
using whris.UI.Services;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.TrnOTApplication
{
    [Authorize]
    [Secure("TrnOverTime")]
    public class DetailModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private IMediator _mediator;

        public TrnOTApplicationDetailDto OTApplicationDetail { get; set; } = new TrnOTApplicationDetailDto();
        public TrnDtrComboboxDatasources ComboboxDataSources = TrnDtrComboboxDatasources.Instance;

        public DetailModel(IWebHostEnvironment environment, IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var otApp = new GetTrnOTApplicationById()
            {
                Id = Id
            };

            OTApplicationDetail = await _mediator.Send(otApp);
        }

        public async Task OnPostAdd(int payrollGroupId)
        {
            var aspUserId = string.Empty;

            if (User.Claims.Count() > 0)
            {
                aspUserId = User.Claims.ToList()[0].Value;
            }

            var addOT = new AddOTApplication()
            {
                AspUserId = aspUserId,
                PayrollGroupId = payrollGroupId
            };

            OTApplicationDetail = await _mediator.Send(addOT);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteOT = new DeleteOTApplication()
            {
                Id = id
            };

            await _mediator.Send(deleteOT);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(TrnOTApplicationDetailDto otApp)
        {
            var saveOT = new SaveOTApplication()
            {
                OTApplication = otApp
            };

            var resultId = await _mediator.Send(saveOT);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostAddOTApplicationLine(int LAId)
        {
            var addOtLine = new AddOTApplicationLine()
            {
                OTApplicationId = LAId
            };

            return new JsonResult(await _mediator.Send(addOtLine));
        }

        public async Task<IActionResult> OnPostTurnPage(int id, int payrollGroupId, string action)
        {
            var getOT = new GetTrnOTApplicationIdByTurnPage()
            {
                Id = id,
                PayrollGroupId = payrollGroupId,
                Action = action
            };

            var otaId = await _mediator.Send(getOT);

            return new JsonResult(new { Id = otaId });
        }

        public async Task<IActionResult> OnPostQuickEncode(int otId,
            int payrollGroupId,
            DateTime dateStart,
            DateTime dateEnd,
            int? employeeId,
            decimal overTimeHours,
            decimal overTimeLimitHours)
        {
            var addOtLines = new AddOTApplicationsByQuickEncode()
            {
                OTId = otId,
                PayrollGroupId = payrollGroupId,
                DateStart = dateStart,
                DateEnd = dateEnd,
                EmployeeId = employeeId,
                OverTimeHours = overTimeHours,
                OvertimeLimitHours = overTimeLimitHours
            };

            var statusCode = await _mediator.Send(addOtLines);

            return new JsonResult(new { Id = statusCode });
        }

        public async Task<IActionResult> OnPostImportOvertime() 
        {
            IFormFile? file = Request.Form.Files[0]; 
            var strId = Request?.Form["Id"][0]?.ToString();

            var tmpOvertimeImports = new List<TmpImportOvertime>();

            if (file is not null && file.Length > 0)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "Uploads", file.FileName);
                var extension = Path.GetExtension(filePath)?.ToLower();
                string[] validFileTypes = { ".xls", ".xlsx", ".csv", ".txt" };

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (validFileTypes.Contains(extension))
                {
                    tmpOvertimeImports = FileUtil.ProcessOvertimeImports(filePath, extension);
                }
                else
                {
                    return new BadRequestObjectResult("File format is incorrect");
                }
            }
            else
            {
                return new BadRequestObjectResult("No file was uploaded.");
            }

            var imports = new ImportLeave()
            {
                Id = int.Parse(strId ?? "0"),
                TmpOvertimeImports = tmpOvertimeImports
            };

            _ = await _mediator.Send(imports);

            return new JsonResult("Ok");
        }

        public async Task<IActionResult> OnGetEmployees(int payrollGroupId)
        {
            var result = Common.GetEmployees().Value;

            return new JsonResult(await Task.Run(() => result));
        }

        public IActionResult OnPostExportToExcel([FromBody] dynamic otApp)
        {
            var otDate = new DateTime();

            if (otApp is JsonElement jsonElement)
            {
                var stringDate = jsonElement.GetProperty("Otdate").GetString();

                otDate = DateTime.Parse(stringDate ?? "1/1/2024");
            }

            var data = new List<TmpImportOvertime>();
            var employees = Common.GetEmployees().Value as List<MstEmployeeDto>;

            if (employees is not null) 
            {
                foreach (var item in employees)
                {
                    data.Add(new TmpImportOvertime() 
                    { 
                        EmployeeName = Lookup.GetEmployeeNameById(item.Id),
                        BiometricId = Lookup.GetBioIdByEmployeeId(item.Id),
                        Date = otDate,
                        OvertimeHours = 0,
                        OvertimeNightHours = 0,
                        OvertimeLimitHours = 0,
                        BranchName = Lookup.GetBranchNameByEmployeeId(item.Id) ?? "None",
                        Remarks = ""
                    });
                }

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("OvertimeApplications");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Employee Name";
                    worksheet.Cells[1, 2].Value = "BiometricId";
                    worksheet.Cells[1, 3].Value = "Date";
                    worksheet.Cells[1, 4].Value = "OT";
                    worksheet.Cells[1, 5].Value = "NightOT";
                    worksheet.Cells[1, 6].Value = "LimitOT";
                    worksheet.Cells[1, 7].Value = "Branch";
                    worksheet.Cells[1, 8].Value = "Remarks";

                    for (int i = 0; i < data.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = data[i].EmployeeName;
                        worksheet.Cells[i + 2, 2].Value = data[i].BiometricId;
                        worksheet.Cells[i + 2, 3].Value = data[i].Date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[i + 2, 4].Value = data[i].OvertimeHours;
                        worksheet.Cells[i + 2, 5].Value = data[i].OvertimeNightHours;
                        worksheet.Cells[i + 2, 6].Value = data[i].OvertimeLimitHours;
                        worksheet.Cells[i + 2, 7].Value = data[i].BranchName;
                        worksheet.Cells[i + 2, 8].Value = data[i].Remarks;
                    }

                    worksheet.Cells[2, 7, employees.Count + 1, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[2, 8, employees.Count + 1, 8].Style.Numberformat.Format = "#,##0.00";

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;
                    string excelName = $"OvertimeApplications-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }

            return null;
        }
    }
}
