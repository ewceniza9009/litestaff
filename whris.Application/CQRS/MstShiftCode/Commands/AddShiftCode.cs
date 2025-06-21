using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstShiftCode.Commands
{
    public class AddShiftCode : IRequest<MstShiftCodeDetailDto>
    {
        public class AddShiftCodeHandler : IRequestHandler<AddShiftCode, MstShiftCodeDetailDto>
        {
            private readonly HRISContext _context;
            public AddShiftCodeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<MstShiftCodeDetailDto> Handle(AddShiftCode command, CancellationToken cancellationToken)
            {
                var newShiftCode = new MstShiftCodeDetailDto()
                {
                    Id = 0,
                    ShiftCode = "NA",
                    Remarks = "NA",
                    IsLocked = true,
                    MstShiftCodeDays = new List<MstShiftCodeDayDto>() 
                    {
                        new MstShiftCodeDayDto()
                        {
                            ShiftCodeId = 0,   
                            RestDay = true,   
                            Day = "Sunday",   
                            TimeIn1 = null,
                            TimeOut1 = null,   
                            TimeIn2 = null,   
                            TimeOut2 = null,   
                            NumberOfHours = 8,   
                            LateFlexibility = 0,   
                            LateGraceMinute = 0,   
                            NightHours = 0,   
                        },
                        new MstShiftCodeDayDto()
                        {
                            ShiftCodeId = 0,
                            RestDay = false,
                            Day = "Monday",
                            TimeIn1 = null,
                            TimeOut1 = null,
                            TimeIn2 = null,
                            TimeOut2 = null,
                            NumberOfHours = 8,
                            LateFlexibility = 0,
                            LateGraceMinute = 0,
                            NightHours = 0,
                        },
                        new MstShiftCodeDayDto()
                        {
                            ShiftCodeId = 0,
                            RestDay = false,
                            Day = "Tuesday",
                            TimeIn1 = null,
                            TimeOut1 = null,
                            TimeIn2 = null,
                            TimeOut2 = null,
                            NumberOfHours = 8,
                            LateFlexibility = 0,
                            LateGraceMinute = 0,
                            NightHours = 0,
                        },
                        new MstShiftCodeDayDto()
                        {
                            ShiftCodeId = 0,
                            RestDay = false,
                            Day = "Wednesday",
                            TimeIn1 = null,
                            TimeOut1 = null,
                            TimeIn2 = null,
                            TimeOut2 = null,
                            NumberOfHours = 8,
                            LateFlexibility = 0,
                            LateGraceMinute = 0,
                            NightHours = 0,
                        },
                        new MstShiftCodeDayDto()
                        {
                            ShiftCodeId = 0,
                            RestDay = false,
                            Day = "Thursday",
                            TimeIn1 = null,
                            TimeOut1 = null,
                            TimeIn2 = null,
                            TimeOut2 = null,
                            NumberOfHours = 8,
                            LateFlexibility = 0,
                            LateGraceMinute = 0,
                            NightHours = 0,
                        },
                        new MstShiftCodeDayDto()
                        {
                            ShiftCodeId = 0,
                            RestDay = false,
                            Day = "Friday",
                            TimeIn1 = null,
                            TimeOut1 = null,
                            TimeIn2 = null,
                            TimeOut2 = null,
                            NumberOfHours = 8,
                            LateFlexibility = 0,
                            LateGraceMinute = 0,
                            NightHours = 0,
                        },
                        new MstShiftCodeDayDto()
                        {
                            ShiftCodeId = 0,
                            RestDay = false,
                            Day = "Saturday",
                            TimeIn1 = null,
                            TimeOut1 = null,
                            TimeIn2 = null,
                            TimeOut2 = null,
                            NumberOfHours = 8,
                            LateFlexibility = 0,
                            LateGraceMinute = 0,
                            NightHours = 0,
                        }
                    }
                };

                return await Task.Run(() => newShiftCode);
            }
        }
    }
}
