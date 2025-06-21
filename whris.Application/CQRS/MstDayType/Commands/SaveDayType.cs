using MediatR;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstDayType.Commands
{
    public class SaveDayType : IRequest<int>
    {
        public MstDayTypeDetailDto? DayType { get; set; }

        public class SaveDayTypeHandler : IRequestHandler<SaveDayType, int>
        {
            private readonly HRISContext _context;
            public SaveDayTypeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveDayType command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var newDayType = command?.DayType ?? new MstDayTypeDetailDto();
                newDayType.MstDayTypeDays.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newDayType);

                var mappingProfile = new MappingProfileForMstDayTypeDetailReverse();

                if (newDayType.Id == 0)
                {
                    var addedDayType = mappingProfile.mapper.Map<Data.Models.MstDayType>(newDayType);
                    await _context.MstDayTypes.AddAsync(addedDayType ?? new Data.Models.MstDayType());

                    await _context.SaveChangesAsync();

                    resultId = addedDayType?.Id ?? 0;
                }
                else 
                {
                    var oldDayType = await _context.MstDayTypes.FindAsync(command?.DayType?.Id ?? 0);
                    mappingProfile.mapper.Map(newDayType, oldDayType);

                    await _context.SaveChangesAsync();

                    resultId = oldDayType?.Id ?? 0;
                }

                var deletedDayTypeIds = newDayType.MstDayTypeDays.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedDayTypeRange = _context.MstDayTypeDays.Where(x => deletedDayTypeIds.Contains(x.Id)).ToList();

                _context.MstDayTypeDays.RemoveRange(deletedDayTypeRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
