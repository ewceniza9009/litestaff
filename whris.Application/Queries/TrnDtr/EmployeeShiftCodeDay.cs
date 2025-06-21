using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.Queries.TrnDtr
{
    public class EmployeeShiftCodeDay
    {
        public int ParamEmployeeId { get; set; }
        public string? ParamDay { get; set; }
        public DateTime ParamLogTimeIn1 { get; set; }

        public IEnumerable<Record> Result(string? logType = "I")
        {
            var result = new List<Record>();
            string sql = $@"SELECT MstEmployeeShiftCode.EmployeeId, 
	                MstShiftCodeDay.ShiftCodeId, 
	                MstShiftCodeDay.Day, 
	                MstShiftCodeDay.TimeIn1,
	                MstShiftCodeDay.TimeOut2
                FROM (MstShiftCodeDay LEFT JOIN MstShiftCode ON MstShiftCodeDay.ShiftCodeId = MstShiftCode.Id) 
	                LEFT JOIN MstEmployeeShiftCode ON MstShiftCode.Id = MstEmployeeShiftCode.ShiftCodeId
                WHERE MstEmployeeShiftCode.EmployeeId={ParamEmployeeId} AND MstShiftCodeDay.Day='{ParamDay}';
                ";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<Record>(sql).ToList();

                foreach (var item in result) 
                {
                    item.LogTimeIn1 = ParamLogTimeIn1;
                    item.TimeIn1 = DateTime.Parse($"{ParamLogTimeIn1.Date:d} {string.Format("{0:hh:mm tt}", item.TimeIn1)}");
                    item.TimeOut2 = DateTime.Parse($"{ParamLogTimeIn1.Date:d} {string.Format("{0:hh:mm tt}", item.TimeOut2)}");

                    var discrepancy = (item.TimeIn1 - item.LogTimeIn1).TotalHours;

                    if (discrepancy < -20) 
                    {
                        item.TimeIn1 = item.TimeIn1.AddDays(1);
                    }

                    if (discrepancy > 20) 
                    {
                        item.TimeIn1 = item.TimeIn1.AddDays(-1);
                    }

                    item.Interval = Math.Abs((item.TimeIn1 - item.LogTimeIn1).TotalHours);

                    if (logType == "O") 
                    {
                        discrepancy = (item.TimeOut2 - item.LogTimeIn1).TotalHours;

                        if (discrepancy < -20)
                        {
                            item.TimeOut2 = item.TimeOut2.AddDays(1);
                        }

                        if (discrepancy > 20)
                        {
                            item.TimeOut2 = item.TimeOut2.AddDays(-1);
                        }

                        item.Interval = Math.Abs((item.TimeOut2 - item.LogTimeIn1).TotalHours);
                    }
                }
            };

            return result.ToArray();
        }

        public class Record 
        {
            public int EmployeeId { get; set; }
            public int ShiftCodeId { get; set; }
            public string? Day { get; set; }
            public DateTime TimeIn1 { get; set; }
            public DateTime TimeOut2 { get; set; }
            public DateTime LogTimeIn1 { get; set;}
            public double Interval { get; set; }
        }
    }
}
