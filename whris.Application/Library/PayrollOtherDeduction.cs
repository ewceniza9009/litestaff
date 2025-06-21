using Microsoft.EntityFrameworkCore;
using whris.Application.CQRS.TrnPayrollOtherDeduction.Commands;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Library
{
    public class PayrollOtherDeduction
    {
        internal static async Task EncodePayrollOtherDeductionLines(AddPayrollOtherDeductionsByQuickEncode command)
        {
            using (var ctx = new HRISContext())
            {
                if (command.EmployeeId is null)
                {
                    var removeExistingLines = ctx.TrnPayrollOtherDeductionLines.Where(x => x.PayrollOtherDeductionId == command.PODId);
                    ctx.TrnPayrollOtherDeductionLines.RemoveRange(removeExistingLines);
                }
                else 
                {
                    var removeExistingLines = ctx.TrnPayrollOtherDeductionLines.Where(x => x.PayrollOtherDeductionId == command.PODId && x.EmployeeId == command.EmployeeId);
                    ctx.TrnPayrollOtherDeductionLines.RemoveRange(removeExistingLines);
                }

                await ctx.SaveChangesAsync();

                var employeeIds = new List<int>();

                if (command.EmployeeId is not null)
                {
                    employeeIds.Add(command?.EmployeeId ?? 0);
                }
                else 
                {
                    employeeIds = ctx.MstEmployees
                        .Where(x => x.PayrollGroupId == command.PayrollGroupId && x.IsLocked)
                        .Select(x => x.Id)
                        .ToList();
                }

                foreach (var empId in employeeIds) 
                {
                    var selectedOtherDeductions = command?.SelectedOtherDeductions ?? new List<Dtos.TmpOtherDeduction>();

                    foreach (var item in selectedOtherDeductions.Where(x => x.IsSelected)) 
                    {
                        var podLine = new TrnPayrollOtherDeductionLine()
                        {
                            PayrollOtherDeductionId = command?.PODId ?? 0,
                            EmployeeId = empId,
                            OtherDeductionId = item.Id,
                            Amount = item.Amount,
                        };

                        await ctx.TrnPayrollOtherDeductionLines.AddAsync(podLine);
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task EncodeLoansPayrollOtherDeductionLines(AddPayrollOtherDeductionsByLoans command) 
        {
            using (var ctx = new HRISContext())
            {
                var employeeIds = new List<int>();
                var removeExistingLines = ctx.TrnPayrollOtherDeductionLines.Where(x => x.PayrollOtherDeductionId == command.PODId && x.EmployeeLoanId != null);

                 employeeIds = ctx.MstEmployees
                       .Where(x => x.PayrollGroupId == command.PayrollGroupId && x.IsLocked)
                       .Select(x => x.Id)
                       .ToList();

                ctx.TrnPayrollOtherDeductionLines.RemoveRange(removeExistingLines);

                await ctx.SaveChangesAsync();

                foreach (var empId in employeeIds)
                {
                    var loans = ctx.MstEmployeeLoans
                        .Where(x => (x.LoanNumber == 0 || x.LoanNumber == command.LoanNumber) 
                            && !x.IsPaid && 
                            x.EmployeeId == empId)
                        .ToList();

                    foreach (var loan in loans) 
                    {
                        var amount = loan.MonthlyAmortization;

                        if (loan.Balance < loan.MonthlyAmortization) 
                        {
                            amount = loan.Balance;
                        }

                        var newPODLine = new TrnPayrollOtherDeductionLine()
                        {
                            PayrollOtherDeductionId = command.PODId,
                            EmployeeId = empId,
                            OtherDeductionId = loan.OtherDeductionId,
                            EmployeeLoanId = loan.Id,
                            Amount = amount
                        };

                        ctx.TrnPayrollOtherDeductionLines.Add(newPODLine);
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task UpdateDeletedLoansFromPOD(List<int?> loanIds, HRISContext ctx) 
        {
            foreach (var loanId in loanIds) 
            {
                var loan = await ctx.MstEmployeeLoans.FirstOrDefaultAsync(x => x.Id == loanId);
                var paymentList = new Queries.TrnPayrollOtherDeduction.EmployeeLoanPaymentList();

                paymentList.EmployeeLoanId = loan?.Id ?? 0;

                var loanAmount = loan?.LoanAmount ?? 0;
                var payments = paymentList.Result().FirstOrDefault()?.TotalPayment ?? 0;
                var balance = loanAmount - payments;

                var isPaid = true;

                if (balance > 0)
                {
                    isPaid = false;
                }

                loan.TotalPayment = payments;
                loan.Balance = balance;
                loan.IsPaid = isPaid;
            }
        }

        internal static async Task UpdateLoanBalance(int podId, HRISContext ctx)
        {
            var podLines = await ctx.TrnPayrollOtherDeductionLines
                    .Where(x => x.PayrollOtherDeductionId == podId)
                    .ToListAsync();

            foreach (var line in podLines)
            {
                if (line.EmployeeLoanId is not null)
                {
                    var loan = await ctx.MstEmployeeLoans.FirstOrDefaultAsync(x => x.Id == line.EmployeeLoanId);

                    if (loan is not null)
                    {
                        var paymentList = new Queries.TrnPayrollOtherDeduction.EmployeeLoanPaymentList();

                        paymentList.EmployeeLoanId = line.EmployeeLoanId;

                        var loanAmount = loan.LoanAmount;
                        var payments = paymentList.Result().FirstOrDefault()?.TotalPayment ?? 0;
                        var balance = loanAmount - payments;

                        var isPaid = true;

                        if (balance > 0)
                        {
                            isPaid = false;
                        }

                        loan.TotalPayment = payments;
                        loan.Balance = balance;
                        loan.IsPaid = isPaid;
                    }
                }
            }

            await ctx.SaveChangesAsync();

            var employees = await ctx.MstEmployees
                    .Where(x => x.IsLocked)
                    .ToListAsync();

            foreach (var emp in employees)
            {
                var loanAmount = await ctx.MstEmployeeLoans
                    .Where(x => x.EmployeeId == emp.Id && x.IsLocked)
                    .SumAsync(x => x.LoanAmount);

                var loanPayment = await ctx.TrnPayrollOtherDeductionLines
                    .Include(x => x.PayrollOtherDeduction)
                    .Where(x => x.EmployeeId == emp.Id && x.PayrollOtherDeduction.IsLocked)
                    .SumAsync(x => x.Amount);

                emp.LoanBalance = loanAmount - loanPayment;
            }
        }
    }
}
