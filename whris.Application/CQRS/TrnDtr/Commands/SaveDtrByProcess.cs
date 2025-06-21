using MediatR;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class SaveDtrByProcess : IRequest<int>
    {
        public TrnDtrDetailDto? DtrDetail { get; set; }

        public class SaveDtrByProcessHandler : IRequestHandler<SaveDtrByProcess, int>
        {
            private readonly HRISContext _context;
            public SaveDtrByProcessHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveDtrByProcess command, CancellationToken cancellationToken)
            {

                var newDtr = command?.DtrDetail ?? new TrnDtrDetailDto();
                newDtr.TrnDtrlines.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newDtr);

                var mappingProfile = new MappingProfileForTrnDtrDetailReverse();

                if (newDtr.Id == 0)
                {
                    var addedDtr = mappingProfile.mapper.Map<Data.Models.TrnDtr>(newDtr);
                    await _context.TrnDtrs.AddAsync(addedDtr ?? new Data.Models.TrnDtr());
                }
                else
                {
                    var oldDtr = _context.TrnDtrs.Find(command?.DtrDetail?.Id ?? 0);
                    mappingProfile.mapper.Map(newDtr, oldDtr);
                }

                await _context.SaveChangesAsync();

                var deletedDtrLines = newDtr.TrnDtrlines.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedEmpMemoRange = _context.TrnDtrlines.Where(x => deletedDtrLines.Contains(x.Id)).ToList();
                _context.TrnDtrlines.RemoveRange(deletedEmpMemoRange);

                await _context.SaveChangesAsync();

                return await Task.Run(() => 0);
            }
        }
    }
}
