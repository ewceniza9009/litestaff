using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOtherDeductionApplication.Commands
{
    public class ImportOtherDeduction : IRequest<int>
    {
        public int Id { get; set; }
        public List<TmpImportOtherDeduction>? TmpOtherDeductionImports { get; set; }

        public class ImportOtherDeductionHandler : IRequestHandler<ImportOtherDeduction, int>
        {
            private readonly HRISContext _context;
            public ImportOtherDeductionHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(ImportOtherDeduction command, CancellationToken cancellationToken)
            {
                var trnOtherDeduction = _context.TrnPayrollOtherDeductions.FirstOrDefault(x => x.Id == command.Id);

                if (trnOtherDeduction is not null) 
                {
                    foreach (var OtherDeductionImport in (command?.TmpOtherDeductionImports ?? new List<TmpImportOtherDeduction>()))
                    {
                        trnOtherDeduction.TrnPayrollOtherDeductionLines.Add(new Data.Models.TrnPayrollOtherDeductionLine 
                        {
                            EmployeeId = OtherDeductionImport.EmployeeId,
                            OtherDeductionId = OtherDeductionImport.OtherDeductionId,
                            Amount = OtherDeductionImport.Amount
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return await Task.Run(() => 0);
            }
        }
    }
}
