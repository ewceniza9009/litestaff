using whris.Application.CQRS.TrnOTApplication.Commands;
using whris.Application.Dtos;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Library
{
    public class Overtime
    {
        internal static async Task EncodeOTApplicationLines(AddOTApplicationsByQuickEncode command)
        {
            using (var ctx = new HRISContext())
            {
                if (command.EmployeeId is null)
                {
                    var lines = ctx.TrnOverTimeLines
                        .Where(x => x.OverTimeId == command.OTId &&
                            x.Date.Date >= command.DateStart.Date &&
                            x.Date <= command.DateEnd.Date);

                    ctx.TrnOverTimeLines.RemoveRange(lines);
                }
                else
                {
                    var lines = ctx.TrnOverTimeLines
                        .Where(x => x.OverTimeId == command.OTId &&
                            x.EmployeeId == command.EmployeeId &&
                            x.Date.Date >= command.DateStart.Date &&
                            x.Date <= command.DateEnd.Date);

                    ctx.TrnOverTimeLines.RemoveRange(lines);
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
                    for (var otDate = command.DateStart; otDate <= command.DateEnd; otDate = otDate.AddDays(1))
                    {
                        var otLine = new TrnOverTimeLine()
                        {
                            OverTimeId = command.OTId,
                            EmployeeId = empId,
                            Date = otDate,
                            OvertimeHours = command.OverTimeHours,
                            OvertimeLimitHours = command.OvertimeLimitHours
                        };

                        await ctx.TrnOverTimeLines.AddAsync(otLine);
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}
