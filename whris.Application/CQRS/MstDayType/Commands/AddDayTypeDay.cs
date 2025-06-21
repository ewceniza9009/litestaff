using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstDayType.Commands
{
    public class AddDayTypeDay : IRequest<MstDayTypeDayDto>
    {
        public int DayTypeId { get; set; }

        public class AddDayTypeDayHandler : IRequestHandler<AddDayTypeDay, MstDayTypeDayDto>
        {
            private readonly HRISContext _context;

            public AddDayTypeDayHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstDayTypeDayDto> Handle(AddDayTypeDay command, CancellationToken cancellationToken)
            {
                var result = new MstDayTypeDayDto()
                {
                    Id = 0,
                    DayTypeId = command.DayTypeId,
                    BranchId = _context.MstBranches.FirstOrDefault()?.Id ?? 0,
                    Date = DateTime.Now.Date,
                    DateAfter = DateTime.Now.Date,
                    DateBefore = DateTime.Now.Date,
                    ExcludedInFixed = false,
                    Remarks = "NA",
                    WithAbsentInFixed = false,
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
