using MediatR;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherIncome.Commands
{
    public class SavePayrollOtherIncome : IRequest<int>
    {
        public TrnPayrollOtherIncomeDetailDto? PayrollOtherIncome { get; set; }

        public class SavePayrollOtherIncomeHandler : IRequestHandler<SavePayrollOtherIncome, int>
        {
            private readonly HRISContext _context;
            public SavePayrollOtherIncomeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SavePayrollOtherIncome command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var poiNumber = "NA";

                if (command?.PayrollOtherIncome?.Poinumber == "NA") 
                {
                    var strPayrollOtherIncomeNumber = _context.TrnPayrollOtherIncomes
                        .Where(x => x.Poinumber != "NA" && x.PeriodId == command.PayrollOtherIncome.PeriodId)
                        .Max(x => x.Poinumber)
                        ?? "NA";

                    var period = _context.MstPeriods
                        .FirstOrDefault(x => x.Id == command.PayrollOtherIncome.PeriodId)
                        ?.Period;

                    if (strPayrollOtherIncomeNumber == "NA")
                    {
                        poiNumber = $"{period}-000001";
                    }
                    else 
                    {
                        var trimmedStrNumber = Int64.Parse(strPayrollOtherIncomeNumber.Substring(5, 6)) + 1_000_001;

                        poiNumber = $"{period}-{trimmedStrNumber.ToString().Substring(1, 6)}";
                    }

                    command.PayrollOtherIncome.Poinumber = poiNumber;
                }

                var newPayrollOtherIncome = command?.PayrollOtherIncome ?? new TrnPayrollOtherIncomeDetailDto();
                newPayrollOtherIncome.TrnPayrollOtherIncomeLines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newPayrollOtherIncome);

                var mappingProfile = new MappingProfileForTrnPayrollOtherIncomeDetailReverse();

                if (newPayrollOtherIncome.Id == 0)
                {
                    var addedPayrollOtherIncome = mappingProfile.mapper.Map<Data.Models.TrnPayrollOtherIncome>(newPayrollOtherIncome);
                    await _context.TrnPayrollOtherIncomes.AddAsync(addedPayrollOtherIncome ?? new Data.Models.TrnPayrollOtherIncome());

                    await _context.SaveChangesAsync();

                    resultId = addedPayrollOtherIncome?.Id ?? 0;
                }
                else 
                {
                    var oldPayrollOtherIncome = await _context.TrnPayrollOtherIncomes.FindAsync(command?.PayrollOtherIncome?.Id ?? 0);
                    mappingProfile.mapper.Map(newPayrollOtherIncome, oldPayrollOtherIncome);

                    await _context.SaveChangesAsync();

                    resultId = oldPayrollOtherIncome?.Id ?? 0;
                }

                var deletedPayrollOtherIncomeFormIds = newPayrollOtherIncome.TrnPayrollOtherIncomeLines.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedPayrollOtherIncomeFormRange = _context.TrnPayrollOtherIncomeLines.Where(x => deletedPayrollOtherIncomeFormIds.Contains(x.Id)).ToList();

                _context.TrnPayrollOtherIncomeLines.RemoveRange(deletedPayrollOtherIncomeFormRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
