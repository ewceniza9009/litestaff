using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using whris.Data.Data;
using static whris.Application.Queries.TrnDtr.GetEmployees;
using static whris.Application.Queries.TrnPayrollOtherDeduction.EmployeeLoanList;

namespace whris.Application.Queries.TrnPayrollOtherDeduction
{
    public class EmployeeLoanList
    {
        public int? EmployeeId { get; set; }
        
        public List<EmployeeLoan> Result() 
        {
            var result = new List<EmployeeLoan>();
            string sql = $@"SELECT MstEmployeeLoan.Id AS LoanId, 
                    MstEmployeeLoan.DateStart AS LoanDate, 
                    MstEmployeeLoan.MonthlyAmortization, 
                    MstEmployeeLoan.EmployeeId, 
                    MstEmployeeLoan.OtherDeductionId 
                FROM MstEmployeeLoan
                WHERE DateStart <= DATEADD(YEAR, 1, GETDATE()) AND DateStart >= DATEADD(YEAR, -1, GETDATE()) AND IsPaid = 0;";

            if (EmployeeId is not null || EmployeeId > 0)
            {
                sql = $@"SELECT MstEmployeeLoan.Id AS LoanId, 
                    MstEmployeeLoan.DateStart AS LoanDate, 
                    MstEmployeeLoan.MonthlyAmortization, 
                    MstEmployeeLoan.EmployeeId, 
                    MstEmployeeLoan.OtherDeductionId 
                FROM MstEmployeeLoan
                WHERE EmployeeId={EmployeeId} AND DateStart <= DATEADD(YEAR, 1, GETDATE()) AND DateStart >= DATEADD(YEAR, -1, GETDATE()) AND IsPaid = 0;";
            }

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<EmployeeLoan>(sql).ToList();
            };

            return result;
        }

        public class EmployeeLoan
        {
            public int LoanId { get; set; }
            [Column(TypeName = "datetime")]
            [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime LoanDate { get; set; }
            public string LoanDate2 => string.Format("{0:MM/dd/yyyy}", LoanDate);
            [Column(TypeName = "decimal(18, 5)")]
            [DisplayFormat(DataFormatString = "{0:N2}")]
            public decimal MonthlyAmortization { get; set; }
            public int EmployeeId { get; set; }
            public int OtherDeductionId { get; set; }
        }
    }

    public class EmployeeLoanPaymentList 
    {
        public int? EmployeeLoanId { get; set; }

        public List<EmployeeLoanPayment> Result()
        {
            var result = new List<EmployeeLoanPayment>();
            var sql = $@"SELECT TrnPayrollOtherDeduction.IsLocked, 
                            TrnPayrollOtherDeductionLine.EmployeeLoanId, 
                            Sum(TrnPayrollOtherDeductionLine.Amount) AS TotalPayment
                        FROM TrnPayrollOtherDeductionLine INNER JOIN TrnPayrollOtherDeduction ON TrnPayrollOtherDeductionLine.PayrollOtherDeductionId = TrnPayrollOtherDeduction.Id
                        GROUP BY TrnPayrollOtherDeduction.IsLocked, TrnPayrollOtherDeductionLine.EmployeeLoanId
                        HAVING TrnPayrollOtherDeduction.IsLocked=1 AND Not TrnPayrollOtherDeductionLine.EmployeeLoanId Is Null AND TrnPayrollOtherDeductionLine.EmployeeLoanId={EmployeeLoanId};";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<EmployeeLoanPayment>(sql).ToList();
            };

            return result;
        }

        public class EmployeeLoanPayment 
        {
            public bool IsLocked { get; set; }     
            public int EmployeeLoanId { get; set; }     
            public decimal TotalPayment { get; set; }     
        }
    }
}
