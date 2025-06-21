using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using static whris.Application.Queries.TrnPayrollOtherDeduction.EmployeeLoanList;
using whris.Data.Data;
using Dapper;

namespace whris.Application.Queries.TrnLoan
{
    public class PaymentList
    {
        public int? EmployeeLoanId { get; set; }

        public List<OtherDeduction> Result()
        {
            var result = new List<OtherDeduction>();
            string sql = $@"SELECT TrnPayrollOtherDeduction.IsLocked, 
                    TrnPayrollOtherDeduction.PODNumber, 
                    TrnPayrollOtherDeduction.Remarks,
                    TrnPayrollOtherDeduction.PODDate, 
                    TrnPayrollOtherDeductionLine.EmployeeLoanId, 
                    TrnPayrollOtherDeductionLine.Amount
                FROM TrnPayrollOtherDeductionLine INNER JOIN TrnPayrollOtherDeduction ON TrnPayrollOtherDeductionLine.PayrollOtherDeductionId = TrnPayrollOtherDeduction.Id
                WHERE TrnPayrollOtherDeduction.IsLocked=1 AND TrnPayrollOtherDeductionLine.EmployeeLoanId={EmployeeLoanId};";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<OtherDeduction>(sql).ToList();
            };

            return result;
        }

        public class OtherDeduction
        {
            public bool IsLocked { get; set; }
            public string? PODNumber { get; set; }
            public string? Remarks { get; set; }
            [Column(TypeName = "datetime")]
            [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime PODDate { get; set; }
            [Column(TypeName = "decimal(18, 5)")]
            public int EmployeeLoanId { get; set; }
            [DisplayFormat(DataFormatString = "{0:N2}")]
            public decimal Amount { get; set; }           
        }
    }
}
