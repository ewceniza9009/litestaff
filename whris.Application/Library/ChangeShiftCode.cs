using whris.Application.CQRS.TrnChangeShiftCode.Commands;
using whris.Application.Dtos;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Library
{
    public class ChangeShift
    {
        internal static async Task EncodeChangeShiftApplicationLines(AddChangeShiftCodesByQuickEncode command)
        {
            using (var ctx = new HRISContext())
            {
                if (command.EmployeeId is null)
                {
                    var lines = ctx.TrnChangeShiftLines
                        .Where(x => x.ChangeShiftId == command.CSId &&
                            x.Date.Date >= command.DateStart.Date &&
                            x.Date <= command.DateEnd.Date);

                    ctx.TrnChangeShiftLines.RemoveRange(lines);
                }
                else
                {
                    var lines = ctx.TrnChangeShiftLines
                        .Where(x => x.ChangeShiftId == command.CSId &&
                            x.EmployeeId == command.EmployeeId &&
                            x.Date.Date >= command.DateStart.Date &&
                            x.Date <= command.DateEnd.Date);

                    ctx.TrnChangeShiftLines.RemoveRange(lines);
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
                    for (var csDate = command.DateStart; csDate <= command.DateEnd; csDate = csDate.AddDays(1))
                    {
                        var csLine = new TrnChangeShiftLine()
                        {
                            ChangeShiftId = command.CSId,
                            EmployeeId = empId,
                            ShiftCodeId = command.ShiftCodeId,
                            Date = csDate,
                            Remarks = "NA"
                        };

                        await ctx.TrnChangeShiftLines.AddAsync(csLine);
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}
