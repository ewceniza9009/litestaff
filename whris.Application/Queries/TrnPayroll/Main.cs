using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static whris.Application.Queries.TrnLoan.PaymentList;
using whris.Data.Data;
using Dapper;
using whris.Application.Library;

namespace whris.Application.Queries.TrnPayroll
{
    public class PayrollDetailMandatoryList
    {
        public int PayrollId { get; set; } 
        public int MonthId { get; set; }

        public List<PayrollDetailMandatory> Result() 
        {
            var result = new List<PayrollDetailMandatory>();
            var sql = $@"SELECT TrnPayrollLine.PayrollId, 
                            TrnPayroll.MonthId, 
                            TrnPayrollLine.EmployeeId, 
                            Sum([TotalSalaryAmount]-[TotalLegalHolidayWorkingAmount]-[TotalSpecialHolidayWorkingAmount]-[TotalRegularRestdayAmount]-[TotalLegalHolidayRestdayAmount]-[TotalSpecialHolidayRestdayAmount]-[TotalRegularOvertimeAmount]-[TotalLegalHolidayOvertimeAmount]-[TotalSpecialHolidayOvertimeAmount]-[TotalRegularNightAmount]-[TotalLegalHolidayNightAmount]-[TotalSpecialHolidayNightAmount]-[TotalRegularNightOvertimeAmount]-[TotalLegalHolidayNightOvertimeAmount]-[TotalSpecialHolidayNightOvertimeAmount]) AS BasicAmountOfTheMonth, 
                            Sum(TrnPayrollLine.TotalSalaryAmount) AS TotalSalaryAmountOfTheMonth, 
                            Sum(TrnPayrollLine.GrossIncome) AS TotalGrossIncome, 
                            Sum(TrnPayrollLine.SSSContribution) AS SSSContributionOfTheMonth, 
                            Sum(TrnPayrollLine.SSSECContribution) AS SSSECContributionOfTheMonth, 
                            Sum(TrnPayrollLine.PHICContribution) AS PHICContributionOfTheMonth, 
                            Sum(TrnPayrollLine.HDMFContribution) AS HDMFContributionOfTheMonth, 
                            Sum(TrnPayrollLine.SSSContributionEmployer) AS SSSContributionEmployerOfTheMonth, 
                            Sum(TrnPayrollLine.SSSECContributionEmployer) AS SSSECContributionEmployerOfTheMonth, 
                            Sum(TrnPayrollLine.PHICContributionEmployer) AS PHICContributionEmployerOfTheMonth, 
                            Sum(TrnPayrollLine.HDMFContributionEmployer) AS HDMFContributionEmployerOfTheMonth, 
                            TrnPayroll.IsLocked
                        FROM TrnPayrollLine INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id
                        GROUP BY TrnPayrollLine.PayrollId, TrnPayroll.MonthId, TrnPayrollLine.EmployeeId, TrnPayroll.IsLocked
                        HAVING TrnPayrollLine.PayrollId<>{PayrollId} AND TrnPayroll.MonthId={MonthId} AND TrnPayroll.IsLocked=1;";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<PayrollDetailMandatory>(sql).ToList();
            };

            return result;
        }

