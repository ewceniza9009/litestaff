using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;
using Dapper;
using whris.Application.Library;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace whris.Application.Mobile.RepPayroll
{
    public class Payslip
    {
        public int PayrollId { get; set; }
        public string? MobileCode { get; set; }

        public List<PaySlipRecord> Result()
        {
            int groupId = MobileUtils.GetPayrollGroupId(MobileCode ?? "NA");

            var result = new List<PaySlipRecord>();
            var sql = $@"SELECT TrnPayroll.IsLocked, 
	            TrnPayrollLine.PayrollId, 
	            TrnPayroll.PayrollOtherDeductionId, 
	            TrnPayroll.PayrollNumber, 
	            TrnPayroll.PayrollDate, 
	            TrnPayroll.Remarks, 
	            MstCompany.Company, 
	            TrnPayrollLine.EmployeeId, 
	            MstEmployee.FullName, 
	            [TotalSalaryAmount]-[TotalLegalHolidayWorkingAmount]-[TotalSpecialHolidayWorkingAmount]-[TotalRegularRestdayAmount]-[TotalLegalHolidayRestdayAmount]-[TotalSpecialHolidayRestdayAmount]-[TotalRegularOvertimeAmount]-[TotalLegalHolidayOvertimeAmount]-[TotalSpecialHolidayOvertimeAmount]-[TotalRegularNightAmount]-[TotalLegalHolidayNightAmount]-[TotalSpecialHolidayNightAmount]-[TotalRegularNightOvertimeAmount]-[TotalLegalHolidayNightOvertimeAmount]-[TotalSpecialHolidayNightOvertimeAmount] AS BasicSalary, 
	            [TotalLegalHolidayWorkingAmount]+[TotalSpecialHolidayWorkingAmount]+[TotalRegularRestdayAmount]+[TotalLegalHolidayRestdayAmount]+[TotalSpecialHolidayRestdayAmount]+[TotalRegularOvertimeAmount]+[TotalLegalHolidayOvertimeAmount]+[TotalSpecialHolidayOvertimeAmount]+[TotalRegularNightAmount]+[TotalLegalHolidayNightAmount]+[TotalSpecialHolidayNightAmount]+[TotalRegularNightOvertimeAmount]+[TotalLegalHolidayNightOvertimeAmount]+[TotalSpecialHolidayNightOvertimeAmount] AS OtherSalary, 
	            TrnPayrollLine.TotalSalaryAmount, 
	            TrnPayrollLine.TotalTardyAmount, 
	            TrnPayrollLine.TotalAbsentAmount, 
	            TrnPayrollLine.TotalNetSalaryAmount,
	            TrnPayrollLine.TotalOtherIncomeTaxable, 
	            TrnPayrollLine.GrossIncome, 
	            TrnPayrollLine.TotalOtherIncomeNonTaxable, 
	            TrnPayrollLine.GrossIncomeWithNonTaxable, 
	            TrnPayrollLine.SSSContribution, 
	            TrnPayrollLine.PHICContribution, 
	            TrnPayrollLine.HDMFContribution, 
	            TrnPayrollLine.Tax, 
	            Coalesce([SSSContribution],0)+Coalesce([PHICContribution],0)+Coalesce([HDMFContribution],0)+Coalesce([Tax],0) AS TotalDeduction, 
	            TrnPayrollLine.TotalOtherDeduction, 
	            ('<table>' +
                STUFF(
                    (
                        SELECT 
                            '<tr><td style=""width: 135px; font-size: 12px;"">' + OtherDeduction + '</td><td style=""width: 50px; text-align: right; font-size: 12px;"">' + CONVERT(NVARCHAR, FORMAT(Round(Amount, 2), 'N2')) + '</td></tr>'
                        FROM (SELECT TrnPayrollOtherDeductionLine.PayrollOtherDeductionId, TrnPayrollOtherDeductionLine.EmployeeId, TrnPayrollOtherDeductionLine.OtherDeductionId, MstOtherDeduction.OtherDeduction, TrnPayrollOtherDeductionLine.EmployeeLoanId, TrnPayrollOtherDeductionLine.Amount
			                FROM TrnPayrollOtherDeductionLine INNER JOIN MstOtherDeduction ON TrnPayrollOtherDeductionLine.OtherDeductionId = MstOtherDeduction.Id
			            ) PayslipLengthwiseSub
			            WHERE PayslipLengthwiseSub.PayrollOtherDeductionId = TrnPayroll.PayrollOtherDeductionId AND PayslipLengthwiseSub.EmployeeId = TrnPayrollLine.EmployeeId
                        FOR XML PATH(''), ROOT('root'), TYPE
                    ).value('.', 'NVARCHAR(MAX)'), 1, 0, ''
                ) +
                '</table>') as OtherDeductionBreakdown,
	            TrnPayrollLine.NetIncome, 
	            TrnPayroll.PreparedBy,
	            [TotalRegularWorkingHours]+[TotalLegalHolidayWorkingHours]+[TotalSpecialHolidayWorkingHours] - [TotalTardyLateHours] - [TotalTardyUndertimeHours] AS TotalWorkingHours,
                MstEmployee.LeaveBalance,
				MstEmployee.LoanBalance,
            FROM ((TrnPayrollLine INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id) 
	            INNER JOIN MstEmployee ON TrnPayrollLine.EmployeeId = MstEmployee.Id) 
	            INNER JOIN MstCompany ON MstEmployee.CompanyId = MstCompany.Id
            WHERE TrnPayroll.IsLocked=1 AND TrnPayrollLine.PayrollId={PayrollId} AND TrnPayroll.PayrollGroupId={groupId} AND dbo.Encode(TrnPayrollLine.EmployeeId)={MobileCode};
             ";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<PaySlipRecord>(sql).ToList();
            };

            return result;
        }

        public async Task<IEnumerable<PaySlipRecord>> ResultAsync()
        {
            int groupId = await MobileUtils.GetPayrollGroupIdAsync(MobileCode ?? "NA");

            var sql = @"SELECT 
    TrnPayroll.IsLocked, 
    TrnPayrollLine.PayrollId, 
    TrnPayroll.PayrollOtherDeductionId, 
    TrnPayroll.PayrollNumber, 
    TrnPayroll.PayrollDate, 
    TrnPayroll.Remarks, 
    MstCompany.Company, 
    TrnPayrollLine.EmployeeId, 
    MstEmployee.FullName, 
    TrnPayrollLine.TotalSalaryAmount 
        - TrnPayrollLine.TotalLegalHolidayWorkingAmount
        - TrnPayrollLine.TotalSpecialHolidayWorkingAmount
        - TrnPayrollLine.TotalRegularRestdayAmount
        - TrnPayrollLine.TotalLegalHolidayRestdayAmount
        - TrnPayrollLine.TotalSpecialHolidayRestdayAmount
        - TrnPayrollLine.TotalRegularOvertimeAmount
        - TrnPayrollLine.TotalLegalHolidayOvertimeAmount
        - TrnPayrollLine.TotalSpecialHolidayOvertimeAmount
        - TrnPayrollLine.TotalRegularNightAmount
        - TrnPayrollLine.TotalLegalHolidayNightAmount
        - TrnPayrollLine.TotalSpecialHolidayNightAmount
        - TrnPayrollLine.TotalRegularNightOvertimeAmount
        - TrnPayrollLine.TotalLegalHolidayNightOvertimeAmount
        - TrnPayrollLine.TotalSpecialHolidayNightOvertimeAmount AS BasicSalary,

    TrnPayrollLine.TotalLegalHolidayWorkingAmount
        + TrnPayrollLine.TotalSpecialHolidayWorkingAmount
        + TrnPayrollLine.TotalRegularRestdayAmount
        + TrnPayrollLine.TotalLegalHolidayRestdayAmount
        + TrnPayrollLine.TotalSpecialHolidayRestdayAmount
        + TrnPayrollLine.TotalRegularOvertimeAmount
        + TrnPayrollLine.TotalLegalHolidayOvertimeAmount
        + TrnPayrollLine.TotalSpecialHolidayOvertimeAmount
        + TrnPayrollLine.TotalRegularNightAmount
        + TrnPayrollLine.TotalLegalHolidayNightAmount
        + TrnPayrollLine.TotalSpecialHolidayNightAmount
        + TrnPayrollLine.TotalRegularNightOvertimeAmount
        + TrnPayrollLine.TotalLegalHolidayNightOvertimeAmount
        + TrnPayrollLine.TotalSpecialHolidayNightOvertimeAmount AS OtherSalary,

    TrnPayrollLine.TotalLegalHolidayWorkingHours,
    TrnPayrollLine.TotalSpecialHolidayWorkingHours,
    TrnPayrollLine.TotalRegularNightHours, 
    TrnPayrollLine.TotalRegularRestdayHours,   
    TrnPayrollLine.TotalRegularOvertimeHours, 

    TrnPayrollLine.TotalSalaryAmount, 
    TrnPayrollLine.TotalTardyAmount, 
    TrnPayrollLine.TotalAbsentAmount, 
    TrnPayrollLine.TotalNetSalaryAmount,

    CAST(DTRSummary.AvgRatePerHour * ISNULL(TrnPayrollLine.TotalRegularRestdayHours, 0) * 0.3 AS DECIMAL(18, 2)) AS ComputedRegularRestdayAmount,

    TrnPayrollLine.TotalLegalHolidayWorkingAmount,
    TrnPayrollLine.TotalSpecialHolidayWorkingAmount,
    TrnPayrollLine.TotalRegularNightAmount,
    TrnPayrollLine.TotalRegularOvertimeAmount,

    TrnPayrollLine.TotalOtherIncomeTaxable, 
    TrnPayrollLine.GrossIncome, 
    TrnPayrollLine.TotalOtherIncomeNonTaxable, 
    TrnPayrollLine.GrossIncomeWithNonTaxable, 
    TrnPayrollLine.SSSContribution, 
    TrnPayrollLine.PHICContribution, 
    TrnPayrollLine.HDMFContribution, 
    TrnPayrollLine.Tax, 

    COALESCE(TrnPayrollLine.SSSContribution, 0) 
        + COALESCE(TrnPayrollLine.PHICContribution, 0) 
        + COALESCE(TrnPayrollLine.HDMFContribution, 0) 
        + COALESCE(TrnPayrollLine.Tax, 0) AS TotalDeduction, 

    TrnPayrollLine.TotalOtherDeduction, 

	(
    '<table>' +
    STUFF(
        (
            SELECT 
                '<tr><td style=""width: 135px; font-size: 12px;"">' 
                + LeaveType + 
                '</td><td style=""width: 50px; text-align: right; font-size: 12px;"">' 
                + CONVERT(NVARCHAR, FORMAT(ROUND(Balance, 2), 'N2')) 
                + '</td></tr>'
            FROM (
                SELECT 
                    LL.LeaveType,
                    SUM(LL.Debit - LL.Credit) AS Balance
                FROM TrnLeaveLedger AS LL
                WHERE LL.EmployeeId = TrnPayrollLine.EmployeeId
                GROUP BY LL.LeaveType
            ) AS Balances
            FOR XML PATH(''), TYPE
        ).value('.', 'NVARCHAR(MAX)'), 1, 0, ''
    ) + '</table>'
) AS LeaveBalanceBreakdown,



    ('<table>' +
        STUFF(
            (
                SELECT 
                    '<tr><td style=""""width: 135px; font-size: 12px;"""">' + OtherDeduction + '</td><td style=""""width: 50px; text-align: right; font-size: 12px;"""">' + CONVERT(NVARCHAR, FORMAT(ROUND(Amount, 2), 'N2')) + '</td></tr>'
                FROM (
                    SELECT 
                        TrnPayrollOtherDeductionLine.PayrollOtherDeductionId, 
                        TrnPayrollOtherDeductionLine.EmployeeId, 
                        TrnPayrollOtherDeductionLine.OtherDeductionId, 
                        MstOtherDeduction.OtherDeduction, 
                        TrnPayrollOtherDeductionLine.EmployeeLoanId, 
                        TrnPayrollOtherDeductionLine.Amount
                    FROM TrnPayrollOtherDeductionLine 
                    INNER JOIN MstOtherDeduction 
                        ON TrnPayrollOtherDeductionLine.OtherDeductionId = MstOtherDeduction.Id
                ) PayslipLengthwiseSub
                WHERE PayslipLengthwiseSub.PayrollOtherDeductionId = TrnPayroll.PayrollOtherDeductionId 
                      AND PayslipLengthwiseSub.EmployeeId = TrnPayrollLine.EmployeeId
                FOR XML PATH(''), ROOT('root'), TYPE
            ).value('.', 'NVARCHAR(MAX)'), 1, 0, ''
        ) + '</table>'
    ) AS OtherDeductionBreakdown,

    STUFF(
        (
            SELECT CONVERT(NVARCHAR, FORMAT(ROUND(LoanSub.Balance, 2), 'N2'))                           
            FROM (
                SELECT 
                    MstEmployeeLoan.Id,
                    MstEmployeeLoan.EmployeeId,
                    MstEmployeeLoan.OtherDeductionId,
                    MstOtherDeduction.OtherDeduction,
                    MstEmployeeLoan.Balance
                FROM MstEmployeeLoan
                INNER JOIN MstOtherDeduction
                    ON MstOtherDeduction.Id = MstEmployeeLoan.OtherDeductionId
            ) LoanSub
            WHERE LoanSub.EmployeeId = TrnPayrollLine.EmployeeId
            FOR XML PATH(''), ROOT('root'), TYPE
        ).value('.', 'NVARCHAR(MAX)'), 1, 0, ''
    ) AS LoanBalances,

    TrnPayrollLine.NetIncome, 
    TrnPayroll.PreparedBy,
    TrnPayrollLine.TotalRegularWorkingHours + TrnPayrollLine.TotalLegalHolidayWorkingHours + TrnPayrollLine.TotalSpecialHolidayWorkingHours 
        - TrnPayrollLine.TotalTardyLateHours - TrnPayrollLine.TotalTardyUndertimeHours AS TotalWorkingHours,

    MstEmployee.LeaveBalance,
    MstEmployee.LoanBalance

FROM TrnPayrollLine 
INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id
INNER JOIN MstEmployee ON TrnPayrollLine.EmployeeId = MstEmployee.Id
INNER JOIN MstCompany ON MstEmployee.CompanyId = MstCompany.Id
LEFT JOIN (
    SELECT 
        EmployeeId,
        AVG(RatePerHour) AS AvgRatePerHour
    FROM TrnDTRLine
    WHERE RestDay = 1
    GROUP BY EmployeeId
) AS DTRSummary ON DTRSummary.EmployeeId = TrnPayrollLine.EmployeeId


WHERE TrnPayroll.IsLocked = 1
    AND TrnPayrollLine.PayrollId = @PayrollId 
    AND TrnPayroll.PayrollGroupId = @GroupId 
    AND dbo.Encode(TrnPayrollLine.EmployeeId) = @MobileCode

GROUP BY 
    TrnPayroll.IsLocked,
    TrnPayrollLine.PayrollId,
    TrnPayroll.PayrollOtherDeductionId,
    TrnPayroll.PayrollNumber,
    TrnPayroll.PayrollDate,
    TrnPayroll.Remarks,
    MstCompany.Company,
    TrnPayrollLine.EmployeeId,

    MstEmployee.FullName,
    TrnPayrollLine.TotalSalaryAmount,
    TrnPayrollLine.TotalLegalHolidayWorkingAmount,
    TrnPayrollLine.TotalSpecialHolidayWorkingAmount,
    TrnPayrollLine.TotalRegularRestdayAmount,
    TrnPayrollLine.TotalLegalHolidayRestdayAmount,
    TrnPayrollLine.TotalSpecialHolidayRestdayAmount,
    TrnPayrollLine.TotalRegularOvertimeAmount,
    TrnPayrollLine.TotalLegalHolidayOvertimeAmount,
    TrnPayrollLine.TotalSpecialHolidayOvertimeAmount,
    TrnPayrollLine.TotalRegularNightAmount,
    TrnPayrollLine.TotalLegalHolidayNightAmount,
    TrnPayrollLine.TotalSpecialHolidayNightAmount,
    TrnPayrollLine.TotalRegularNightOvertimeAmount,
    TrnPayrollLine.TotalLegalHolidayNightOvertimeAmount,
    TrnPayrollLine.TotalSpecialHolidayNightOvertimeAmount,
    TrnPayrollLine.TotalLegalHolidayWorkingHours,
    TrnPayrollLine.TotalSpecialHolidayWorkingHours,
    TrnPayrollLine.TotalRegularNightHours,
    TrnPayrollLine.TotalRegularRestdayHours,
    TrnPayrollLine.TotalRegularOvertimeHours,
    TrnPayrollLine.TotalTardyAmount,
    TrnPayrollLine.TotalAbsentAmount,
    TrnPayrollLine.TotalNetSalaryAmount,
    DTRSummary.AvgRatePerHour,
    TrnPayrollLine.TotalOtherIncomeTaxable,
    TrnPayrollLine.GrossIncome,
    TrnPayrollLine.TotalOtherIncomeNonTaxable,
    TrnPayrollLine.GrossIncomeWithNonTaxable,
    TrnPayrollLine.SSSContribution,
    TrnPayrollLine.PHICContribution,
    TrnPayrollLine.HDMFContribution,
    TrnPayrollLine.Tax,
    TrnPayrollLine.TotalOtherDeduction,
    TrnPayrollLine.NetIncome,
    TrnPayroll.PreparedBy,
    TrnPayrollLine.TotalRegularWorkingHours,
    TrnPayrollLine.TotalTardyLateHours,
    TrnPayrollLine.TotalTardyUndertimeHours,
	
    MstEmployee.LeaveBalance,
    MstEmployee.LoanBalance
";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                return await connection.QueryAsync<PaySlipRecord>(sql,
                    new { PayrollId, GroupId = groupId, MobileCode });
            }
            ;
        }

        public class PaySlipRecord
        {
                public bool IsLocked {get; set;}
                public int PayrollId {get; set;}
                public int PayrollOtherDeductionId {get; set;}
                public string? PayrollNumber {get; set;}
                public DateTime PayrollDate {get; set;}
                public string? Remarks {get; set;}
                public string? Company {get; set;}
                public int EmployeeId {get; set;}
                public string? FullName {get; set;}
                public decimal BasicSalary {get; set;}
                public decimal OtherSalary {get; set;}
                public decimal TotalSalaryAmount {get; set;}
                public decimal TotalTardyAmount {get; set;}
                public decimal TotalAbsentAmount {get; set;}
                public decimal TotalNetSalaryAmount {get; set;}
                public decimal TotalLegalHolidayWorkingAmount { get; set; } 
                public decimal TotalSpecialHolidayWorkingAmount { get; set; }
                public decimal TotalRegularNightAmount { get; set; }
                public decimal TotalRegularOvertimeAmount { get; set; }
                public decimal ComputedRegularRestdayAmount { get; set; }
                public decimal LoanBalances { get; set; }
                public decimal TotalOtherIncomeTaxable {get; set;}
                public decimal GrossIncome {get; set;}
                public decimal TotalOtherIncomeNonTaxable {get; set;}
                public decimal GrossIncomeWithNonTaxable {get; set;}
                public decimal SSSContribution {get; set;}
                public decimal PHICContribution {get; set;}
                public decimal HDMFContribution {get; set;}
                public decimal Tax {get; set;}
                public decimal TotalDeduction {get; set;}
                public decimal TotalOtherDeduction {get; set;} 
	            public string? OtherDeductionBreakdown {get; set;}
	            public decimal NetIncome {get; set;} 
	            public int PreparedBy {get; set;}
	            public decimal TotalWorkingHours { get; set; }
                public decimal TotalLegalHolidayWorkingHours { get; set; }
                public decimal TotalSpecialHolidayWorkingHours { get; set; }
                public decimal TotalRegularNightHours { get; set; }
                public decimal TotalRegularRestdayHours { get; set; }
                public decimal TotalRegularOvertimeHours { get; set; }
                public string? LeaveBalanceBreakdown { get; set; }
        }
    }
}
