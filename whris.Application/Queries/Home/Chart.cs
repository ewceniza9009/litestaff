using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static whris.Application.Queries.TrnLoan.PaymentList;
using whris.Data.Data;
using Dapper;
using whris.Application.Dtos;

namespace whris.Application.Queries.Home
{
    public class Chart
    {
        public List<EmployeeTimeIn> Result()
        {
            var result = new List<EmployeeTimeIn>();
            string sql = $@"SELECT EmployeeTimeIn.Id, 
	                EmployeeTimeIn.EmployeeId, 
	                Format([EmployeeTimeIn].[TimeIn1],'hh:mm tt') AS DtrTimeIn, 
	                Format([ShiftCodeDayTimeIn].[TimeIn1],'hh:mm tt') AS ShiftTimeIn
                FROM 
                (
	                SELECT TrnDTRLine.Id, TrnDTRLine.EmployeeId, TrnDTRLine.ShiftCodeId, Format([Date],'dddd') AS [Day], TrnDTRLine.TimeIn1
	                FROM TrnDTRLine
	                WHERE (((TrnDTRLine.TimeIn1) Is Not Null)) AND MONTH(Date) = MONTH(GETDATE())
                ) EmployeeTimeIn 
	                LEFT JOIN 
                (
	                SELECT MstShiftCodeDay.ShiftCodeId, MstShiftCodeDay.Day, MstShiftCodeDay.TimeIn1
	                FROM MstShiftCodeDay
                )ShiftCodeDayTimeIn ON (EmployeeTimeIn.Day = ShiftCodeDayTimeIn.Day) AND (EmployeeTimeIn.ShiftCodeId = ShiftCodeDayTimeIn.ShiftCodeId);";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<EmployeeTimeIn>(sql).ToList();
            };

            return result;
        }

        public class EmployeeTimeIn 
        {
            public int Id { get; set; }
            public int EmployeeId { get; set; }
            public DateTime DtrTimeIn { get; set; }
            public DateTime ShiftTimeIn { get; set; }
        }
    }
}
