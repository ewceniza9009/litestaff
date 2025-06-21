using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;
using Dapper;

namespace whris.Application.Queries.TrnDtr
{
    public class Logs
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? EmployeeId { get; set; }

        public List<Record> Result()
        {
            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                // Set up the parameters for the query
                var parameters = new DynamicParameters();
                parameters.Add("@StartDate", StartDate.Date);
                parameters.Add("@EndDatePlusOne", EndDate.Date.AddDays(1));

                // Start with the base query using range comparison on LogDateTime
                string sql = @"
                    SELECT t.Id,
                           t.BiometricIdNumber, 
                           e.FullName AS EmployeeName, 
                           t.LogDateTime, 
                           t.LogType,
                           d.Department
                    FROM TrnLogs t
                    INNER JOIN MstEmployee e ON t.BiometricIdNumber = e.BiometricIdNumber
                    INNER JOIN MstDepartment d ON e.DepartmentId = d.Id
                    WHERE t.LogDateTime >= @StartDate AND t.LogDateTime < @EndDatePlusOne
                ";

                // If an EmployeeId is provided, get the biometric ID and add it as a parameter
                if (EmployeeId != 0)
                {
                    sql += " AND e.Id = @EmployeeId";
                    parameters.Add("@EmployeeId", EmployeeId);
                }

                // Append ordering
                sql += " ORDER BY e.FullName ASC";

                // Execute the query with Dapper and return the results
                return connection.Query<Record>(sql, parameters).ToList();
            }
        }

        public class Record
        {
            public int Id { get; set; }
            public string? BiometricIdNumber { get; set; }
            public string? EmployeeName { get; set; }
            public DateTime LogDateTime { get; set; }
            public string? LogType { get; set; }
            public string? LogTypeString 
            {
                get 
                {
                    return LogType switch
                    {
                        "I" => "Time In",
                        "0" => "Break-Out",
                        "1" => "Break-In",
                        "O" => "Time Out",
                        "X" => "Deleted",
                        _ => string.Empty
                    };
                }
            }
            public string? Department { get; set; }
        }
    }
}
