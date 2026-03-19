using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.Mobile.RepPayroll
{
    public class Memo
    {
        public string? MobileCode { get; set; }

        public async Task<IEnumerable<MemoRecord>> ResultAsync()
        {
            var sql = @"
                DECLARE @ResolvedEmployeeId INT;
                
                SELECT TOP 1 @ResolvedEmployeeId = Id 
                FROM MstEmployee 
                WHERE dbo.Encode(Id) = @MobileCode;

                SELECT Id, 
                       EmployeeId, 
                       FORMAT(MemoDate, 'MM/dd/yyyy') AS MemoDate, 
                       MemoSubject, 
                       MemoContent, 
                       PreparedBy, 
                       ApprovedBy, 
                       FilePath
                FROM MstEmployeeMemo
                WHERE EmployeeId = @ResolvedEmployeeId
                ORDER BY MemoDate DESC";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                return await connection.QueryAsync<MemoRecord>(
                    sql,
                    new { MobileCode },
                    commandTimeout: 120
                );
            }
        }

        public class MemoRecord
        {
            public int Id { get; set; }
            public int EmployeeId { get; set; }
            public string? MemoDate { get; set; }
            public string? MemoSubject { get; set; }
            public string? MemoContent { get; set; }
            public int PreparedBy { get; set; }
            public int ApprovedBy { get; set; }
            public string? FilePath { get; set; }
        }
    }
}
