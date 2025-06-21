using whris.Application.CQRS.TrnPayrollOtherIncome.Commands;
using whris.Application.Queries.TrnPayrollOtherIncome;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Library
{
    public class PayrollOtherIncome
    {
        internal static async Task Post13MonthPayrollOtherIncomeLines(AddPayrollOtherIncomesBy13Month command)
        {
            using (var ctx = new HRISContext())
            {
                var otherIncomeId = command.OtherIncomeId != 0 ? command.OtherIncomeId ?? 0: ctx.MstOtherIncomes
                    ?.FirstOrDefault(x => x.OtherIncome.Contains("13"))
                    ?.Id ?? 0;

                if (command.EmployeeId is null)
                {
                    var removeExistingLines = ctx.TrnPayrollOtherIncomeLines.Where(x => x.PayrollOtherIncomeId == command.POIId && x.OtherIncomeId == otherIncomeId);
                    ctx.TrnPayrollOtherIncomeLines.RemoveRange(removeExistingLines);
                }
                else
                {
                    var removeExistingLines = ctx.TrnPayrollOtherIncomeLines.Where(x => x.PayrollOtherIncomeId == command.POIId && x.OtherIncomeId == otherIncomeId && x.EmployeeId == command.EmployeeId);
                    ctx.TrnPayrollOtherIncomeLines.RemoveRange(removeExistingLines);
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
                        .Where(x => x.PayrollGroupId == command.PayrollGroupId)
                        .Select(x => x.Id)
                        .ToList();
                }

                var payrollOtherIncomeId = command?.POIId ?? 0;

                foreach (var empId in employeeIds)
                {
                    var payrollOtherIncome = new Main();

                    payrollOtherIncome.PayrollGroupId = command?.PayrollGroupId ?? 0;
                    payrollOtherIncome.StartPayNo = command?.StartPayNo ?? 0;
                    payrollOtherIncome.EndPayNo = command?.EndPayNo ?? 0;
                    payrollOtherIncome.NoOfPayroll = (command?.NoOfPayroll ?? 1) == 0 ? 1 : (command?.NoOfPayroll ?? 1);
                    payrollOtherIncome.EmployeeId = empId;

                    var payrollOtherIncomeEmployeeId = empId;
                    var payrollOtherIncome13MonthAmount = 0m;
                    var payrollOtherIncome13Month = payrollOtherIncome.Result()
                        .FirstOrDefault();

                    var payrollOtherIncomeLine = new TrnPayrollOtherIncomeLine() 
                    {
                        PayrollOtherIncomeId = payrollOtherIncomeId,
                        EmployeeId = payrollOtherIncomeEmployeeId,
                        OtherIncomeId = otherIncomeId
                    };

                    if (payrollOtherIncome13Month?.PayrollTypeId == 1)
                    {
                        payrollOtherIncome13MonthAmount = payrollOtherIncome13Month?.VariableSalary ?? 0;
                    }
                    else 
                    {
                        payrollOtherIncome13MonthAmount = payrollOtherIncome13Month?.FixedSalary ?? 0;
                    }

                    payrollOtherIncomeLine.Amount = payrollOtherIncome13MonthAmount;

                    await ctx.TrnPayrollOtherIncomeLines.AddAsync(payrollOtherIncomeLine);
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task EncodePayrollOtherIncomeLines(AddPayrollOtherIncomesByQuickEncode command)
            {
                using (var ctx = new HRISContext())
                {
                    if (command.EmployeeId is null)
                    {
                        var removeExistingLines = ctx.TrnPayrollOtherIncomeLines.Where(x => x.PayrollOtherIncomeId == command.POIId);
                        ctx.TrnPayrollOtherIncomeLines.RemoveRange(removeExistingLines);
                    }
                    else
                    {
                        var removeExistingLines = ctx.TrnPayrollOtherIncomeLines.Where(x => x.PayrollOtherIncomeId == command.POIId && x.EmployeeId == command.EmployeeId);
                        ctx.TrnPayrollOtherIncomeLines.RemoveRange(removeExistingLines);
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
                        var selectedOtherIncomes = command?.SelectedOtherIncomes ?? new List<Dtos.TmpOtherIncome>();

                        foreach (var item in selectedOtherIncomes.Where(x => x.IsSelected))
                        {
                            var podLine = new TrnPayrollOtherIncomeLine()
                            {
                                PayrollOtherIncomeId = command?.POIId ?? 0,
                                EmployeeId = empId,
                                OtherIncomeId = item.Id,
                                Amount = item.Amount,
                            };

                            await ctx.TrnPayrollOtherIncomeLines.AddAsync(podLine);
                        }
                    }

                    await ctx.SaveChangesAsync();
                }
            }
    }
}
