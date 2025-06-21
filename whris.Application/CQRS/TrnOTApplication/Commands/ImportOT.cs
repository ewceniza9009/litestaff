using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Commands
{
    public class ImportLeave : IRequest<int>
    {
        public int Id { get; set; }
        public List<TmpImportOvertime>? TmpOvertimeImports { get; set; }

        public class ImportOTHandler : IRequestHandler<ImportLeave, int>
        {
            private readonly HRISContext _context;
            public ImportOTHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(ImportLeave command, CancellationToken cancellationToken)
            {
                var trnOvertime = _context.TrnOverTimes.FirstOrDefault(x => x.Id == command.Id);

                if (trnOvertime is not null) 
                {
                    foreach (var otImport in (command?.TmpOvertimeImports ?? new List<TmpImportOvertime>()))
                    {
                        if (otImport.OvertimeHours > 0) 
                        {
                            trnOvertime.TrnOverTimeLines.Add(new Data.Models.TrnOverTimeLine
                            {
                                EmployeeId = otImport.EmployeeId,
                                Date = otImport.Date,
                                OvertimeHours = otImport.OvertimeHours,
                                OvertimeNightHours = otImport.OvertimeNightHours,
                                OvertimeLimitHours = otImport.OvertimeLimitHours,
                                Remarks = otImport.Remarks
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return await Task.Run(() => 0);
            }
        }
    }
}
