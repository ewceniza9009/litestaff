using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstDayType.Commands
{
    public class AddDayType : IRequest<MstDayTypeDetailDto>
    {
        public class AddDayTypeHandler : IRequestHandler<AddDayType, MstDayTypeDetailDto>
        {
            private readonly HRISContext _context;
            public AddDayTypeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<MstDayTypeDetailDto> Handle(AddDayType command, CancellationToken cancellationToken)
            {
                var newDayType = new MstDayTypeDetailDto()
                {
                    Id = 0,
                    DayType = "NA",
                    WorkingDays = 5,
                    RestdayDays = 2,
                    IsLocked = true
                };

                return await Task.Run(() => newDayType);
            }
        }
    }
}
