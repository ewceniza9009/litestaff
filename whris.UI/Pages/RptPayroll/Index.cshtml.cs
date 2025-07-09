using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Data;
using whris.Data.Models;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;


namespace whris.UI.Pages.RptPayroll
{
    [Authorize]
    [Secure("RepPayroll")]
    public class IndexModel : PageModel
    {
        public class ExcelRequestModel
        {
            public int ParamPayrollId { get; set; }

            public string? ParamEmploymentType { get; set; }

            public int ParamBranchId { get; set; }

            public int ParamCompanyId { get; set; }
        }
        public List<ReportList> Reports { get; set; } = new List<ReportList>();
        public MstEmployeeComboboxDatasources ComboboxDatasources = MstEmployeeComboboxDatasources.Instance;
        public List<TrnPayrollDto> PayrollNumbers => (List<TrnPayrollDto>)(Common.GetPayrollNumbers()?.Value ?? new List<TrnPayrollDto>());
        public List<MstEmploymentTypeDto> EmploymentTypeCmbDs => new List<MstEmploymentTypeDto>()
        {
            new MstEmploymentTypeDto() { Id = 1, EmploymentType = "Regular" },
            new MstEmploymentTypeDto() { Id = 2, EmploymentType = "Probationary"},
            new MstEmploymentTypeDto() { Id = 3, EmploymentType = "Newly Hired"}
        };
        public List<MstEmployeeDto> Employees => (List<MstEmployeeDto>)(Common.GetEmployees()?.Value ?? new List<MstEmployeeDto>());
        public List<MstPayrollGroup> PayrollGroupCmbDs => (List<MstPayrollGroup>)(Common.GetPayrollGroups()?.Value ?? new List<MstPayrollGroup>());
        public List<MstMonth> MonthCmbDs => (List<MstMonth>)(Common.GetMonths()?.Value ?? new List<MstMonth>());        
        public List<MstPeriod> PeriodCmbDs => (List<MstPeriod>)(Common.GetPeriods()?.Value ?? new List<MstPeriod>());
        public int DefaultMonth { get; set; } = DateTime.Now.Month;
        public int DefaultPeriod { get; set; } = DateTime.Now.Year;

        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Payslip" },
                new ReportList(){ Value = "2", Text = "Payslip (Lengthwise)" },
                new ReportList(){ Value = "2.1", Text = "Payslip (Continues)" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "3.1", Text = "Payroll Worksheet w/ Other Income & Deduction Breakdown (Excel)" },
                new ReportList(){ Value = "3.2", Text = "" },
                new ReportList(){ Value = "4", Text = "Payroll Worksheet w/ No. of Hrs" },
                new ReportList(){ Value = "5", Text = "Payroll Worksheet w/ Departments" },
                new ReportList(){ Value = "5.1", Text = "" },
                new ReportList(){ Value = "6", Text = "Monthly Payroll Worksheet" },
                new ReportList(){ Value = "8", Text = "" },
                new ReportList(){ Value = "9", Text = "Other Deduction Detail Report" },
                new ReportList(){ Value = "10", Text = "Other Income Detail Report" },
            };
        }

        public IActionResult OnPostExportToExcelAsync(ExcelRequestModel request)
        {
            var results = new List<IDictionary<string, object>>();
            using var connection = new SqlConnection(Config.ConnectionString);

            var parameters = new
            {
                request.ParamPayrollId,
                ParamEmploymentType = string.IsNullOrEmpty(request.ParamEmploymentType) ? null : request.ParamEmploymentType,
                request.ParamBranchId,
                request.ParamCompanyId
            };

            using (var reader = connection.ExecuteReader(GetSqlScript(), parameters))
            {
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        var value = reader.GetValue(i);
                        row[name] = value == DBNull.Value ? null : value;
                    }
                    results.Add(row);
                }
            }

            if (!results.Any())
            {
                return Page();
            }

            var dataTable = new DataTable();
            var headers = results.First().Keys.ToList();
            foreach (var header in headers)
            {
                var firstValue = results.First()[header];
                dataTable.Columns.Add(header, firstValue?.GetType() ?? typeof(string));
            }
            foreach (var row in results)
            {
                dataTable.Rows.Add(row.Values.ToArray());
            }

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Payroll Worksheet w/ Other Income & Deduction Breakdown");

                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

                using (var range = worksheet.Cells[1, 1, 1, headers.Count])
                {
                    range.Style.Font.Bold = true;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                package.SaveAs(stream);
                stream.Position = 0;
            }

            var excelName = $"PayrollWorksheetWIncomeDeductionBreakdown_{DateTime.Now:yyyyMMdd}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        private string GetSqlScript()
        {
            return @"
            DECLARE @deductionCols AS NVARCHAR(MAX),
                    @deductionColsForSubquery AS NVARCHAR(MAX),
                    @deductionColsForFinalSelect AS NVARCHAR(MAX),
                    @incomeCols AS NVARCHAR(MAX),
                    @incomeColsForSubquery AS NVARCHAR(MAX),
                    @incomeColsForFinalSelect AS NVARCHAR(MAX),
                    @sql AS NVARCHAR(MAX);

            WITH DeductionNames AS (
                SELECT DISTINCT OD.OtherDeduction
                FROM MstOtherDeduction OD
                INNER JOIN TrnPayrollOtherDeductionLine ODL ON OD.Id = ODL.OtherDeductionId
                INNER JOIN TrnPayrollOtherDeduction POD ON ODL.PayrollOtherDeductionId = POD.Id
                INNER JOIN TrnPayroll P ON POD.Id = P.PayrollOtherDeductionId
                WHERE P.Id = @ParamPayrollId AND ODL.Amount <> 0
            )
            SELECT
                @deductionCols = STUFF((SELECT ',' + QUOTENAME(OtherDeduction) FROM DeductionNames FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''),
                @deductionColsForSubquery = STUFF((SELECT ',' + QUOTENAME(OtherDeduction) FROM DeductionNames FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''),
                @deductionColsForFinalSelect = STUFF((SELECT ', ISNULL(PivotedDeductions.' + QUOTENAME(OtherDeduction) + ', 0) AS ' + QUOTENAME(OtherDeduction + '_Deduction') FROM DeductionNames FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

            WITH IncomeNames AS (
                SELECT DISTINCT OI.OtherIncome
                FROM MstOtherIncome OI
                INNER JOIN TrnPayrollOtherIncomeLine OIL ON OI.Id = OIL.OtherIncomeId
                INNER JOIN TrnPayrollOtherIncome POI ON OIL.PayrollOtherIncomeId = POI.Id
                INNER JOIN TrnPayroll P ON POI.Id = P.PayrollOtherIncomeId
                WHERE P.Id = @ParamPayrollId AND OIL.Amount <> 0
            )
            SELECT
                @incomeCols = STUFF((SELECT ',' + QUOTENAME(OtherIncome) FROM IncomeNames FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''),
                @incomeColsForSubquery = STUFF((SELECT ',' + QUOTENAME(OtherIncome) FROM IncomeNames FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''),
                @incomeColsForFinalSelect = STUFF((SELECT ', ISNULL(PivotedIncomes.' + QUOTENAME(OtherIncome) + ', 0) AS ' + QUOTENAME(OtherIncome + '_Income') FROM IncomeNames FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

            SET @sql = N'
            SELECT
                MainReport.*'
                + CASE WHEN @incomeColsForFinalSelect IS NOT NULL THEN N', ' + @incomeColsForFinalSelect ELSE N'' END
                + CASE WHEN @deductionColsForFinalSelect IS NOT NULL THEN N', ' + @deductionColsForFinalSelect ELSE N'' END
            + N'
            FROM
                (
                    SELECT
                        TrnPayrollLine.PayrollId, TrnPayroll.PayrollNumber, TrnPayroll.PayrollDate, TrnPayroll.Remarks,
                        TrnPayrollLine.EmployeeId, MstEmployee.FullName,
                        [TotalRegularWorkingHours] + [TotalLegalHolidayWorkingHours] + [TotalSpecialHolidayWorkingHours] AS TotalWorkingHours,
                        [TotalRegularRestdayHours] + [TotalLegalHolidayRestdayHours] + [TotalSpecialHolidayRestdayHours] AS TotalRestdayHours,
                        [TotalRegularOvertimeHours] + [TotalLegalHolidayOvertimeHours] + [TotalSpecialHolidayOvertimeHours] + [TotalSpecialHolidayNightHours] AS TotalOverTimeHours,
                        [TotalRegularNightHours] + [TotalLegalHolidayNightHours] AS TotalNightHours,
                        [TotalTardyLateHours] + [TotalTardyUndertimeHours] AS TotalTardyHours,
                        IIF([MstEmployee].[PayrollTypeId] = 2, [mstEmployee].[PayrollRate], 0) AS FixBasicSalary,
                        IIF([MstEmployee].[PayrollTypeId] = 1, [TotalSalaryAmount] - [TotalLegalHolidayWorkingAmount] - [TotalSpecialHolidayWorkingAmount] - [TotalRegularRestdayAmount] - [TotalLegalHolidayRestdayAmount] - [TotalSpecialHolidayRestdayAmount] - [TotalRegularOvertimeAmount] - [TotalLegalHolidayOvertimeAmount] - [TotalSpecialHolidayOvertimeAmount] - [TotalRegularNightAmount] - [TotalLegalHolidayNightAmount] - [TotalSpecialHolidayNightAmount] - [TotalRegularNightOvertimeAmount] - [TotalLegalHolidayNightOvertimeAmount] - [TotalSpecialHolidayNightOvertimeAmount], 0) AS VariableBasicSalary,
                        [TotalLegalHolidayWorkingAmount] + [TotalSpecialHolidayWorkingAmount] + [TotalRegularRestdayAmount] + [TotalLegalHolidayRestdayAmount] + [TotalSpecialHolidayRestdayAmount] + [TotalRegularOvertimeAmount] + [TotalLegalHolidayOvertimeAmount] + [TotalSpecialHolidayOvertimeAmount] + [TotalRegularNightAmount] + [TotalLegalHolidayNightAmount] + [TotalSpecialHolidayNightAmount] + [TotalRegularNightOvertimeAmount] + [TotalLegalHolidayNightOvertimeAmount] + [TotalSpecialHolidayNightOvertimeAmount] AS OtherSalary,
                        TrnPayrollLine.TotalSalaryAmount, TrnPayrollLine.TotalTardyAmount, TrnPayrollLine.TotalAbsentAmount, TrnPayrollLine.TotalNetSalaryAmount,
                        TrnPayrollLine.TotalOtherIncomeTaxable, TrnPayrollLine.GrossIncome, TrnPayrollLine.TotalOtherIncomeNonTaxable,
                        TrnPayrollLine.GrossIncomeWithNonTaxable, TrnPayrollLine.SSSContribution, TrnPayrollLine.SSSECContribution,
                        TrnPayrollLine.SSSContribution AS SSSContributionTotal, TrnPayrollLine.PHICContribution, TrnPayrollLine.HDMFContribution,
                        TrnPayrollLine.Tax, TrnPayrollLine.TotalOtherDeduction, TrnPayrollLine.NetIncome, TrnPayroll.PreparedBy,
                        TrnPayroll.CheckedBy, TrnPayroll.ApprovedBy,
                        IIF(@ParamBranchId = 0, ''All'', MstBranch.Branch) as Branch,
                        IIF(@ParamCompanyId = 0, ''All'', MstCompany.Company) as Company
                    FROM TrnPayrollLine
                    INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id
                    INNER JOIN MstEmployee ON TrnPayrollLine.EmployeeId = MstEmployee.Id
                    INNER JOIN MstBranch ON MstEmployee.BranchId = MstBranch.Id
                    INNER JOIN MstCompany ON MstEmployee.CompanyId = MstCompany.Id
                    WHERE TrnPayroll.IsLocked = 1
                        AND TrnPayrollLine.PayrollId = @ParamPayrollId
                        AND ISNULL(MstEmployee.EmploymentType, 1) LIKE ISNULL(@ParamEmploymentType, ''%'')
                        AND (@ParamBranchId = 0 OR MstEmployee.BranchId = @ParamBranchId)
                        AND (@ParamCompanyId = 0 OR MstEmployee.CompanyId = @ParamCompanyId)
                ) AS MainReport'
            + CASE WHEN @deductionCols IS NOT NULL THEN N'
            LEFT JOIN
                (
                    SELECT PayrollId, EmployeeId, ' + @deductionColsForSubquery + N'
                    FROM (
                        SELECT TrnPayroll.Id AS PayrollId, TrnPayrollOtherDeductionLine.EmployeeId, MstOtherDeduction.OtherDeduction, TrnPayrollOtherDeductionLine.Amount
                        FROM TrnPayrollOtherDeductionLine
                        INNER JOIN TrnPayrollOtherDeduction ON TrnPayrollOtherDeduction.Id = TrnPayrollOtherDeductionLine.PayrollOtherDeductionId
                        INNER JOIN TrnPayroll ON TrnPayrollOtherDeduction.Id = TrnPayroll.PayrollOtherDeductionId
                        INNER JOIN MstEmployee ON MstEmployee.Id = TrnPayrollOtherDeductionLine.EmployeeId
                        INNER JOIN MstOtherDeduction ON MstOtherDeduction.Id = TrnPayrollOtherDeductionLine.OtherDeductionId
                        WHERE TrnPayroll.IsLocked = 1 AND TrnPayroll.Id = @ParamPayrollId
                    ) AS SourceData
                    PIVOT (SUM(Amount) FOR OtherDeduction IN (' + @deductionCols + N')) AS PivotTable
                ) AS PivotedDeductions ON MainReport.PayrollId = PivotedDeductions.PayrollId AND MainReport.EmployeeId = PivotedDeductions.EmployeeId'
            ELSE N'' END
            + CASE WHEN @incomeCols IS NOT NULL THEN N'
            LEFT JOIN
                (
                    SELECT PayrollId, EmployeeId, ' + @incomeColsForSubquery + N'
                    FROM (
                        SELECT TrnPayroll.Id AS PayrollId, TrnPayrollOtherIncomeLine.EmployeeId, MstOtherIncome.OtherIncome, TrnPayrollOtherIncomeLine.Amount
                        FROM TrnPayrollOtherIncomeLine
                        INNER JOIN TrnPayrollOtherIncome ON TrnPayrollOtherIncome.Id = TrnPayrollOtherIncomeLine.PayrollOtherIncomeId
                        INNER JOIN TrnPayroll ON TrnPayrollOtherIncome.Id = TrnPayroll.PayrollOtherIncomeId
                        INNER JOIN MstEmployee ON MstEmployee.Id = TrnPayrollOtherIncomeLine.EmployeeId
                        INNER JOIN MstOtherIncome ON MstOtherIncome.Id = TrnPayrollOtherIncomeLine.OtherIncomeId
                        WHERE TrnPayroll.IsLocked = 1 AND TrnPayroll.Id = @ParamPayrollId
                    ) AS SourceData
                    PIVOT (SUM(Amount) FOR OtherIncome IN (' + @incomeCols + N')) AS PivotTable
                ) AS PivotedIncomes ON MainReport.PayrollId = PivotedIncomes.PayrollId AND MainReport.EmployeeId = PivotedIncomes.EmployeeId'
            ELSE N'' END
            + N'
            ORDER BY
                MainReport.FullName;
            ';

            EXEC sp_executesql @sql,
                N'@ParamPayrollId INT, @ParamEmploymentType NVARCHAR(50), @ParamBranchId INT, @ParamCompanyId INT',
                @ParamPayrollId = @ParamPayrollId,
                @ParamEmploymentType = @ParamEmploymentType,
                @ParamBranchId = @ParamBranchId,
                @ParamCompanyId = @ParamCompanyId;
            ";
        }
    }
}
