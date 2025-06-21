using MediatR;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstMstOtherIncome.Commands
{
    public class SaveMstOtherIncome : IRequest<int>
    {
        public List<MstOtherIncomeDto>? MstOtherIncomes { get; set; }

        public class SaveMstOtherIncomeHandler : IRequestHandler<SaveMstOtherIncome, int>
        {
            private readonly HRISContext _context;
            public SaveMstOtherIncomeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveMstOtherIncome command, CancellationToken cancellationToken)
            {
                var mappingProfileOtherIncome = new MappingProfile<MstOtherIncomeDto, Data.Models.MstOtherIncome>();

                foreach (var item in command.MstOtherIncomes ?? new List<MstOtherIncomeDto>()) 
                {
                    var oldItem = _context.MstOtherIncomes
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new Data.Models.MstOtherIncome();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstOtherIncomes.AddRangeAsync(mappingProfileOtherIncome.mapper.Map(item, new Data.Models.MstOtherIncome()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileOtherIncome.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstOtherIncomes.Remove(oldItem);
                }
                
                await _context.SaveChangesAsync();

                return 0;
            }
        }
    }
}
