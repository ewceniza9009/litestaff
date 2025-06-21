using MediatR;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstOtherDeduction.Commands
{
    public class SaveMstOtherDeduction : IRequest<int>
    {
        public List<MstOtherDeductionDto>? MstOtherDeductions { get; set; }

        public class SaveMstOtherDeductionHandler : IRequestHandler<SaveMstOtherDeduction, int>
        {
            private readonly HRISContext _context;
            public SaveMstOtherDeductionHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveMstOtherDeduction command, CancellationToken cancellationToken)
            {
                var mappingProfileOtherDeduction = new MappingProfile<MstOtherDeductionDto, Data.Models.MstOtherDeduction>();

                foreach (var item in command.MstOtherDeductions ?? new List<MstOtherDeductionDto>()) 
                {
                    var oldItem = _context.MstOtherDeductions
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new Data.Models.MstOtherDeduction();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstOtherDeductions.AddRangeAsync(mappingProfileOtherDeduction.mapper.Map(item, new Data.Models.MstOtherDeduction()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileOtherDeduction.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstOtherDeductions.Remove(oldItem);
                }
                
                await _context.SaveChangesAsync();

                return 0;
            }
        }
    }
}
