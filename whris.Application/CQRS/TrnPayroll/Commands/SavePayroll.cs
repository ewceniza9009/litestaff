using MediatR;
using System.Security.Cryptography.X509Certificates;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnPayroll.Commands
{
    public class SavePayroll : IRequest<int>
    {
        public TrnPayrollDetailDto? Payroll { get; set; }

        public class SavePayrollHandler : IRequestHandler<SavePayroll, int>
        {
            private readonly HRISContext _context;
            public SavePayrollHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SavePayroll command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var dtrNumber = "NA";

                if (command?.Payroll?.PayrollNumber == "NA") 
                {
                    var strPayrollNumber = _context.TrnPayrolls
                        .Where(x => x.PayrollNumber != "NA" && x.PeriodId == command.Payroll.PeriodId)
                        .Max(x => x.PayrollNumber)
                        ?? "NA";

                    var period = _context.MstPeriods
                        .FirstOrDefault(x => x.Id == command.Payroll.PeriodId)
                        ?.Period;

                    if (strPayrollNumber == "NA")
                    {
                        dtrNumber = $"{period}-000001";
                    }
                    else 
                    {
                        var trimmedStrNumber = Int64.Parse(strPayrollNumber.Substring(5, 6)) + 1_000_001;

                        dtrNumber = $"{period}-{trimmedStrNumber.ToString().Substring(1, 6)}";
                    }

                    command.Payroll.PayrollNumber = dtrNumber;
                }

                var newPayroll = command?.Payroll ?? new TrnPayrollDetailDto();
                newPayroll.TrnPayrollLines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newPayroll);

                var mappingProfile = new MappingProfileForTrnPayrollDetailReverse();

                if (newPayroll.Id == 0)
                {
                    var addedPayroll = mappingProfile.mapper.Map<Data.Models.TrnPayroll>(newPayroll);
                    await _context.TrnPayrolls.AddAsync(addedPayroll ?? new Data.Models.TrnPayroll());

                    await _context.SaveChangesAsync();

                    resultId = addedPayroll?.Id ?? 0;
                }
                else 
                {
                    var oldPayroll = await _context.TrnPayrolls.FindAsync(command?.Payroll?.Id ?? 0);
                    mappingProfile.mapper.Map(newPayroll, oldPayroll);

                    await _context.SaveChangesAsync();

                    resultId = oldPayroll?.Id ?? 0;
                }

                var deletedPayrollFormIds = newPayroll.TrnPayrollLines.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedPayrollFormRange = _context.TrnPayrollLines.Where(x => deletedPayrollFormIds.Contains(x.Id)).ToList();

                _context.TrnPayrollLines.RemoveRange(deletedPayrollFormRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
