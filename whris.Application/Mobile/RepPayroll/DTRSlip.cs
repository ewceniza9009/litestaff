using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.Mobile.RepPayroll
{
    public class DTRSlip
    {
        public int DTRId { get; set; }
        public string? MobileCode { get; set; }

        public List<DTRSlipRecord> Result()
        {
            int groupId = MobileUtils.GetPayrollGroupId(MobileCode ?? "NA");

            var result = new List<DTRSlipRecord>();
            var sql = $@"SELECT TrnDTRLine.DTRId, 
	            TrnDTR.DTRNumber, 
	            FORMAT(TrnDTR.DateStart, 'MM/dd/yyyy') AS DateStart, 
	            FORMAT(TrnDTR.DateEnd, 'MM/dd/yyyy') AS DateEnd, 
	            TrnDTR.Remarks, 
	            MstDepartment.Department, 
	            MstPosition.Position, 
	            TrnDTRLine.EmployeeId, 
	            MstEmployee.FullName, 
	            FORMAT(TrnDTRLine.Date, 'MM/dd/yyyy') AS Date, 
	            FORMAT(TrnDTRLine.TimeIn1, 'hh:mm tt') AS TimeIn1, 
	            FORMAT(TrnDTRLine.TimeOut1, 'hh:mm tt') AS TimeOut1, 
	            FORMAT(TrnDTRLine.TimeIn2, 'hh:mm tt') AS TimeIn2, 
	            FORMAT(TrnDTRLine.TimeOut2, 'hh:mm tt') AS TimeOut2, 
	            TrnDTRLine.OfficialBusiness, 
	            TrnDTRLine.OnLeave, 
	            TrnDTRLine.Absent, 
	            TrnDTRLine.RegularHours, 
	            TrnDTRLine.NightHours, 
	            TrnDTRLine.OvertimeHours, 
	            TrnDTRLine.OvertimeNightHours, 
	            TrnDTRLine.GrossTotalHours, 
	            TrnDTRLine.TardyLateHours, 
	            TrnDTRLine.TardyUndertimeHours, 
	            TrnDTRLine.NetTotalHours, 
	            TrnDTRLine.DayTypeId, 
	            MstDayType.DayType, 
	            TrnDTRLine.RestDay
            FROM ((((TrnDTRLine INNER JOIN MstEmployee ON TrnDTRLine.EmployeeId = MstEmployee.Id) 
	            INNER JOIN TrnDTR ON TrnDTRLine.DTRId = TrnDTR.Id) 
	            INNER JOIN MstDayType ON TrnDTRLine.DayTypeId = MstDayType.Id) 
	            INNER JOIN MstPosition ON MstEmployee.PositionId = MstPosition.Id) 
	            INNER JOIN MstDepartment ON MstEmployee.DepartmentId = MstDepartment.Id
            WHERE(TrnDTR.Id = {DTRId}) AND dbo.Encode(TrnDTRLine.EmployeeId)={MobileCode}
            ";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<DTRSlipRecord>(sql).ToList();
            };

            return result;
        }

        public async Task<IEnumerable<DTRSlipRecord>> ResultAsync()
        {
            var sql = @"SELECT TrnDTRLine.DTRId, 
                    TrnDTR.DTRNumber, 
                    FORMAT(TrnDTR.DateStart, 'MM/dd/yyyy') AS DateStart, 
                    FORMAT(TrnDTR.DateEnd, 'MM/dd/yyyy') AS DateEnd, 
                    TrnDTR.Remarks, 
                    MstDepartment.Department, 
                    MstPosition.Position, 
                    TrnDTRLine.EmployeeId, 
                    MstEmployee.FullName, 
                    FORMAT(TrnDTRLine.Date, 'MM/dd/yyyy') AS Date, 
                    FORMAT(TrnDTRLine.TimeIn1, 'hh:mm tt') AS TimeIn1, 
                    FORMAT(TrnDTRLine.TimeOut1, 'hh:mm tt') AS TimeOut1, 
                    FORMAT(TrnDTRLine.TimeIn2, 'hh:mm tt') AS TimeIn2, 
                    FORMAT(TrnDTRLine.TimeOut2, 'hh:mm tt') AS TimeOut2, 
                    TrnDTRLine.OfficialBusiness, 
                    TrnDTRLine.OnLeave, 
                    TrnDTRLine.Absent, 
                    TrnDTRLine.RegularHours, 
                    TrnDTRLine.NightHours, 
                    TrnDTRLine.OvertimeHours, 
                    TrnDTRLine.OvertimeNightHours, 
                    TrnDTRLine.GrossTotalHours, 
                    TrnDTRLine.TardyLateHours, 
                    TrnDTRLine.TardyUndertimeHours, 
                    TrnDTRLine.NetTotalHours, 
                    TrnDTRLine.DayTypeId, 
                    MstDayType.DayType, 
                    TrnDTRLine.RestDay
            FROM ((((TrnDTRLine INNER JOIN MstEmployee ON TrnDTRLine.EmployeeId = MstEmployee.Id) 
                INNER JOIN TrnDTR ON TrnDTRLine.DTRId = TrnDTR.Id) 
                INNER JOIN MstDayType ON TrnDTRLine.DayTypeId = MstDayType.Id) 
                INNER JOIN MstPosition ON MstEmployee.PositionId = MstPosition.Id) 
                INNER JOIN MstDepartment ON MstEmployee.DepartmentId = MstDepartment.Id
            WHERE (TrnDTR.Id = @DTRId) AND dbo.Encode(TrnDTRLine.EmployeeId) = @MobileCode";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                return await connection.QueryAsync<DTRSlipRecord>(sql, new { DTRId, MobileCode });
            }
        }

        public class DTRSlipRecord
        {
            public int DTRId { get; set; }
            public int PayrollOtherDeductionId { get; set; }
            public string? DTRNumber { get; set; }
            public string? DateStart { get; set; }
            public string? DateEnd { get; set; }
            public string? Remarks { get; set; }
            public string? Department { get; set; }
            public string? Position { get; set; }
            public int EmployeeId { get; set; }
            public string? FullName { get; set; }
            public string? Date { get; set; }
            public string? TimeIn1 { get; set; }
            public string? TimeOut1 { get; set; }
            public string? TimeIn2 { get; set; }
            public string? TimeOut2 { get; set; }
            public bool OfficialBusiness { get; set; }
            public bool OnLeave { get; set; }
            public bool Absent { get; set; }
            public decimal RegularHours { get; set; }
            public decimal NightHours { get; set; }
            public decimal OvertimeHours { get; set; }
            public decimal OvertimeNightHours { get; set; }
            public decimal GrossTotalHours { get; set; }
            public decimal TardyLateHours { get; set; }
            public decimal TardyUndertimeHours { get; set; }
            public decimal NetTotalHours { get; set; }
            public int DayTypeId { get; set; }
            public string? DayType { get; set; }
            public bool RestDay { get; set; }
        }
    }
}
