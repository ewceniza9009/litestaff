using Microsoft.Data.SqlClient;
using whris.Data.Data;
using Dapper;

namespace whris.Application.Queries.TrnPayrollOtherIncome
{
    internal class Main
    {
        public int? PayrollGroupId { get; set; }
        public int? EmployeeId { get; set; }
        public int?  StartPayNo{ get; set; }
        public int? EndPayNo { get; set; }
        public decimal? NoOfPayroll { get; set; }

        public List<Get13Month> Result()
        {
            var result = new List<Get13Month>();
            string sql = $@"SELECT TrnPayroll.IsLocked, 
	                TrnPayroll.PeriodId, 
	                MstEmployee.PayrollGroupId, 
	                TrnPayrollLine.EmployeeId, 
	                MstEmployee.PayrollTypeId, 
	                Sum(Round((([TotalRegularWorkingAmount]-[TotalTardyAmount]))/{NoOfPayroll},2)) AS VariableSalary, 
	                Sum(Round(([MstEmployee].[PayrollRate]-[TotalTardyAmount]-[TotalAbsentAmount])/{NoOfPayroll},2)) AS FixedSalary
                FROM (TrnPayrollLine INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id) 
	                INNER JOIN MstEmployee ON TrnPayrollLine.EmployeeId = MstEmployee.Id
                GROUP BY TrnPayroll.IsLocked, 
	                TrnPayroll.PeriodId, 
	                MstEmployee.PayrollGroupId, 
	                TrnPayrollLine.EmployeeId, 
	                MstEmployee.PayrollTypeId, 
	                IIf([TrnPayroll].[Id]>={StartPayNo} And [TrnPayroll].[Id]<={EndPayNo},1,0)
                HAVING TrnPayroll.IsLocked=1 AND TrnPayrollLine.EmployeeId={EmployeeId} AND MstEmployee.PayrollGroupId={PayrollGroupId} AND IIf([TrnPayroll].[Id]>={StartPayNo} And [TrnPayroll].[Id]<={EndPayNo},1,0)=1;";

            //*** For CTL ***
            //string sql = $@"SELECT TrnPayroll.IsLocked, 
	           //     TrnPayroll.PeriodId, 
	           //     MstEmployee.PayrollGroupId, 
	           //     TrnPayrollLine.EmployeeId, 
	           //     MstEmployee.PayrollTypeId, 
	           //     Sum(Round(((TotalRegularWorkingHours-TotalTardyLateHours-TotalTardyUndertimeHours)*HourlyRate)/12/2,2)) AS VariableSalary, 
	           //     Sum(Round(([MstEmployee].[PayrollRate]-[TotalTardyAmount]-[TotalAbsentAmount])/{NoOfPayroll},2)) AS FixedSalary
            //    FROM (TrnPayrollLine INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id) 
	           //     INNER JOIN MstEmployee ON TrnPayrollLine.EmployeeId = MstEmployee.Id
            //    GROUP BY TrnPayroll.IsLocked, 
	           //     TrnPayroll.PeriodId, 
	           //     MstEmployee.PayrollGroupId, 
	           //     TrnPayrollLine.EmployeeId, 
	           //     MstEmployee.PayrollTypeId, 
	           //     IIf([TrnPayroll].[Id]>={StartPayNo} And [TrnPayroll].[Id]<={EndPayNo},1,0)
            //    HAVING TrnPayroll.IsLocked=1 AND TrnPayrollLine.EmployeeId={EmployeeId} AND MstEmployee.PayrollGroupId={PayrollGroupId} AND IIf([TrnPayroll].[Id]>={StartPayNo} And [TrnPayroll].[Id]<={EndPayNo},1,0)=1;";
            //*** For CTL end ***

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<Get13Month>(sql).ToList();
            };

            return result;
        }

        public class Get13Month
        {
            public bool IsLocked { get; set; }
            public int PeriodId { get; set; }
            public int PayrollGroupId { get; set; }
            public int EmployeeId { get; set; }
            public int PayrollTypeId { get; set; }
            public decimal VariableSalary { get; set; }
            public decimal FixedSalary { get; set; }
        }
    }
}
