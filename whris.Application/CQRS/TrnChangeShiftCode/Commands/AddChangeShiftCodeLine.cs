using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnChangeShiftCode.Commands
{
    public class AddChangeShiftCodeLine : IRequest<TrnChangeShiftCodeLineDto>
    {
        public int ChangeShiftCodeId { get; set; }

        public class AddChangeShiftCodeLineHandler : IRequestHandler<AddChangeShiftCodeLine, TrnChangeShiftCodeLineDto>
        {
            private readonly HRISContext _context;

            public AddChangeShiftCodeLineHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnChangeShiftCodeLineDto> Handle(AddChangeShiftCodeLine command, CancellationToken cancellationToken)
            {
                var result = new TrnChangeShiftCodeLineDto()
                {
                    Id = 0,
                    ChangeShiftId = command.ChangeShiftCodeId,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    Date = DateTime.Now,
                    ShiftCodeId = _context.MstShiftCodes.OrderBy(x => x.ShiftCode).FirstOrDefault()?.Id ?? 0,
                    Remarks = "NA",
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
