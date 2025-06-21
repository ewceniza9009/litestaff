using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Data;
using System.Data.OleDb;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Library;

namespace whris.UI.Services
{
    public class FileUtil
    {
        public static DataTable ConvertToDataTable(string filePath)
        {
            DataTable dataTable = new DataTable();
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string[] headers = streamReader.ReadLine().Split('\t');
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                while (!streamReader.EndOfStream)
                {
                    string[] rows = streamReader.ReadLine().Split('\t');
                    if (rows.Length > 1)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dataRow[i] = rows[i].Trim();
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }

            return dataTable;
        }

        public static DataTable ConvertDatToDataTable(string filePath)
        {
            DataTable dataTable = new DataTable();
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string[] headers = streamReader.ReadLine().Split('\t');
                DataRow dataRow = dataTable.NewRow();
                int headerCol = 0;

                foreach (string header in headers)
                {
                    dataTable.Columns.Add("H" + headerCol);
                    headerCol++;
                }

                headerCol = 0;
                var firstRow = dataTable.NewRow();

                foreach (string header in headers) 
                {
                    firstRow[headerCol] = header;
                    headerCol++;
                }

                dataTable.Rows.Add(firstRow);

                while (!streamReader.EndOfStream)
                {
                    dataRow = dataTable.NewRow();
                    string[] rows = streamReader.ReadLine().Split('\t');
                    if (rows.Length > 1)
                    {
                       
                        for (int i = 0; i < headers.Length; i++)
                        {
                            try
                            {
                                dataRow[i] = rows[i].Trim();
                            }
                            catch 
                            {
                                continue;
                            }
                            
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }

            return dataTable;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public static DataTable? ConvertExcelToDataTablev1(string filePath)
        {
            var connString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=""Excel 12.0;HDR=No;IMEX=1""";

            var conn = new OleDbConnection(connString);
            var dataTable = new DataTable();
            var dataSet = new DataSet();

            try
            {
                conn.Open();

                using (var sheet = conn?.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null))
                {
                    if (sheet is not null)
                    {
                        for (int i = 0; i < sheet.Rows.Count; i++)
                        {
                            var worksheets = sheet.Rows[i]["TABLE_NAME"].ToString();
                            var command = new OleDbCommand(string.Format("SELECT * FROM [{0}]", worksheets), conn);
                            var adapter = new OleDbDataAdapter();

                            adapter.SelectCommand = command;

                            adapter.Fill(dataSet);
                        }

                        dataTable = dataSet.Tables[0];
                    }
                }

            }
            catch (Exception? ex)
            {
                Console.WriteLine(ex.ToString());

                return null;
            }
            finally
            {

                conn?.Close();
            }

            return dataTable;

        }

        public static DataTable ConvertExcelToDataTable(string filePath)
        {
            var dataTable = new DataTable();

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets[0]; // Get the first worksheet

                    if (worksheet == null || worksheet.Dimension == null)
                    {
                        Console.WriteLine("The worksheet is empty or does not exist.");
                        return dataTable;
                    }

                    // Read the first row as headers
                    var headerRow = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];
                    foreach (var headerCell in headerRow)
                    {
                        var columnName = headerCell.Text.Trim(); // Trim any extra spaces
                        if (!string.IsNullOrEmpty(columnName))
                        {
                            dataTable.Columns.Add(columnName); // Add each header as a DataTable column
                        }
                    }

                    // Log column names to verify them
                    Console.WriteLine("Column Names:");
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        Console.WriteLine(col.ColumnName);
                    }

                    // Read the remaining rows
                    for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++) // Start from row 2 to skip headers
                    {
                        var row = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                        var newRow = dataTable.NewRow();

                        for (int colNum = 1; colNum <= worksheet.Dimension.End.Column; colNum++)
                        {
                            newRow[colNum - 1] = row[rowNum, colNum].Text; // Populate DataRow with cell values
                        }

                        dataTable.Rows.Add(newRow); // Add the new row to the DataTable
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null; // Return null in case of error
            }

            return dataTable;
        }

        public static List<TmpDtrLogs> ProcessLogs(int? departmentId, int? employeeId, DateTime startDate, DateTime endDate, string filePath, string? extension)
        {
            List<TmpDtrLogs> tmplLogs;
            var dataTable = new DataTable();
            var extentionType = "Type1";

            if (extension == ".csv" || extension == ".txt")
            {
                dataTable = FileUtil.ConvertToDataTable(filePath);
            }

            if (extension == ".xls" || extension == ".xlsx")
            {
                extentionType = "Type2";

                dataTable = FileUtil.ConvertExcelToDataTable(filePath);
            }

            if (extension == ".dat")
            {
                dataTable = FileUtil.ConvertDatToDataTable(filePath);
            }

            tmplLogs = GetLogs(extentionType, dataTable ?? new DataTable(), departmentId, employeeId, startDate, endDate);
            return tmplLogs;
        }

        private static List<TmpDtrLogs> GetLogs(string extensionType, DataTable dataTable, int? departmentId = null, int? employeeId = null, DateTime? dateStart = null, DateTime? dateEnd = null)
        {
            var logs = new List<TmpDtrLogs>();
            var employees = DTR.GetEmployees();
            var shiftCodeDays = DTR.GetShiftCodeDays();

            //if (extensionType == "Type2")
            //{
            //    var columnName = dataTable.Rows[0].ItemArray;

            //    for (int i = 0; i < dataTable.Columns.Count; i++) 
            //    {
            //        dataTable.Columns[i].ColumnName = columnName[i]?.ToString();
            //    }

            //    dataTable.Rows.Remove(dataTable.Rows[0]);
            //}

            var empIds = new List<int>();

            empIds = DTR.GetEmployeeIds(departmentId);

            if (employeeId is not null) 
            {
                empIds = new List<int> { employeeId ?? 0 };
            }

            foreach (var empId in empIds) 
            {
                var companyId = Lookup.GetCompanyIdByEmployeeId(empId);

                var noField = Lookup.GetDTRNoFieldByCompanyId(companyId);
                var dateTimeField = Lookup.GetDTRDateTimeFieldByCompanyId(companyId);
                var logTypeField = Lookup.GetDTRLogTypeFieldByCompanyId(companyId);

                var bioId = Lookup.GetBioIdByEmployeeId(empId);
                var isDataTableHasRows = dataTable.AsEnumerable()
                  .Where(row => row.Field<string>(noField) == bioId)
                  .Any();

                if (isDataTableHasRows) 
                {
                    var table = dataTable.AsEnumerable()
                        .Where(row => row.Field<string>(noField) == bioId);

                    var empFirsLogDateTime = table.AsEnumerable()
                        .OrderBy(x => DateTime.Parse(x[dateTimeField]?.ToString() ?? new DateTime(1990, 09, 15).ToString()))
                        .Select(x => DateTime.Parse(x[dateTimeField]?.ToString() ?? new DateTime(1990, 09, 15).ToString()))
                        .FirstOrDefault();

                    var dateOnlyStart = DateOnly.FromDateTime(dateStart ?? new DateTime(199, 09, 15));
                    var dateOnlyEnd = DateOnly.FromDateTime(dateEnd ?? new DateTime(199, 09, 15));

                    //var shiftCodeId = DTR.QuickChangeShift(empFirsLogDateTime, empId, empFirsLogDateTime, 0);

                    //if (shiftCodeId == 0)
                    //{
                    //    shiftCodeId = DTR.GetShiftCodeId(empId, employees);
                    //}

                    //var shiftCodeDay = DTR.GetShiftCodeDay(shiftCodeId, empFirsLogDateTime.ToString("dddd"), shiftCodeDays);

                    //var shiftTimeIn1 = TimeOnly.FromDateTime(shiftCodeDay.TimeIn1 ?? default);
                    //var shiftTimeOut2 = TimeOnly.FromDateTime(shiftCodeDay.TimeOut2 ?? default);

                    //if (shiftTimeIn1 > shiftTimeOut2)
                    //{
                    //    dateOnlyEnd = dateOnlyEnd.AddDays(1);
                    //}

                    try
                    {
                        var table1 = table
                        .Where(row => DateOnly.FromDateTime(DateTime.Parse(row.Field<string>(dateTimeField) ?? new DateTime(1990, 09, 15).ToString())) >= dateOnlyStart &&
                            DateOnly.FromDateTime(DateTime.Parse(row.Field<string>(dateTimeField) ?? new DateTime(1990, 09, 15).ToString())) <= dateOnlyEnd)
                        .CopyToDataTable();

                        foreach (DataRow row in table1.Rows)
                        {
                            logs.Add(new TmpDtrLogs()
                            {
                                EmployeeId = Lookup.GetEmployeeIdByBioId(row[noField]?.ToString() ?? ""),
                                Date = DateOnly.FromDateTime(DateTime.Parse(row[dateTimeField]?.ToString() ?? new DateTime(1990, 09, 15).ToString())),
                                Time = TimeOnly.FromDateTime(DateTime.Parse(row[dateTimeField]?.ToString() ?? new DateTime(1990, 09, 15).ToString())),
                                LogType = row[logTypeField]?.ToString()
                            });
                        }
                    }
                    catch 
                    {
                        continue;
                    }                    
                }
            }

            employees = null;
            shiftCodeDays = null;

            return logs
                ?.OrderBy(x => x.EmployeeId)
                ?.ThenBy(x => DateTime.Parse($"{x.Date} {x.Time}"))
                ?.ToList() ?? new List<TmpDtrLogs>();
        }

        public static List<TmpImportOvertime> ProcessOvertimeImports(string filePath, string? extension)
        {
            List<TmpImportOvertime> tmpOvertimeImports;
            var dataTable = new DataTable();
            var extentionType = "Type1";

            if (extension == ".csv" || extension == ".txt")
            {
                dataTable = ConvertToDataTable(filePath);
            }

            if (extension == ".xls" || extension == ".xlsx")
            {
                extentionType = "Type2";

                dataTable = ConvertExcelToDataTable(filePath);
            }

            tmpOvertimeImports = GetOvertimeImports(extentionType, dataTable ?? new DataTable());
            return tmpOvertimeImports;
        }

        private static List<TmpImportOvertime> GetOvertimeImports(string extensionType, DataTable dataTable)
        {
            var overtimeImports = new List<TmpImportOvertime>();

            //if (extensionType == "Type2")
            //{
            //    var columnName = dataTable.Rows[0].ItemArray;

            //    for (int i = 0; i < dataTable.Columns.Count; i++)
            //    {
            //        dataTable.Columns[i].ColumnName = columnName[i]?.ToString();
            //    }

            //    dataTable.Rows.Remove(dataTable.Rows[0]);
            //}

            var isDataTableHasRows = dataTable.AsEnumerable()
                .Any();


            if (isDataTableHasRows) 
            {
                var table = dataTable.AsEnumerable()
                        .CopyToDataTable();

                foreach (DataRow row in table.Rows) 
                {
                    var bioId = Lookup.GetEmployeeIdByBioId(row["BiometricId"]?.ToString() ?? "");

                    if (bioId > 0) 
                    {
                        overtimeImports.Add(new TmpImportOvertime
                        {
                            EmployeeId = Lookup.GetEmployeeIdByBioId(row["BiometricId"]?.ToString() ?? ""),
                            Date = DateTime.Parse(row["Date"]?.ToString() ?? new DateTime(1990, 09, 15).ToString()),
                            OvertimeHours = decimal.Parse(row["OT"]?.ToString() ?? "0"),
                            OvertimeNightHours = decimal.Parse(row["NightOT"]?.ToString() ?? "0"),
                            OvertimeLimitHours = decimal.Parse(row["LimitOT"]?.ToString() ?? "0"),
                            Remarks = row["Remarks"]?.ToString() ?? "NA"
                        });
                    }                    
                }
            }

            return overtimeImports;
        }

        public static List<TmpImportLeave> ProcessLeaveImports(string filePath, string? extension)
        {
            List<TmpImportLeave> tmpLeaveImports;
            var dataTable = new DataTable();
            var extentionType = "Type1";

            if (extension == ".csv" || extension == ".txt")
            {
                dataTable = ConvertToDataTable(filePath);
            }

            if (extension == ".xls" || extension == ".xlsx")
            {
                extentionType = "Type2";

                dataTable = ConvertExcelToDataTable(filePath);
            }

            tmpLeaveImports = GetLeaveImports(extentionType, dataTable ?? new DataTable());
            return tmpLeaveImports;
        }

        private static List<TmpImportLeave> GetLeaveImports(string extensionType, DataTable dataTable)
        {
            var LeaveImports = new List<TmpImportLeave>();

            //if (extensionType == "Type2")
            //{
            //    var columnName = dataTable.Rows[0].ItemArray;

            //    for (int i = 0; i < dataTable.Columns.Count; i++)
            //    {
            //        dataTable.Columns[i].ColumnName = columnName[i]?.ToString();
            //    }

            //    dataTable.Rows.Remove(dataTable.Rows[0]);
            //}

            var isDataTableHasRows = dataTable.AsEnumerable()
                .Any();


            if (isDataTableHasRows)
            {
                var table = dataTable.AsEnumerable()
                        .CopyToDataTable();

                foreach (DataRow row in table.Rows)
                {
                    var withPay = int.Parse(row["WithPay"]?.ToString() ?? "0");
                    var debitToLedger = int.Parse(row["DebitToLedger"]?.ToString() ?? "0");

                    LeaveImports.Add(new TmpImportLeave
                    {
                        EmployeeId = Lookup.GetEmployeeIdByBioId(row["BiometricId"]?.ToString() ?? ""),
                        LeaveId = Lookup.GetLeaveIdByDescription(row["Leave"]?.ToString() ?? ""),
                        Date = DateTime.Parse(row["Date"]?.ToString() ?? new DateTime(1990, 09, 15).ToString()),
                        NumberOfHours = decimal.Parse(row["NoOfHours"]?.ToString() ?? "0"),
                        WithPay = withPay == 1 ? true : false,
                        DebitToLedger = debitToLedger == 1 ? true : false,
                        Remarks = row["Remarks"]?.ToString() ?? "NA"
                    });
                }
            }

            return LeaveImports;
        }

        public static List<TmpImportOtherIncome> ProcessOtherIncomeImports(string filePath, string? extension)
        {
            List<TmpImportOtherIncome> tmpOtherIncomeImports;
            var dataTable = new DataTable();
            var extentionType = "Type1";

            if (extension == ".csv" || extension == ".txt")
            {
                dataTable = ConvertToDataTable(filePath);
            }

            if (extension == ".xls" || extension == ".xlsx")
            {
                extentionType = "Type2";

                dataTable = ConvertExcelToDataTable(filePath);
            }

            tmpOtherIncomeImports = GetOtherIncomeImports(extentionType, dataTable ?? new DataTable());
            return tmpOtherIncomeImports;
        }

        private static List<TmpImportOtherIncome> GetOtherIncomeImports(string extensionType, DataTable dataTable)
        {
            var OtherIncomeImports = new List<TmpImportOtherIncome>();

            //if (extensionType == "Type2")
            //{
            //    var columnName = dataTable.Rows[0].ItemArray;

            //    for (int i = 0; i < dataTable.Columns.Count; i++)
            //    {
            //        dataTable.Columns[i].ColumnName = columnName[i]?.ToString();
            //    }

            //    dataTable.Rows.Remove(dataTable.Rows[0]);
            //}

            var isDataTableHasRows = dataTable.AsEnumerable()
                .Any();

            if (isDataTableHasRows)
            {
                var table = dataTable.AsEnumerable()
                    .Where(row => !string.IsNullOrEmpty(row?.Field<string>("Amount")) &&
                         decimal.Parse(row?.Field<string>("Amount") ?? "0") != 0)
                    .CopyToDataTable();

                foreach (DataRow row in table.Rows)
                {
                    var bioId = Lookup.GetEmployeeIdByBioId(row["BiometricId"]?.ToString() ?? "");

                    if (bioId > 0)
                    {
                        OtherIncomeImports.Add(new TmpImportOtherIncome
                        {
                            EmployeeId = Lookup.GetEmployeeIdByBioId(row["BiometricId"]?.ToString() ?? ""),
                            OtherIncomeId = Lookup.GetOtherIncomeIdByDescription(row["OtherIncome"]?.ToString() ?? ""),
                            Amount = decimal.Parse(row["Amount"]?.ToString() ?? "0")
                        });
                    }
                }
            }

            return OtherIncomeImports;
        }

        public static List<TmpImportOtherDeduction> ProcessOtherDeductionImports(string filePath, string? extension)
        {
            List<TmpImportOtherDeduction> tmpOtherDeductionImports;
            var dataTable = new DataTable();
            var extentionType = "Type1";

            if (extension == ".csv" || extension == ".txt")
            {
                dataTable = ConvertToDataTable(filePath);
            }

            if (extension == ".xls" || extension == ".xlsx")
            {
                extentionType = "Type2";

                dataTable = ConvertExcelToDataTable(filePath);
            }

            tmpOtherDeductionImports = GetOtherDeductionImports(extentionType, dataTable ?? new DataTable());
            return tmpOtherDeductionImports;
        }

        private static List<TmpImportOtherDeduction> GetOtherDeductionImports(string extensionType, DataTable dataTable)
        {
            var OtherDeductionImports = new List<TmpImportOtherDeduction>();

            //if (extensionType == "Type2")
            //{
            //    var columnName = dataTable.Rows[0].ItemArray;

            //    for (int i = 0; i < dataTable.Columns.Count; i++)
            //    {
            //        dataTable.Columns[i].ColumnName = columnName[i]?.ToString();
            //    }

            //    dataTable.Rows.Remove(dataTable.Rows[0]);
            //}

            List<string> bioIdsNotFoundInEmployeeSetup = new();
            var isDataTableHasRows = dataTable.AsEnumerable()
                .Any();

            if (isDataTableHasRows)
            {
                var table = dataTable.AsEnumerable()
                    .Where(row => !string.IsNullOrEmpty(row?.Field<string>("Amount")) && 
                        decimal.Parse(row?.Field<string>("Amount") ?? "0") != 0)
                    .CopyToDataTable();

                foreach (DataRow row in table.Rows)
                {
                    var bioId = Lookup.GetEmployeeIdByBioId(row["BiometricId"]?.ToString() ?? "");

                    if (bioId > 0)
                    {
                        OtherDeductionImports.Add(new TmpImportOtherDeduction
                        {
                            EmployeeId = Lookup.GetEmployeeIdByBioId(row["BiometricId"]?.ToString() ?? ""),
                            OtherDeductionId = Lookup.GetOtherDeductionIdByDescription(row["OtherDeduction"]?.ToString() ?? ""),
                            Amount = decimal.Parse(row["Amount"]?.ToString() ?? "0")
                        });
                    }
                    else 
                    {
                        bioIdsNotFoundInEmployeeSetup.Add(row["BiometricId"]?.ToString() ?? "");
                    }
                }

                if (bioIdsNotFoundInEmployeeSetup != null && bioIdsNotFoundInEmployeeSetup.Count > 0)
                {
                    var ctr = 0;
                    Console.WriteLine("BiometricId's not found on the 'Employee Setup': \n");

                    foreach (var item in bioIdsNotFoundInEmployeeSetup) 
                    {
                        ctr++;
                        Console.WriteLine($"{ctr}.) {item}");
                    }
                }
            }

            return OtherDeductionImports;
        }

        public static List<TmpUploadEmployee> ProcessEmployeUploads(string filePath, string? extension)
        {
            List<TmpUploadEmployee> tmpEmployeeUploads;
            var dataTable = new DataTable();
            var extentionType = "Type1";

            if (extension == ".csv" || extension == ".txt")
            {
                dataTable = ConvertToDataTable(filePath);
            }

            if (extension == ".xls" || extension == ".xlsx")
            {
                extentionType = "Type2";

                dataTable = ConvertExcelToDataTable(filePath);
            }

            tmpEmployeeUploads = GetEmployeUploads(extentionType, dataTable ?? new DataTable());
            return tmpEmployeeUploads;
        }

        private static List<TmpUploadEmployee> GetEmployeUploads(string extensionType, DataTable dataTable)
        {
            var EmployeeUploads = new List<TmpUploadEmployee>();

            if (extensionType == "Type2")
            {
                var columnName = dataTable.Rows[0].ItemArray;

                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    dataTable.Columns[i].ColumnName = columnName[i]?.ToString();
                }

                dataTable.Rows.Remove(dataTable.Rows[0]);
            }

            var isDataTableHasRows = dataTable.AsEnumerable()
                .Any();


            if (isDataTableHasRows)
            {
                var table = dataTable.AsEnumerable()
                        .CopyToDataTable();

                foreach (DataRow row in table.Rows)
                {
                    EmployeeUploads.Add(new TmpUploadEmployee
                    {
                        BiometricIdNumber = row["BiometricId"]?.ToString() ?? "",
                        FirstName = row["FirstName"]?.ToString() ?? "",
                        MiddleName = row["MiddleName"]?.ToString() ?? "",
                        LastName = row["LastName"]?.ToString() ?? "",
                        AdditionalName = row["AdditionalName"]?.ToString() ?? "",
                        DailyRate = decimal.Parse(row["DailyRate"]?.ToString() ?? "0")
                    });
                }
            }

            return EmployeeUploads;
        }
    }
}