        public class PayrollDetailMandatory
        {
            public int PayrollId { get; set; }
            public int MonthId { get; set; }
            public int EmployeeId { get; set; }
            public decimal BasicAmountOfTheMonth { get; set; }
            public decimal TotalSalaryAmountOfTheMonth { get; set; }
            public decimal TotalGrossIncome { get; set; }
            public decimal SSSContributionOfTheMonth { get; set; }
            public decimal SSSECContributionOfTheMonth { get; set; }
            public decimal PHICContributionOfTheMonth { get; set; }
            public decimal HDMFContributionOfTheMonth { get; set; }
            public decimal SSSContributionEmployerOfTheMonth { get; set; }
            public decimal SSSECContributionEmployerOfTheMonth { get; set; }
            public decimal PHICContributionEmployerOfTheMonth { get; set; }
            public decimal HDMFContributionEmployerOfTheMonth { get; set; }
            public bool IsLocked { get; set; }
        }
    }

    public class PayrollDetailWithholdingList
    {
        public int PayrollId { get; set; } 
        public int MonthId { get; set; }

        public List<PayrollDetailWithholding> Result() 
        {
            var result = new List<PayrollDetailWithholding>();
            var sql = $@"SELECT TrnPayrollLine.PayrollId, 
                            TrnPayroll.MonthId, 
                            TrnPayrollLine.EmployeeId, 
                            Sum(TrnPayrollLine.TotalNetSalaryAmount) AS TotalSalaryAmountOfTheMonth, 
                            Sum(TrnPayrollLine.TotalOtherIncomeTaxable) AS TotalOtherIncomeTaxableOfTheMonth, 
                            Sum(TrnPayrollLine.GrossIncome) AS GrossIncomeOfTheMonth, 
                            Sum(TrnPayrollLine.Tax) AS TaxOfTheMonth, 
                            Sum(TrnPayrollLine.SSSContribution) AS SSSContributionOfTheMonth, 
                            Sum(TrnPayrollLine.SSSECContribution) AS SSSECContributionOfTheMonth, 
                            Sum(TrnPayrollLine.PHICContribution) AS PHICContributionOfTheMonth, 
                            Sum(TrnPayrollLine.HDMFContribution) AS HDMFContributionOfTheMonth
                        FROM TrnPayrollLine INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id
                        GROUP BY TrnPayrollLine.PayrollId, TrnPayroll.MonthId, TrnPayrollLine.EmployeeId
                        HAVING TrnPayrollLine.PayrollId<>{PayrollId} AND TrnPayroll.MonthId={MonthId};
                        ";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<PayrollDetailWithholding>(sql).ToList();
            };

            return result;
        }

        public class PayrollDetailWithholding
        {
            public int PayrollId { get; set; }
            public int MonthId { get; set; }
            public int EmployeeId { get; set; }
            public decimal TotalSalaryAmountOfTheMonth { get; set; }
            public decimal TotalOtherIncomeTaxableOfTheMonth { get; set; }
            public decimal GrossIncomeOfTheMonth { get; set; }
            public decimal TaxOfTheMonth { get; set; }
            public decimal SSSContributionOfTheMonth { get; set; }
            public decimal SSSECContributionOfTheMonth { get; set; }
            public decimal PHICContributionOfTheMonth { get; set; }
            public decimal HDMFContributionOfTheMonth { get; set; }
        }
    }

    public class PayrollDetailOtherIncomeList 
    {
        public int PayrollOtherIncomeId { get; set; }
        public bool IsTaxable { get; set; }

        public List<PayrollDetailOtherIncome> Result()
        {
            var result = new List<PayrollDetailOtherIncome>();
            var sql = $@"SELECT TrnPayrollOtherIncomeLine.PayrollOtherIncomeId, 
                            MstOtherIncome.Taxable, 
                            TrnPayrollOtherIncomeLine.EmployeeId, 
                            Sum(TrnPayrollOtherIncomeLine.Amount) AS TotalAmount
                        FROM (TrnPayrollOtherIncomeLine INNER JOIN TrnPayrollOtherIncome ON TrnPayrollOtherIncomeLine.PayrollOtherIncomeId = TrnPayrollOtherIncome.Id) 
                            INNER JOIN MstOtherIncome ON TrnPayrollOtherIncomeLine.OtherIncomeId = MstOtherIncome.Id
                        GROUP BY TrnPayrollOtherIncomeLine.PayrollOtherIncomeId, MstOtherIncome.Taxable, TrnPayrollOtherIncomeLine.EmployeeId
                        HAVING TrnPayrollOtherIncomeLine.PayrollOtherIncomeId={PayrollOtherIncomeId} AND MstOtherIncome.Taxable={(IsTaxable ? 1 : 0)};
                        ";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<PayrollDetailOtherIncome>(sql).ToList();
            };

            return result;
        }

        public class PayrollDetailOtherIncome
        {
            public int PayrollOtherIncomeId { get; set; }
            public bool Taxable { get; set; }
            public int EmployeeId { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}
