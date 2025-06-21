using MediatR;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstShiftCode.Commands
{
    public class SaveShiftCode : IRequest<int>
    {
        public MstShiftCodeDetailDto? ShiftCode { get; set; }

        public class SaveShiftCodeHandler : IRequestHandler<SaveShiftCode, int>
        {
            private readonly HRISContext _context;
            public SaveShiftCodeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveShiftCode command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var newShiftCode = command?.ShiftCode ?? new MstShiftCodeDetailDto();
                newShiftCode.MstShiftCodeDays.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newShiftCode);

                var mappingProfile = new MappingProfileForMstShiftCodeDetailReverse();

                if (newShiftCode.Id == 0)
                {
                    var addedShiftCode = mappingProfile.mapper.Map<Data.Models.MstShiftCode>(newShiftCode);
                    await _context.MstShiftCodes.AddAsync(addedShiftCode ?? new Data.Models.MstShiftCode());

                    await _context.SaveChangesAsync();

                    resultId = addedShiftCode?.Id ?? 0;
                }
                else 
                {
                    var oldShiftCode = _context.MstShiftCodes.Find(command?.ShiftCode?.Id ?? 0);
                    mappingProfile.mapper.Map(newShiftCode, oldShiftCode);

                    await _context.SaveChangesAsync();

                    resultId = oldShiftCode?.Id ?? 0;
                }

                var deletedShiftCodeIds = newShiftCode.MstShiftCodeDays.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedShiftCodeRange = _context.MstShiftCodeDays.Where(x => deletedShiftCodeIds.Contains(x.Id)).ToList();

                _context.MstShiftCodeDays.RemoveRange(deletedShiftCodeRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
