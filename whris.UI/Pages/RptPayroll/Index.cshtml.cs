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

        public class MonthlySummaryExcelRequestModel
        {
            public DateTime? ParamDateStart { get; set; }

            public DateTime? ParamDateEnd { get; set; }

            public int ParamStartMonthId { get; set; }

            public int ParamEndMonthId { get; set; }

            public int ParamDepartmentId { get; set; }

            public int ParamPositionId { get; set; }

            public int ParamCompanyId { get; set; }

            public int ParamBranchId { get; set; }

            public int ParamPayrollGroupId { get; set; }

            public string? ParamEmploymentType { get; set; }

            public int ParamEmployeeId { get; set; }
        }

        private class MonthlySummaryRow
        {
            public int EmployeeId { get; set; }

            public string Department { get; set; } = string.Empty;

            public string Position { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;

            public string FirstName { get; set; } = string.Empty;

            public decimal? Allowance { get; set; }

            public string MiddleName { get; set; } = string.Empty;

            public int MonthId { get; set; }

            public decimal MonthlyRateAmount { get; set; }

            public decimal ActualNetSalaryAmount { get; set; }
        }

        private static readonly string[] MonthHeaders =
        {
            string.Empty, "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"
        };

        private static readonly System.Drawing.Color RateChangeColor = System.Drawing.Color.FromArgb(255, 242, 204);
        private static readonly System.Drawing.Color FirstZeroColor = System.Drawing.Color.FromArgb(255, 192, 0);
        private static readonly System.Drawing.Color ConsecutiveZeroColor = System.Drawing.Color.FromArgb(217, 217, 217);
        private static readonly System.Drawing.Color InactiveNameColor = System.Drawing.Color.FromArgb(191, 191, 191);
        private static readonly System.Drawing.Color CellBorderColor = System.Drawing.Color.FromArgb(208, 208, 208);
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
        public int DefaultPeriod { get; set; } =
            ((List<MstPeriod>)Common.GetPeriods()?.Value ?? new List<MstPeriod>())
            .FirstOrDefault(p => p.Period == DateTime.Now.Year.ToString())
            ?.Id ?? 0;



        public void OnGet()
        {
            Reports = new List<ReportList>
            {
                new ReportList(){ Value = "1", Text = "Payslip" },
                new ReportList(){ Value = "2", Text = "Payslip (Lengthwise)" },
                new ReportList(){ Value = "2.1", Text = "Payslip (Continues)" },
                new ReportList(){ Value = "3", Text = "" },
                new ReportList(){ Value = "3.1", Text = "Payroll Worksheet w/ Other Income & Deduction Breakdown (Excel)" },
                new ReportList(){ Value = "3.3", Text = "Payroll Worksheet w/ Other Income & Deduction Breakdown (Excel) - Legacy" },
                new ReportList(){ Value = "3.2", Text = "" },
                new ReportList(){ Value = "4", Text = "Payroll Worksheet w/ No. of Hrs" },
                new ReportList(){ Value = "5", Text = "Payroll Worksheet w/ Departments" },
                new ReportList(){ Value = "5.1", Text = "" },
                new ReportList(){ Value = "6", Text = "Monthly Payroll Worksheet" },
                new ReportList(){ Value = "6.1", Text = "" },
                new ReportList(){ Value = "7.1", Text = "Monthly Rate Summary Report (Excel)" },
                new ReportList(){ Value = "7.2", Text = "Actual Monthly Salary Report - Net (Excel)" },
                new ReportList(){ Value = "8", Text = "" },
                new ReportList(){ Value = "9", Text = "Other Deduction Detail Report" },
                new ReportList(){ Value = "10", Text = "Other Income Detail Report" },
                new ReportList(){ Value = "11", Text = "" },
                new ReportList(){ Value = "12", Text = "13th Month Pay Report" },
                new ReportList(){ Value = "13", Text = "13th Month Pay Report (Summary)" },
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

            // Fetch payroll info for headers
            var payrollInfo = connection.QueryFirstOrDefault("SELECT Remarks, PayrollDate FROM TrnPayroll WHERE Id = @ParamPayrollId", new { request.ParamPayrollId });
            string periodCovered = payrollInfo?.Remarks ?? "";
            DateTime? payrollDate = payrollInfo?.PayrollDate;

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

            string companyName = results.First().ContainsKey("CompanyName") ? results.First()["CompanyName"]?.ToString() ?? "CEBU GOLDEN RESTAURANT INC" : "CEBU GOLDEN RESTAURANT INC";

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
                var worksheet = package.Workbook.Worksheets.Add("Payroll Worksheet");

                // Headers at the top
                worksheet.Cells["A1"].Value = companyName;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 14;

                worksheet.Cells["A2"].Value = $"PERIOD COVERED: {periodCovered}";
                worksheet.Cells["A3"].Value = $"PAYROLL FOR: {payrollDate?.ToString("MMMM d, yyyy")}";

                // Load data starting from A5
                worksheet.Cells["A5"].LoadFromDataTable(dataTable, true);

                // Style the headers at row 5
                using (var range = worksheet.Cells[5, 1, 5, headers.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[5, 1, 5, headers.Count].AutoFilter = true;
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
            --DECLARE @ParamPayrollId AS INTEGER;
                --DECLARE @ParamEmploymentType AS NVARCHAR(50);
                --DECLARE @ParamBranchId AS INTEGER;
                --DECLARE @ParamCompanyId AS INTEGER;

                --SET @ParamPayrollId = 24;
                --SET @ParamEmploymentType = NULL;
                --SET @ParamBranchId = 0;
                --SET @ParamCompanyId = 0;

                DECLARE @deductionCols AS NVARCHAR(MAX),
                        @deductionColsForSubquery AS NVARCHAR(MAX),
                        @deductionColsForFinalSelect AS NVARCHAR(MAX),
                        @incomeCols AS NVARCHAR(MAX),
                        @incomeColsForSubquery AS NVARCHAR(MAX),
                        @incomeColsForFinalSelect AS NVARCHAR(MAX),
                        @sql AS NVARCHAR(MAX);

                -- Prepare dynamic deduction columns
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

                -- Prepare dynamic income columns
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

                -- Construct the final dynamic SQL query
                SET @sql = N'
                SELECT
                    MainReport.FullName,
                    IIF(MainReport.PayrollTypeId = 2, ''BANK'', ''CASH'') AS [Cash/Bank],
                    MainReport.BranchName,
                    MainReport.CompanyName,
                    MainReport.TotalWorkingHours,
                    MainReport.TotalRestdayHours,
                    MainReport.TotalOverTimeHours,
                    MainReport.TotalNightHours,
                    MainReport.TotalTardyHours,
                    MainReport.FixBasicSalary,
                    MainReport.VariableBasicSalary,
                    0 AS [UNCREDITED HOURS],
                    0 AS [1 DAY],
                    0 AS [ASSUMED 4 DAYS],
                    0 AS [UNDERTIME],
                    0 AS [OVER PAY],
                    MainReport.TotalSalaryAmount AS [Total Worked Amount],
                    MainReport.OtherSalary,
                    0 AS [OVERTIME],
                    0 AS [OVERDEDUCTION],
                    0 AS [Premium- Regular Holiday],
                    MainReport.TotalSalaryAmount + MainReport.OtherSalary AS [Totals] '
                    + CASE WHEN @incomeColsForFinalSelect IS NOT NULL THEN N', ' + @incomeColsForFinalSelect ELSE N'' END
                    + N', MainReport.GrossIncome AS [GROSS PAY] '
                    + CASE WHEN @deductionColsForFinalSelect IS NOT NULL THEN N', ' + @deductionColsForFinalSelect ELSE N'' END
                    + N', MainReport.SSSContribution,
                    MainReport.PHICContribution AS [PHIC],
                    MainReport.SSSContribution + MainReport.PHICContribution + MainReport.HDMFContribution + MainReport.Tax + MainReport.TotalOtherDeduction AS [TOTAL DEDUCTIONS],
                    MainReport.NetIncome AS [NET PAY]
                FROM
                    (
                        SELECT
                            TrnPayrollLine.PayrollId, 
                            TrnPayrollLine.EmployeeId,
                            MstEmployee.PayrollTypeId,
                            TrnPayroll.PayrollNumber, 
                            TrnPayroll.PayrollDate, 
                            TrnPayroll.Remarks,
                            MstBranch.Branch as BranchName,
                            MstCompany.Company as CompanyName,
                            MstEmployee.FullName,
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
                            TrnPayrollLine.PHICContribution, TrnPayrollLine.HDMFContribution,
                            TrnPayrollLine.Tax, TrnPayrollLine.TotalOtherDeduction, TrnPayrollLine.NetIncome              
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
                -- Join pivoted deduction data
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
                -- Join pivoted income data
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

                -- Execute the final query
                EXEC sp_executesql @sql,
                    N'@ParamPayrollId INT, @ParamEmploymentType NVARCHAR(50), @ParamBranchId INT, @ParamCompanyId INT',
                    @ParamPayrollId = @ParamPayrollId,
                    @ParamEmploymentType = @ParamEmploymentType,
                    @ParamBranchId = @ParamBranchId,
                    @ParamCompanyId = @ParamCompanyId;";
        }
        public IActionResult OnPostExportToExcelLegacyAsync(ExcelRequestModel request)
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

            using (var reader = connection.ExecuteReader(GetSqlScriptLegacy(), parameters))
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

            var excelName = $"PayrollWorksheetWIncomeDeductionBreakdown_Legacy_{DateTime.Now:yyyyMMdd}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        private string GetSqlScriptLegacy()
        {
            return @"
            --DECLARE @ParamPayrollId AS INTEGER;
                --DECLARE @ParamEmploymentType AS NVARCHAR(50);
                --DECLARE @ParamBranchId AS INTEGER;
                --DECLARE @ParamCompanyId AS INTEGER;

                --SET @ParamPayrollId = 24;
                --SET @ParamEmploymentType = NULL;
                --SET @ParamBranchId = 0;
                --SET @ParamCompanyId = 0;

                DECLARE @deductionCols AS NVARCHAR(MAX),
                        @deductionColsForSubquery AS NVARCHAR(MAX),
                        @deductionColsForFinalSelect AS NVARCHAR(MAX),
                        @incomeCols AS NVARCHAR(MAX),
                        @incomeColsForSubquery AS NVARCHAR(MAX),
                        @incomeColsForFinalSelect AS NVARCHAR(MAX),
                        @sql AS NVARCHAR(MAX);

                -- Prepare dynamic deduction columns
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

                -- Prepare dynamic income columns
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

                -- Construct the final dynamic SQL query
                SET @sql = N'
                SELECT
                    -- Explicitly list all columns from MainReport except PayrollId, EmployeeId, PayrollNumber, PayrollDate
                    MainReport.Remarks AS PayrollRemarks,
                    MainReport.BranchName,
                    MainReport.CompanyName,
                    MainReport.FullName,
                    MainReport.TotalWorkingHours,
                    MainReport.TotalRestdayHours,
                    MainReport.TotalOverTimeHours,
                    MainReport.TotalNightHours,
                    MainReport.TotalTardyHours,
                    MainReport.FixBasicSalary,
                    MainReport.VariableBasicSalary,
                    MainReport.OtherSalary,
                    MainReport.TotalSalaryAmount, 
                    MainReport.TotalTardyAmount, 
                    MainReport.TotalAbsentAmount, 
                    MainReport.TotalNetSalaryAmount,
                    MainReport.TotalOtherIncomeTaxable, 
                    MainReport.GrossIncome, 
                    MainReport.TotalOtherIncomeNonTaxable,
                    MainReport.GrossIncomeWithNonTaxable, 
                    MainReport.SSSContribution, 
                    MainReport.SSSECContribution,
                    MainReport.SSSContributionTotal, 
                    MainReport.PHICContribution, 
                    MainReport.HDMFContribution,
                    MainReport.Tax, 
                    MainReport.TotalOtherDeduction, 
                    MainReport.NetIncome'
                    -- Append dynamic income and deduction columns
                    + CASE WHEN @incomeColsForFinalSelect IS NOT NULL THEN N', ' + @incomeColsForFinalSelect ELSE N'' END
                    + CASE WHEN @deductionColsForFinalSelect IS NOT NULL THEN N', ' + @deductionColsForFinalSelect ELSE N'' END
                + N'
                FROM
                    (
                        SELECT
                            -- PayrollId and EmployeeId are selected here for JOINing purposes
                            -- but are not included in the final SELECT list above.
                            TrnPayrollLine.PayrollId, 
                            TrnPayrollLine.EmployeeId,
                            TrnPayroll.PayrollNumber, 
                            TrnPayroll.PayrollDate, 
                            TrnPayroll.Remarks,
                            MstBranch.Branch as BranchName,
                            MstCompany.Company as CompanyName,
                            MstEmployee.FullName,
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
                            TrnPayrollLine.Tax, TrnPayrollLine.TotalOtherDeduction, TrnPayrollLine.NetIncome              
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
                -- Join pivoted deduction data
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
                -- Join pivoted income data
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

                -- Execute the final query
                EXEC sp_executesql @sql,
                    N'@ParamPayrollId INT, @ParamEmploymentType NVARCHAR(50), @ParamBranchId INT, @ParamCompanyId INT',
                    @ParamPayrollId = @ParamPayrollId,
                    @ParamEmploymentType = @ParamEmploymentType,
                    @ParamBranchId = @ParamBranchId,
                    @ParamCompanyId = @ParamCompanyId;";
        }

        public async Task<IActionResult> OnPostExportMonthlyRateToExcelAsync(MonthlySummaryExcelRequestModel request)
        {
            return await GenerateMonthlySummaryExcelAsync(request, isActualNetSalary: false);
        }

        public async Task<IActionResult> OnPostExportActualMonthlySalaryToExcelAsync(MonthlySummaryExcelRequestModel request)
        {
            return await GenerateMonthlySummaryExcelAsync(request, isActualNetSalary: true);
        }

        private async Task<IActionResult> GenerateMonthlySummaryExcelAsync(MonthlySummaryExcelRequestModel request, bool isActualNetSalary)
        {
            var (startMonth, endMonth) = ResolveMonthRange(request.ParamStartMonthId, request.ParamEndMonthId);

            using var connection = new SqlConnection(Config.ConnectionString);

            var parameters = new
            {
                ParamDateStart = request.ParamDateStart?.Date,
                ParamDateEnd = request.ParamDateEnd?.Date.AddDays(1).AddSeconds(-1),
                ParamStartMonthId = startMonth,
                ParamEndMonthId = endMonth,
                request.ParamDepartmentId,
                request.ParamPositionId,
                request.ParamCompanyId,
                request.ParamBranchId,
                request.ParamPayrollGroupId,
                request.ParamEmployeeId,
                ParamEmploymentType = string.IsNullOrEmpty(request.ParamEmploymentType) ? null : request.ParamEmploymentType
            };

            var rows = (await connection.QueryAsync<MonthlySummaryRow>(GetMonthlySummarySqlScript(), parameters)).ToList();

            var dataTable = BuildMonthlySummaryDataTable(rows, startMonth, endMonth, isActualNetSalary);

            var worksheetName = isActualNetSalary ? "Actual Monthly Salary (Net)" : "Monthly Rate";
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

                var totalColumns = dataTable.Columns.Count;
                using (var headerRange = worksheet.Cells[1, 1, 1, totalColumns])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    headerRange.AutoFilter = true;
                }

                if (dataTable.Rows.Count > 0)
                {
                    var lastDataRow = 1 + dataTable.Rows.Count;
                    worksheet.Cells[2, 5, lastDataRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[2, 7, lastDataRow, totalColumns].Style.Numberformat.Format = "#,##0.00";

                    ApplyMonthlySummaryColorCoding(worksheet, rows, startMonth, endMonth, isActualNetSalary);
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                package.SaveAs(stream);
                stream.Position = 0;
            }

            var filePrefix = isActualNetSalary ? "ActualMonthlySalaryReport_Net" : "MonthlyRateSummaryReport";
            var excelName = $"{filePrefix}_{DateTime.Now:yyyyMMdd}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        private static (int startMonth, int endMonth) ResolveMonthRange(int paramStartMonthId, int paramEndMonthId)
        {
            if (paramStartMonthId > 0 && paramEndMonthId > 0)
            {
                var start = Math.Clamp(Math.Min(paramStartMonthId, paramEndMonthId), 1, 12);
                var end = Math.Clamp(Math.Max(paramStartMonthId, paramEndMonthId), 1, 12);
                return (start, end);
            }

            if (paramStartMonthId > 0)
            {
                return (1, Math.Clamp(paramStartMonthId, 1, 12));
            }

            if (paramEndMonthId > 0)
            {
                return (1, Math.Clamp(paramEndMonthId, 1, 12));
            }

            return (1, 12);
        }

        private static DataTable BuildMonthlySummaryDataTable(
            List<MonthlySummaryRow> rows,
            int startMonth,
            int endMonth,
            bool isActualNetSalary)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("DEPARTMENT", typeof(string));
            dataTable.Columns.Add("POSITION", typeof(string));
            dataTable.Columns.Add("FAMILY NAME", typeof(string));
            dataTable.Columns.Add("GIVEN NAME", typeof(string));
            dataTable.Columns.Add("Allowance", typeof(decimal));
            dataTable.Columns.Add("MIDDLE NAME", typeof(string));

            for (var month = startMonth; month <= endMonth; month++)
            {
                dataTable.Columns.Add(MonthHeaders[month], typeof(decimal));
            }

            foreach (var empGroup in rows.GroupBy(r => r.EmployeeId))
            {
                var first = empGroup.First();
                var dataRow = dataTable.NewRow();

                dataRow["DEPARTMENT"] = first.Department;
                dataRow["POSITION"] = first.Position;
                dataRow["FAMILY NAME"] = first.LastName;
                dataRow["GIVEN NAME"] = first.FirstName;
                dataRow["Allowance"] = first.Allowance.HasValue && first.Allowance.Value != 0
                    ? first.Allowance.Value
                    : DBNull.Value;
                dataRow["MIDDLE NAME"] = first.MiddleName;

                var monthLookup = BuildMonthLookup(empGroup);

                for (var month = startMonth; month <= endMonth; month++)
                {
                    var colName = MonthHeaders[month];
                    if (monthLookup.TryGetValue(month, out var monthRow))
                    {
                        dataRow[colName] = isActualNetSalary
                            ? monthRow.ActualNetSalaryAmount
                            : monthRow.MonthlyRateAmount;
                    }
                    else
                    {
                        dataRow[colName] = DBNull.Value;
                    }
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private static Dictionary<int, MonthlySummaryRow> BuildMonthLookup(IGrouping<int, MonthlySummaryRow> empGroup)
        {
            return empGroup
                .GroupBy(r => r.MonthId)
                .ToDictionary(g => g.Key, g => g.First());
        }

        /// <summary>
        /// Highlights onboarding months, rate changes, partial pay (&lt;= 5,000), return-to-work, and zero/inactive months.
        /// </summary>
        private static void ApplyMonthlySummaryColorCoding(
            ExcelWorksheet worksheet,
            List<MonthlySummaryRow> rows,
            int startMonth,
            int endMonth,
            bool isActualNetSalary)
        {
            var monthCount = endMonth - startMonth + 1;
            var excelRow = 2;

            foreach (var empGroup in rows.GroupBy(r => r.EmployeeId))
            {
                var monthLookup = BuildMonthLookup(empGroup);
                var displayValues = new decimal?[monthCount];
                var rateValues = new decimal?[monthCount];

                for (var i = 0; i < monthCount; i++)
                {
                    var month = startMonth + i;
                    if (monthLookup.TryGetValue(month, out var monthRow))
                    {
                        displayValues[i] = isActualNetSalary
                            ? monthRow.ActualNetSalaryAmount
                            : monthRow.MonthlyRateAmount;
                        rateValues[i] = monthRow.MonthlyRateAmount;
                    }
                }

                var firstPositiveIdx = Array.FindIndex(displayValues, v => v.HasValue && v.Value > 0m);

                if (firstPositiveIdx == -1)
                {
                    SetCellFill(worksheet.Cells[excelRow, 3, excelRow, 6], InactiveNameColor);

                    for (var i = 0; i < monthCount; i++)
                    {
                        if (displayValues[i] == 0m)
                        {
                            SetCellFill(worksheet.Cells[excelRow, 7 + i], ConsecutiveZeroColor);
                        }
                    }

                    excelRow++;
                    continue;
                }

                if (firstPositiveIdx > 0)
                {
                    var joinColor = GetMonthHighlightColor(startMonth + firstPositiveIdx);
                    SetCellFill(worksheet.Cells[excelRow, 3, excelRow, 6], joinColor);
                    SetCellFill(worksheet.Cells[excelRow, 7 + firstPositiveIdx], joinColor);

                    for (var i = 0; i < firstPositiveIdx; i++)
                    {
                        if (displayValues[i] == 0m)
                        {
                            SetCellFill(worksheet.Cells[excelRow, 7 + i], ConsecutiveZeroColor);
                        }
                    }
                }
                else if (displayValues[0] <= 5000m)
                {
                    SetCellFill(worksheet.Cells[excelRow, 7], GetMonthHighlightColor(startMonth));
                }

                for (var i = firstPositiveIdx + 1; i < monthCount; i++)
                {
                    var fillColor = GetSubsequentMonthColor(
                        startMonth + i,
                        displayValues[i],
                        displayValues[i - 1],
                        rateValues[i],
                        rateValues[i - 1]);

                    if (fillColor.HasValue)
                    {
                        SetCellFill(worksheet.Cells[excelRow, 7 + i], fillColor.Value);
                    }
                }

                excelRow++;
            }
        }

        private static System.Drawing.Color? GetSubsequentMonthColor(
            int month,
            decimal? curr,
            decimal? prev,
            decimal? currRate,
            decimal? prevRate)
        {
            if (!curr.HasValue)
            {
                return null;
            }

            if (curr.Value == 0m)
            {
                return prev.HasValue && prev.Value > 0m
                    ? FirstZeroColor
                    : ConsecutiveZeroColor;
            }

            if (curr.Value <= 0m)
            {
                return null;
            }

            if (!prev.HasValue || prev.Value <= 0m)
            {
                return GetMonthHighlightColor(month);
            }

            if (curr.Value <= 5000m && prev.Value > 5000m)
            {
                return GetMonthHighlightColor(month);
            }

            if (prev.Value <= 5000m && curr.Value > 5000m)
            {
                return RateChangeColor;
            }

            // Use base monthly rate rather than net salary so normal OT/holiday/tardy variance is not flagged.
            if (currRate.HasValue && prevRate.HasValue
                && currRate.Value > 0m && prevRate.Value > 0m
                && currRate.Value != prevRate.Value)
            {
                return RateChangeColor;
            }

            return null;
        }

        private static System.Drawing.Color GetMonthHighlightColor(int month) => month switch
        {
            1 => System.Drawing.Color.FromArgb(218, 227, 243),
            2 => System.Drawing.Color.FromArgb(255, 192, 0),
            3 => System.Drawing.Color.FromArgb(226, 239, 218),
            4 => System.Drawing.Color.FromArgb(255, 242, 204),
            5 => System.Drawing.Color.FromArgb(252, 228, 214),
            6 => System.Drawing.Color.FromArgb(255, 255, 0),
            7 => System.Drawing.Color.FromArgb(255, 242, 204),
            8 => System.Drawing.Color.FromArgb(221, 235, 247),
            9 => System.Drawing.Color.FromArgb(226, 239, 218),
            10 => System.Drawing.Color.FromArgb(218, 227, 243),
            11 => System.Drawing.Color.FromArgb(255, 242, 204),
            12 => System.Drawing.Color.FromArgb(221, 235, 247),
            _ => System.Drawing.Color.White
        };

        private static void SetCellFill(ExcelRange range, System.Drawing.Color color)
        {
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(color);

            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            range.Style.Border.Top.Color.SetColor(CellBorderColor);
            range.Style.Border.Bottom.Color.SetColor(CellBorderColor);
            range.Style.Border.Left.Color.SetColor(CellBorderColor);
            range.Style.Border.Right.Color.SetColor(CellBorderColor);
        }

        private static string GetMonthlySummarySqlScript()
        {
            return @"
                SELECT
                    e.Id AS EmployeeId,
                    ISNULL(d.Department, '') AS Department,
                    ISNULL(pos.Position, '') AS Position,
                    ISNULL(e.LastName, '') AS LastName,
                    ISNULL(e.FirstName, '') AS FirstName,
                    e.Allowance,
                    ISNULL(e.MiddleName, '') AS MiddleName,
                    tp.MonthId,
                    MAX(CASE WHEN tpl.TotalSalaryAmount > 0 OR tpl.NetIncome > 0
                             THEN ISNULL(NULLIF(tpl.PayrollRate * 2, 0), e.MonthlyRate)
                             ELSE 0 END) AS MonthlyRateAmount,
                    SUM(tpl.NetIncome) AS ActualNetSalaryAmount
                FROM TrnPayrollLine tpl
                INNER JOIN TrnPayroll tp ON tpl.PayrollId = tp.Id
                INNER JOIN MstEmployee e ON tpl.EmployeeId = e.Id
                LEFT JOIN MstDepartment d ON e.DepartmentId = d.Id
                LEFT JOIN MstPosition pos ON e.PositionId = pos.Id
                WHERE tp.IsLocked = 1
                  AND e.IsLocked = 1
                  AND (@ParamDateStart IS NULL OR tp.PayrollDate >= @ParamDateStart)
                  AND (@ParamDateEnd IS NULL OR tp.PayrollDate <= @ParamDateEnd)
                  AND tp.MonthId BETWEEN @ParamStartMonthId AND @ParamEndMonthId
                  AND (@ParamDepartmentId = 0 OR e.DepartmentId = @ParamDepartmentId)
                  AND (@ParamPositionId = 0 OR e.PositionId = @ParamPositionId)
                  AND (@ParamCompanyId = 0 OR e.CompanyId = @ParamCompanyId)
                  AND (@ParamBranchId = 0 OR e.BranchId = @ParamBranchId)
                  AND (@ParamPayrollGroupId = 0 OR tp.PayrollGroupId = @ParamPayrollGroupId)
                  AND (@ParamEmployeeId = 0 OR e.Id = @ParamEmployeeId)
                  AND ISNULL(CAST(e.EmploymentType AS NVARCHAR(50)), '1') LIKE ISNULL(@ParamEmploymentType, '%')
                GROUP BY
                    e.Id,
                    d.Department,
                    pos.Position,
                    e.LastName,
                    e.FirstName,
                    e.Allowance,
                    e.MiddleName,
                    tp.MonthId
                ORDER BY
                    e.LastName,
                    e.FirstName,
                    e.MiddleName,
                    tp.MonthId;";
        }
    }
}
