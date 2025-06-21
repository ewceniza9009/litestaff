using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;
using static whris.Application.Mobile.RepPayroll.Payslip;

namespace whris.Application.Mobile
{
    public class Login
    {
        public string? MobileCode { get; set; }

        public bool Result() 
        {
            var result = new List<Login>();
            var sql = $@"SELECT Id FROM dbo.MstEmployee WHERE dbo.Encode(dbo.MstEmployee.Id)={MobileCode}";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<Login>(sql)?.ToList();
            };

            return (result?.Count() ?? 0) > 0;
        }

        public class LoginRecord 
        {
            public int Id { get; set; }
        }
    }
}
