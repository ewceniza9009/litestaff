using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static whris.Application.Queries.TrnDtr.GetEmployees;
using whris.Data.Data;
using Dapper;

namespace whris.Application.Queries.TrnLastWithholdingTax
{
    public class Main
    {
        public int LastWithholdingTaxId { get; set; }
        public int PeriodId { get; set; }
        public int PayrollGroupId { get; set; }        
        public int? EmployeeId { get; set; }        

        public List<LastWithholdingTaxDetail> Result()
        {
            var result = new List<LastWithholdingTaxDetail>();

            var addSql = EmployeeId == null ? "" : $@" TrnPayrollLine.Employee={EmployeeId}";
            var sql = $@"SELECT { LastWithholdingTaxId } AS LastWithholdingTaxId, 
	                            TrnPayrollLine.EmployeeId, 
	                            MstEmployee.TaxCodeId, 
	                            Sum(TrnPayrollLine.TotalNetSalaryAmount) AS SumOfTotalNetSalaryAmount, 
	                            Sum(TrnPayrollLine.TotalOtherIncomeTaxable) AS SumOfTotalOtherIncomeTaxable, 
	                            Sum(TrnPayrollLine.SSSContribution) AS SumOfSSSContribution, 
	                            Sum(TrnPayrollLine.SSSECContribution) AS SumOfSSSECContribution, 
	                            Sum(TrnPayrollLine.PHICContribution) AS SumOfPHICContribution, 
	                            Sum(TrnPayrollLine.HDMFContribution) AS SumOfHDMFContribution, 
	                            Sum(0) AS TotalOtherDeduction1, [ExemptionAmount]+[DependentAmount] AS Exemption1, 0 AS Tax1, 
	                            Sum(TrnPayrollLine.Tax) AS SumOfTax, 0 AS LastTax1
                            FROM ((TrnPayrollLine INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id) 
	                            INNER JOIN MstEmployee ON TrnPayrollLine.EmployeeId = MstEmployee.Id) 
	                            INNER JOIN MstTaxCode ON MstEmployee.TaxCodeId = MstTaxCode.Id
                            GROUP BY TrnPayrollLine.EmployeeId, 
	                            MstEmployee.TaxCodeId, 
	                            [ExemptionAmount]+[DependentAmount], 
	                            TrnPayroll.PeriodId, TrnPayroll.IsLocked, 
	                            MstEmployee.PayrollGroupId
                            HAVING TrnPayroll.PeriodId={ PeriodId } AND TrnPayroll.IsLocked=1 AND MstEmployee.PayrollGroupId={ PayrollGroupId }" + addSql;

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<LastWithholdingTaxDetail>(sql).ToList();
            };

            return result;
        }

        public class LastWithholdingTaxDetail 
        {
            public int LastWithholdingTaxId { get; set; }
            public int EmployeeId { get; set; }
            public int TaxCodeId { get; set; }
            public decimal SumOfTotalNetSalaryAmount { get; set; }
            public decimal SumOfTotalOtherIncomeTaxable { get; set; }
            public decimal SumOfSSSContribution { get; set; }
            public decimal SumOfSSSECContribution { get; set; }
            public decimal SumOfPHICContribution { get; set; }
            public decimal SumOfHDMFContribution { get; set; }
            public decimal TotalOtherDeduction1 { get; set; }
            public decimal Exemption1 { get; set; }
            public decimal Tax1 { get; set; }
            public decimal SumOfTax { get; set; }
            public decimal LastTax1 { get; set; }
            public int PeriodId { get; set; }
            public bool IsLocked { get; set; }
            public int PayrollGroupId { get; set; }
        }
    }
}
