using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.EntityFrameworkCore;
using whris.Application.CQRS.TrnLeaveApplication.Commands;
using whris.Application.Dtos;
using whris.Data.Data;
using whris.Data.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace whris.Application.Library
{
    public class Leave
    {
        internal static async Task EncodeLeaveApplicationLines(AddLeaveApplicationsByQuickEncode command)
        {
            using (var ctx = new HRISContext())
            {
                if (command.EmployeeId is null)
                {
                    var lines = ctx.TrnLeaveApplicationLines
                        .Where(x => x.LeaveApplicationId == command.LAId &&
                            x.Date.Date >= command.DateStart.Date &&
                            x.Date <= command.DateEnd.Date);

                    ctx.TrnLeaveApplicationLines.RemoveRange(lines);
                }
                else
                {
                    var lines = ctx.TrnLeaveApplicationLines
                        .Where(x => x.LeaveApplicationId == command.LAId &&
                            x.EmployeeId == command.EmployeeId &&
                            x.Date.Date >= command.DateStart.Date &&
                            x.Date <= command.DateEnd.Date);

                    ctx.TrnLeaveApplicationLines.RemoveRange(lines);
                }

                ctx.SaveChanges();

                var empIds = ctx.MstEmployees
                    .Where(x => x.Id == command.EmployeeId)
                    .Select(x => x.Id)
                    .ToList();

                if (command.EmployeeId is null)
                {
                    empIds = ctx.MstEmployees
                        .Where(x => x.PayrollGroupId == command.PayrollGroupId && x.IsLocked)
                        .Select(x => x.Id)
                        .ToList();
                }

                foreach (var empId in empIds)
                {
                    for (var laDate = command.DateStart; laDate <= command.DateEnd; laDate = laDate.AddDays(1))
                    {
                        var laLine = new TrnLeaveApplicationLine()
                        {
                            LeaveApplicationId = command.LAId,
                            EmployeeId = empId,
                            LeaveId = command.LeaveId,
                            Date = laDate,
                            NumberOfHours = command.NumberOfHours,
                            WithPay = command.WithPay,
                            DebitToLedger = command.DebitToLedger,
                            Remarks = command.Remarks
                        };

                        await ctx.TrnLeaveApplicationLines.AddAsync(laLine);
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task LeaveLedgerLA(int laId, HRISContext ctx) 
        {
            var period = await ctx.TrnLeaveApplications
                .FirstOrDefaultAsync(x => x.Id == laId);

            var periodId = period?.PeriodId ?? 0;
            var debit = 0m;
            var credit = 0m;
            var leave = new MstLeave();

            var deleteLeaveLedgers = ctx.Database.ExecuteSqlRaw($"DELETE FROM TrnLeaveLedger WHERE LeaveApplicationId={laId}");
            ctx.SaveChanges();

            var leaveApplicationLines = await ctx.TrnLeaveApplicationLines
                .Where(x => x.LeaveApplicationId == laId)
                .ToListAsync();

            foreach (var line in leaveApplicationLines)
            {
                if (line.NumberOfHours > 0)
                {
                    if (line.DebitToLedger)
                    {
                        debit = line.NumberOfHours;
                        credit = 0;
                    }
                    else
                    {
                        debit = 0;
                        credit = line.NumberOfHours;
                    }

                    leave = await ctx.MstLeaves
                        .FirstOrDefaultAsync(x => x.Id == line.LeaveId);

                    var newLeaveLedger = new TrnLeaveLedger();

                    newLeaveLedger.PeriodId = periodId;
                    newLeaveLedger.EmployeeId = line.EmployeeId;
                    newLeaveLedger.Date = line.Date;
                    newLeaveLedger.LeaveId = line.LeaveId;
                    newLeaveLedger.LeaveType = leave?.LeaveType ?? string.Empty;
                    newLeaveLedger.Debit = debit;
                    newLeaveLedger.Credit = credit;
                    newLeaveLedger.LeaveApplicationId = laId;

                    await ctx.TrnLeaveLedgers.AddAsync(newLeaveLedger);
                }
            }

            await ctx.SaveChangesAsync();

            var employees = await ctx.MstEmployees
                .Where(x => x.IsLocked)
                .ToListAsync();

            foreach (var emp in employees)
            {
                emp.LeaveBalance = await ctx.TrnLeaveLedgers
                    .Where(x => x.EmployeeId == emp.Id)
                    .SumAsync(x => x.Debit - x.Credit);
            }

            await ctx.SaveChangesAsync();
        }
    }
}
