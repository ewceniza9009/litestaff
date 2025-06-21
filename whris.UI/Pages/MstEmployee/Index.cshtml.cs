using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using whris.Application.Common;
using whris.Application.CQRS.MstEmployee.Queries;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstEmployee
{
    [Authorize]
    [Secure("MstEmployee")]
    public class IndexModel : PageModel
    {
        private IMediator _mediator;

        public List<MstDepartmentDto> allDepartments = new List<MstDepartmentDto>();

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            allDepartments = (List<MstDepartmentDto>)(Common.GetDepartmentList()?.Value ?? new List<MstDepartmentDto>());

            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnGetEmployeeDetail(int id)
        {
            allDepartments = (List<MstDepartmentDto>)(Common.GetDepartmentList()?.Value ?? new List<MstDepartmentDto>());

            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostReadEmployeeList([DataSourceRequest] DataSourceRequest request, int? departmentId, string search)
        {
            var allEmployees = new GetMstEmployeesByDepartmentAndSearch()
            {
                Request = request,
                DepartmentId = departmentId,
                Search = search
            };

            return new JsonResult(await _mediator.Send(allEmployees));
        }

        public IActionResult OnPostGenerateEmployeeExcel()
        {
            var employees = Common.GetEmployeesWithSalary();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                // Add headers
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Biometric Id Number";
                worksheet.Cells[1, 3].Value = "Name";
                worksheet.Cells[1, 4].Value = "Cellphone Number";
                worksheet.Cells[1, 5].Value = "Email Address";
                worksheet.Cells[1, 6].Value = "Department Id";
                worksheet.Cells[1, 7].Value = "Department Name";
                worksheet.Cells[1, 8].Value = "Locked";
                worksheet.Cells[1, 9].Value = "GSIS Number";
                worksheet.Cells[1, 10].Value = "SSS Number";
                worksheet.Cells[1, 11].Value = "HDMF Number";
                worksheet.Cells[1, 12].Value = "PHIC Number";
                worksheet.Cells[1, 13].Value = "TIN";
                worksheet.Cells[1, 14].Value = "ATM Account Number";
                worksheet.Cells[1, 15].Value = "Company";
                worksheet.Cells[1, 16].Value = "Branch";
                worksheet.Cells[1, 17].Value = "Department";
                worksheet.Cells[1, 18].Value = "Position";
                worksheet.Cells[1, 19].Value = "Monthly Rate";
                worksheet.Cells[1, 20].Value = "Daily Rate";
                worksheet.Cells[1, 21].Value = "Allowance";
                worksheet.Cells[1, 22].Value = "Mobile Code";

                // Make headers bold
                using (var range = worksheet.Cells[1, 1, 1, 22])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                // Add data
                for (int i = 0; i < employees.Count; i++)
                {
                    var employee = employees[i];
                    worksheet.Cells[i + 2, 1].Value = employee.Id;
                    worksheet.Cells[i + 2, 2].Value = employee.BiometricIdNumber;
                    worksheet.Cells[i + 2, 3].Value = employee.FullName;
                    worksheet.Cells[i + 2, 4].Value = employee.CellphoneNumber;
                    worksheet.Cells[i + 2, 5].Value = employee.EmailAddress;
                    worksheet.Cells[i + 2, 6].Value = employee.DepartmentId;
                    worksheet.Cells[i + 2, 7].Value = employee.DepartmentName;
                    worksheet.Cells[i + 2, 8].Value = employee.IsLocked;
                    worksheet.Cells[i + 2, 9].Value = employee.Gsisnumber;
                    worksheet.Cells[i + 2, 10].Value = employee.Sssnumber;
                    worksheet.Cells[i + 2, 11].Value = employee.Hdmfnumber;
                    worksheet.Cells[i + 2, 12].Value = employee.Phicnumber;
                    worksheet.Cells[i + 2, 13].Value = employee.Tin;
                    worksheet.Cells[i + 2, 14].Value = employee.AtmaccountNumber;
                    worksheet.Cells[i + 2, 15].Value = employee.Company;
                    worksheet.Cells[i + 2, 16].Value = employee.Branch;
                    worksheet.Cells[i + 2, 17].Value = employee.Department;
                    worksheet.Cells[i + 2, 18].Value = employee.Position;
                    worksheet.Cells[i + 2, 19].Value = employee.MonthlyRate;
                    worksheet.Cells[i + 2, 20].Value = employee.DailyRate;
                    worksheet.Cells[i + 2, 21].Value = employee.Allowance;
                    worksheet.Cells[i + 2, 22].Value = employee.MobileCode;
                }

                //for (int col = 9; col <= 17; col++)
                //{
                //    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                //}

                // Format MonthlyRate, DailyRate, and Allowance as N2
                worksheet.Cells[2, 19, employees.Count + 1, 19].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[2, 20, employees.Count + 1, 20].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[2, 21, employees.Count + 1, 21].Style.Numberformat.Format = "#,##0.00";                

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Employees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}