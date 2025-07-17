using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Commands
{
    public class ImportShiftCode : IRequest<int>
    {
        public required List<TmpImportShiftCode>? TmpImportShiftCodes { get; set; }
        public int Id { get; set; } 

        public class ImportShiftCodeHandler : IRequestHandler<ImportShiftCode, int>
        {
            private readonly HRISContext _context;
            public ImportShiftCodeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(ImportShiftCode command, CancellationToken cancellationToken)
            {
                var trnChangeShiftCode = _context.TrnChangeShifts.FirstOrDefault(x => x.Id == command.Id);
                if (trnChangeShiftCode is not null)
                {
                    foreach (var shiftCodeImport in (command?.TmpImportShiftCodes ?? new List<TmpImportShiftCode>()))
                    {
                        var shiftCodeEntity = _context.MstShiftCodes.FirstOrDefault(x => x.ShiftCode == shiftCodeImport.ShiftCode);
                        if (shiftCodeEntity == null)
                        {
                            throw new InvalidOperationException($"ShiftCode '{shiftCodeImport.ShiftCode}' not found in the database.");
                        }

                        trnChangeShiftCode.TrnChangeShiftLines.Add(new Data.Models.TrnChangeShiftLine
                        {
                            EmployeeId = shiftCodeImport.EmployeeId,
                            Date = shiftCodeImport.Date,
                            ShiftCode = shiftCodeEntity,
                            Remarks = shiftCodeImport.Remarks,
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return await Task.Run(() => 0);
            }
        }
    }
}