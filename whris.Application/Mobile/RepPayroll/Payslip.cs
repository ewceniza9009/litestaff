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
				MstEmployee.LoanBalance
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

            var sql = @"SELECT TrnPayroll.IsLocked, 
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
		            MstEmployee.LoanBalance
            FROM ((TrnPayrollLine INNER JOIN TrnPayroll ON TrnPayrollLine.PayrollId = TrnPayroll.Id) 
                INNER JOIN MstEmployee ON TrnPayrollLine.EmployeeId = MstEmployee.Id) 
                INNER JOIN MstCompany ON MstEmployee.CompanyId = MstCompany.Id
            WHERE TrnPayroll.IsLocked=1 
              AND TrnPayrollLine.PayrollId = @PayrollId 
              AND TrnPayroll.PayrollGroupId = @GroupId 
              AND dbo.Encode(TrnPayrollLine.EmployeeId) = @MobileCode;";

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
        }
    }
}
