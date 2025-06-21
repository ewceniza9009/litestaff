using MediatR;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayrollOtherDeduction.Commands
{
    public class SavePayrollOtherDeduction : IRequest<int>
    {
        public TrnPayrollOtherDeductionDetailDto? PayrollOtherDeduction { get; set; }

        public class SavePayrollOtherDeductionHandler : IRequestHandler<SavePayrollOtherDeduction, int>
        {
            private readonly HRISContext _context;
            public SavePayrollOtherDeductionHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SavePayrollOtherDeduction command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var podNumber = "NA";

                if (command?.PayrollOtherDeduction?.Podnumber == "NA") 
                {
                    var strPayrollOtherDeductionNumber = _context.TrnPayrollOtherDeductions
                        .Where(x => x.Podnumber != "NA" && x.PeriodId == command.PayrollOtherDeduction.PeriodId)
                        .Max(x => x.Podnumber)
                        ?? "NA";

                    var period = _context.MstPeriods
                        .FirstOrDefault(x => x.Id == command.PayrollOtherDeduction.PeriodId)
                        ?.Period;

                    if (strPayrollOtherDeductionNumber == "NA")
                    {
                        podNumber = $"{period}-000001";
                    }
                    else 
                    {
                        var trimmedStrNumber = Int64.Parse(strPayrollOtherDeductionNumber.Substring(5, 6)) + 1_000_001;

                        podNumber = $"{period}-{trimmedStrNumber.ToString().Substring(1, 6)}";
                    }

                    command.PayrollOtherDeduction.Podnumber = podNumber;
                }

                var newPayrollOtherDeduction = command?.PayrollOtherDeduction ?? new TrnPayrollOtherDeductionDetailDto();
                newPayrollOtherDeduction.TrnPayrollOtherDeductionLines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newPayrollOtherDeduction);

                var mappingProfile = new MappingProfileForTrnPayrollOtherDeductionDetailReverse();

                if (newPayrollOtherDeduction.Id == 0)
                {
                    var addedPayrollOtherDeduction = mappingProfile.mapper.Map<Data.Models.TrnPayrollOtherDeduction>(newPayrollOtherDeduction);
                    await _context.TrnPayrollOtherDeductions.AddAsync(addedPayrollOtherDeduction ?? new Data.Models.TrnPayrollOtherDeduction());

                    await _context.SaveChangesAsync();

                    resultId = addedPayrollOtherDeduction?.Id ?? 0;
                }
                else 
                {
                    var oldPayrollOtherDeduction = await _context.TrnPayrollOtherDeductions.FindAsync(command?.PayrollOtherDeduction?.Id ?? 0);
                    mappingProfile.mapper.Map(newPayrollOtherDeduction, oldPayrollOtherDeduction);

                    await _context.SaveChangesAsync();

                    resultId = oldPayrollOtherDeduction?.Id ?? 0;
                }

                var podLines = newPayrollOtherDeduction.TrnPayrollOtherDeductionLines.Where(x => x.IsDeleted);

                var loanIds = podLines.Where(x => x.EmployeeLoanId != null).Select(x => x.EmployeeLoanId).ToList();

                var deletedPayrollOtherDeductionFormIds = podLines.Select(x => x.Id).ToList();
                var deletedPayrollOtherDeductionFormRange = _context.TrnPayrollOtherDeductionLines.Where(x => deletedPayrollOtherDeductionFormIds.Contains(x.Id)).ToList();

                _context.TrnPayrollOtherDeductionLines.RemoveRange(deletedPayrollOtherDeductionFormRange);

                await _context.SaveChangesAsync();

                await Library.PayrollOtherDeduction.UpdateDeletedLoansFromPOD(loanIds, _context);
                await Library.PayrollOtherDeduction.UpdateLoanBalance(resultId, _context);

                return resultId;
            }
        }
    }
}
