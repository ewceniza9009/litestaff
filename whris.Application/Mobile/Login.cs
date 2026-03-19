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
            var sql = "SELECT TOP 1 1 FROM dbo.MstEmployee WHERE dbo.Encode(Id) = @MobileCode";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { MobileCode });

                return result.HasValue;
            }
        }

        public class LoginRecord
        {
            public int Id { get; set; }
        }
    }
}