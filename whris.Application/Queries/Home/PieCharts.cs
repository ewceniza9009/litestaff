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
    public class OtherIncomePieChart
    {
        public List<OtherIncome> Result()
        {
            var result = new List<OtherIncome>();
            string sql = $@"SELECT TrnPayrollOtherIncomeLine.OtherIncomeId, 
	                MstOtherIncome.OtherIncome AS OtherIncomeAccount, 
	                Sum(TrnPayrollOtherIncomeLine.Amount) AS Amount,
	                (SELECT Sum(Amount) FROM TrnPayrollOtherIncomeLine) OverallTotal,
	                (Sum(TrnPayrollOtherIncomeLine.Amount)/(SELECT Sum(Amount) FROM TrnPayrollOtherIncomeLine)) * 100 AS Percentage
                FROM TrnPayrollOtherIncomeLine 
	                LEFT JOIN MstOtherIncome ON TrnPayrollOtherIncomeLine.OtherIncomeId = MstOtherIncome.Id 
	                LEFT JOIN TrnPayrollOtherIncome ON  TrnPayrollOtherIncome.Id = TrnPayrollOtherIncomeLine.PayrollOtherIncomeId
                WHERE IsLocked = 1 AND YEAR(POIDate) = YEAR(GETDATE())
                GROUP BY TrnPayrollOtherIncomeLine.OtherIncomeId, MstOtherIncome.OtherIncome, IsLocked;";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<OtherIncome>(sql).ToList();
            };

            return result;
        }

        public class OtherIncome
        {
            public int OtherIncomeId { get; set; }
            public string? OtherIncomeAccount { get; set; }
            public decimal Amount { get; set; }
            public decimal OverallTotal { get; set; }
            public decimal Percentage { get; set; }
        }
    }

    public class OtherDeductionPieChart
    {
        public List<OtherDeduction> Result()
        {
            var result = new List<OtherDeduction>();
            string sql = $@"SELECT TrnPayrollOtherDeductionLine.OtherDeductionId, 
	                MstOtherDeduction.OtherDeduction AS OtherDeductionAccount, 
	                Sum(TrnPayrollOtherDeductionLine.Amount) AS Amount,
	                (SELECT Sum(Amount) FROM TrnPayrollOtherDeductionLine) OverallTotal,
	                (Sum(TrnPayrollOtherDeductionLine.Amount)/(SELECT Sum(Amount) FROM TrnPayrollOtherDeductionLine)) * 100 AS Percentage
                FROM TrnPayrollOtherDeductionLine 
	                LEFT JOIN MstOtherDeduction ON TrnPayrollOtherDeductionLine.OtherDeductionId = MstOtherDeduction.Id
	                LEFT JOIN TrnPayrollOtherDeduction ON TrnPayrollOtherDeduction.Id = TrnPayrollOtherDeductionLine.PayrollOtherDeductionId 
                WHERE IsLocked = 1 AND YEAR(PODDate) = YEAR(GETDATE())
                GROUP BY TrnPayrollOtherDeductionLine.OtherDeductionId, MstOtherDeduction.OtherDeduction, IsLocked;";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<OtherDeduction>(sql).ToList();
            };

            return result;
        }

        public class OtherDeduction
        {
            public int OtherDeductionId { get; set; }
            public string? OtherDeductionAccount { get; set; }
            public decimal Amount { get; set; }
            public decimal OverallTotal { get; set; }
            public decimal Percentage { get; set; }
        }
    }
}
