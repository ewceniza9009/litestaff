using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.Library
{
    public class Utilities
    {
        public static void DeleteAllTransactions() 
        {
            string sql = $@"DELETE FROM TrnPayroll
                DELETE FROM TrnDTR
                DELETE FROM TrnChangeShift
                DELETE FROM TrnOverTime
                DELETE FROM TrnLeaveApplication
                DELETE FROM TrnPayrollOtherIncome
                DELETE FROM TrnPayrollOtherDeduction
                DELETE FROM TrnLastWithholdingTax
                DELETE FROM MstEmployeeLoan 
                DELETE FROM CalendarTask 

                DBCC CHECKIDENT ('[TrnChangeShift]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnChangeShiftLine]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnDTR]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnDTRLine]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnDTRLog]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnLogs]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnLastWithholdingTax]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnLastWithholdingTaxLine]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnLeaveApplication]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnLeaveApplicationLine]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnLeaveLedger]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnLoanLedger]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnOverTime]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnOverTimeLine]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnPayroll]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnPayrollLine]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnPayrollOtherDeduction]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnPayrollOtherDeductionLine]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnPayrollOtherIncome]', RESEED, 0);
                DBCC CHECKIDENT ('[TrnPayrollOtherIncomeLine]', RESEED, 0);
                DBCC CHECKIDENT ('[MstEmployeeLoan]', RESEED, 0);
                DBCC CHECKIDENT ('[CalendarTask]', RESEED, 0);";
                
                //DBCC CHECKIDENT ('[MstEmployee]', RESEED, 0);";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Execute(sql);
            };
        }
    }
}
