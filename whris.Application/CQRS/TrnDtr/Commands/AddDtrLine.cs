using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class AddDtrLine : IRequest<TrnDtrLineDto>
    {
        public int DtrId { get; set; }

        public class AddDtrLineHandler : IRequestHandler<AddDtrLine, TrnDtrLineDto>
        {
            private readonly HRISContext _context;

            public AddDtrLineHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnDtrLineDto> Handle(AddDtrLine command, CancellationToken cancellationToken)
            {
                var result = new TrnDtrLineDto()
                {
                    Id = 0,
                    Dtrid = command.DtrId,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    ShiftCodeId = _context.MstShiftCodes.FirstOrDefault()?.Id ?? 0,
                    Date = DateTime.Now,
                    DayTypeId = _context.MstDayTypes.FirstOrDefault()?.Id ?? 0,
                    DayMultiplier = 1,
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
