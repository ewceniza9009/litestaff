using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Commands
{
    public class ImportLeave : IRequest<int>
    {
        public int Id { get; set; }
        public List<TmpImportLeave>? TmpLeaveImports { get; set; }

        public class ImportOTHandler : IRequestHandler<ImportLeave, int>
        {
            private readonly HRISContext _context;
            public ImportOTHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(ImportLeave command, CancellationToken cancellationToken)
            {
                var trnLeave = _context.TrnLeaveApplications.FirstOrDefault(x => x.Id == command.Id);

                if (trnLeave is not null) 
                {
                    foreach (var leaveImport in (command?.TmpLeaveImports ?? new List<TmpImportLeave>()))
                    {
                        trnLeave.TrnLeaveApplicationLines.Add(new Data.Models.TrnLeaveApplicationLine 
                        {
                            EmployeeId = leaveImport.EmployeeId,
                            LeaveId = leaveImport.LeaveId,
                            Date = leaveImport.Date,
                            NumberOfHours = leaveImport.NumberOfHours,
                            WithPay = leaveImport.WithPay,
                            DebitToLedger = leaveImport.DebitToLedger,
                            Remarks = leaveImport.Remarks
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return await Task.Run(() => 0);
            }
        }
    }
}
