using Dapper;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.Mobile
{
    public class Login
    {
        public string? MobileCode { get; set; }

        public async Task<bool> ResultAsync()
        {
            var sql = "SELECT Id FROM dbo.MstEmployee WHERE dbo.Encode(dbo.MstEmployee.Id) = @MobileCode";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                var result = await connection.QueryAsync<LoginRecord>(sql, new { MobileCode });

                return result.Any();
            }
        }

        public class LoginRecord
        {
            public int Id { get; set; }
        }
    }
}