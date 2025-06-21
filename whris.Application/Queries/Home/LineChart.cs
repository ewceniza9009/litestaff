using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.Queries.Home
{
    public class LoanLineChart
    {
        public List<Loan> Result()
        {
            var result = new List<Loan>();
            string sql = $@"SELECT [Month], MonthNumber, [Cash Advance] AS CashAdvances, [SSS Loan] AS SSSLoans
                FROM 
                   (SELECT Format([DateStart],'yyyy') AS [Year], 
		                MstOtherDeduction.OtherDeduction, 
		                MONTH([DateStart]) AS MonthNumber,
		                Format([DateStart],'MMMM') AS [Month], 
		                Sum(MstEmployeeLoan.LoanAmount) AS Amount
	                FROM MstEmployeeLoan LEFT JOIN MstOtherDeduction ON MstEmployeeLoan.OtherDeductionId = MstOtherDeduction.Id
	                GROUP BY Format([DateStart],'yyyy'), MstOtherDeduction.OtherDeduction, MONTH([DateStart]), Format([DateStart],'MMMM'), IsLocked
	                HAVING (((Format([DateStart],'yyyy'))=Format(GETDATE(),'yyyy')) AND IsLocked = 1)
                   ) Loans
                PIVOT
                   ( SUM (Amount)
                     FOR [OtherDeduction] IN ([Cash Advance], [SSS Loan])
                   ) AS pvt
                ORDER BY MonthNumber ASC";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<Loan>(sql).ToList();
            };

            return result;
        }

        public class Loan
        {
            public string? Month { get; set; }
            public string? CashAdvances { get; set; }
            public string? SSSLoans { get; set; }
            public decimal Amount { get; set; }
        }
    }

    public class LeaveLineChart
    {
        public List<Leave> Result()
        {
            var result = new List<Leave>();
            string sql = $@"SELECT [Month], MonthNumber, [WithPay] AS WithPay, [NoPay] AS NoPay
                FROM 
	                (SELECT Format([Date],'yyyy') AS [Year], 
		                Format([Date],'MMMM') AS [Month], 
		                Month([Date]) AS [MonthNumber], 
		                IIf(IsNull([WithPay],0) = 1,'WithPay','NoPay') AS Type, 
		                Sum(TrnLeaveApplicationLine.NumberOfHours) AS NoOfHours
	                FROM TrnLeaveApplicationLine LEFT JOIN TrnLeaveApplication ON TrnLeaveApplicationLine.LeaveApplicationId = TrnLeaveApplication.Id
	                WHERE TrnLeaveApplication.IsLocked=1 AND IsNull(TrnLeaveApplicationLine.DebitToLedger, 0) = 0
	                GROUP BY Format([Date],'yyyy'), 
		                Format([Date],'MMMM'), 
		                Month([Date]), 
		                IIf(IsNull([WithPay],0) = 1,'WithPay','NoPay')
	                HAVING ((Format(Date,'yyyy'))=Format(GETDATE(),'yyyy'))
	                ) Leaves
                PIVOT
                    ( SUM (NoOfHours)
                        FOR [Type] IN ([WithPay], [NoPay])
                    ) AS pvt
                ORDER BY MonthNumber ASC";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<Leave>(sql).ToList();
            };

            return result;
        }

        public class Leave
        {
            public string? Month { get; set; }
            public string? WithPay { get; set; }
            public string? NoPay { get; set; }
            public decimal NoOfHours { get; set; }
        }
    }

    public class OvertimeLineChart
    {
        public List<Overtime> Result()
        {
            var result = new List<Overtime>();
            string sql = $@"SELECT Format([Date],'yyyy') AS [Year], 
		                Format([Date],'MMMM') AS [Month], 
		                Month([Date]) AS [MonthNumber],
		                Sum(TrnOverTimeLine.OvertimeHours) AS NoOfHours
	                FROM TrnOverTimeLine LEFT JOIN TrnOverTime ON TrnOverTimeLine.OverTimeId = TrnOverTime.Id
	                WHERE TrnOverTime.IsLocked=1
	                GROUP BY Format([Date],'yyyy'), 
		                Format([Date],'MMMM'), 
		                Month([Date])
	                HAVING ((Format(Date,'yyyy'))=Format(GETDATE(),'yyyy'))
					ORDER BY MonthNumber ASC";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<Overtime>(sql).ToList();
            };

            return result;
        }

        public class Overtime
        {
            public string? Month { get; set; }
            public decimal NoOfHours { get; set; }
        }
    }
}
