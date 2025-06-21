using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstShiftCode.Commands
{
    public class AddShiftCodeDay : IRequest<MstShiftCodeDayDto>
    {
        public int ShiftCodeId { get; set; }

        public class AddShiftCodeDayHandler : IRequestHandler<AddShiftCodeDay, MstShiftCodeDayDto>
        {
            private readonly HRISContext _context;

            public AddShiftCodeDayHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstShiftCodeDayDto> Handle(AddShiftCodeDay command, CancellationToken cancellationToken)
            {
                var result = new MstShiftCodeDayDto()
                {
                    Id = 0,
                    ShiftCodeId = command.ShiftCodeId,
                    RestDay = false,
                    Day = "Monday",
                    TimeIn1 = DateTime.Now,
                    TimeOut1 = null,
                    TimeIn2 = null,
                    TimeOut2 = DateTime.Now,
                    NumberOfHours = 8,
                    LateFlexibility = 0,
                    LateGraceMinute = 0,
                    NightHours = 0,
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
