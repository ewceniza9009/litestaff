using Dapper;
using Kendo.Mvc.UI;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using whris.Data.Data;

namespace whris.Application.Library
{
    public class MobileUtils
    {
        public static int GetEmployeeByMobileCode(string Code)
        {
            int output = 0;
            bool result = int.TryParse(Code, out output);

            if (!result) 
            {
                return 0;
            }

            var employeeId = 0;
            var employeeSql = $@"SELECT Id FROM dbo.MstEmployee WHERE dbo.Encode(Id)={Code}";
            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                employeeId = connection.QueryFirstOrDefault<int>(employeeSql);
            }

            return employeeId;
        }

        public static int GetPayrollGroupId(string Code)
        {
            var groupId = 0;
            var groupSql = $@"SELECT PayrollGroupId FROM dbo.MstEmployee WHERE dbo.Encode(Id)={Code}";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                groupId = connection.QueryFirstOrDefault<int>(groupSql);
            };

            return groupId;
        }

        public static async Task<int> GetPayrollGroupIdAsync(string Code)
        {
            var sql = "SELECT PayrollGroupId FROM dbo.MstEmployee WHERE dbo.Encode(Id) = @Code";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(sql, new { Code });
            }
        }

        public static string Encode(string input)
        {
            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                var result = connection.QueryFirstOrDefault<Mobile>($@"SELECT dbo.Encode({input}) AS MobileCode");

                return result?.MobileCode ?? string.Empty;
            }
        }

        public class Mobile 
        {
            public string? MobileCode { get; set; }
        }
    }
}
