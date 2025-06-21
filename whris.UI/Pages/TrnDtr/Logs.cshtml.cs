using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using whris.Application.CQRS.TrnDtr.Commands;
using whris.Application.CQRS.TrnLogs.Queries;
using whris.Application.CQRS.TrnTrnLogById.Commands;
using whris.UI.Services.Datasources;
using whris.Application.Queries.TrnDtr;
using System.Drawing;

namespace whris.UI.Pages.TrnDtr
{
    public class LogsModel : PageModel
    {

        private IMediator _mediator;

        public TrnDtrComboboxDatasources ComboboxDatasources = TrnDtrComboboxDatasources.Instance;

        public LogsModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void OnGet()
        {
            ComboboxDatasources.EmployeeCmbDs = ComboboxDatasources.EmployeeCmbDs
               .ToList();
        }

        public async Task<IActionResult> OnPostReadLogs([DataSourceRequest] DataSourceRequest request, DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var logs = new GetTrnLogs()
            {
                Request = request,
                StartDate = startDate ?? DateTime.Now,
                EndDate = endDate ?? DateTime.Now,
                EmployeeId = employeeId
            };

            return new JsonResult(await _mediator.Send(logs));
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteLog = new DeleteTrnLogById()
            {
                Id = id
            };

            await _mediator.Send(deleteLog);

            return new JsonResult(await Task.Run(() => id));
        }

        public IActionResult OnPostExportLogsToExcelFull(int employeeId, DateTime dateStart, DateTime dateEnd)
        {
            var logs = new Logs()
            {
                StartDate =dateStart,
                EndDate = dateEnd,
                EmployeeId = employeeId
            };

            var logsResult = logs.Result();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Logs");

                // Add headers
                worksheet.Cells[1, 1].Value = "Biometric Id Number";
                worksheet.Cells[1, 2].Value = "Employee";
                worksheet.Cells[1, 3].Value = "Log Date Time";
                worksheet.Cells[1, 4].Value = "Log Type";
                worksheet.Cells[1, 5].Value = "Department";

                // Make headers bold
                using (var range = worksheet.Cells[1, 1, 1, 22])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                // Add data
                for (int i = 0; i < logsResult.Count; i++)
                {
                    var logResult = logsResult[i];
                    worksheet.Cells[i + 2, 1].Value = logResult.BiometricIdNumber;
                    worksheet.Cells[i + 2, 2].Value = logResult.EmployeeName;
                    worksheet.Cells[i + 2, 3].Value = logResult.LogDateTime;
                    worksheet.Cells[i + 2, 4].Value = logResult.LogTypeString;
                    worksheet.Cells[i + 2, 5].Value = logResult.Department;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                worksheet.Column(3).Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Logs-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}
