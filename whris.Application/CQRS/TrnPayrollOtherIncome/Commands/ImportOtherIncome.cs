using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOtherIncomeApplication.Commands
{
    public class ImportOtherIncome : IRequest<int>
    {
        public int Id { get; set; }
        public List<TmpImportOtherIncome>? TmpOtherIncomeImports { get; set; }

        public class ImportOtherIncomeHandler : IRequestHandler<ImportOtherIncome, int>
        {
            private readonly HRISContext _context;
            public ImportOtherIncomeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(ImportOtherIncome command, CancellationToken cancellationToken)
            {
                var trnOtherIncome = _context.TrnPayrollOtherIncomes.FirstOrDefault(x => x.Id == command.Id);

                if (trnOtherIncome is not null) 
                {
                    foreach (var otherIncomeImport in (command?.TmpOtherIncomeImports ?? new List<TmpImportOtherIncome>()))
                    {
                        trnOtherIncome.TrnPayrollOtherIncomeLines.Add(new Data.Models.TrnPayrollOtherIncomeLine 
                        {
                            EmployeeId = otherIncomeImport.EmployeeId,
                            OtherIncomeId = otherIncomeImport.OtherIncomeId,
                            Amount = otherIncomeImport.Amount
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return await Task.Run(() => 0);
            }
        }
    }
}
