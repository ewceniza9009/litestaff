using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLeaveApplication.Commands
{
    public class AddLeaveApplicationLine : IRequest<TrnLeaveApplicationLineDto>
    {
        public int LeaveApplicationId { get; set; }

        public class AddLeaveApplicationLineHandler : IRequestHandler<AddLeaveApplicationLine, TrnLeaveApplicationLineDto>
        {
            private readonly HRISContext _context;

            public AddLeaveApplicationLineHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnLeaveApplicationLineDto> Handle(AddLeaveApplicationLine command, CancellationToken cancellationToken)
            {
                var result = new TrnLeaveApplicationLineDto()
                {
                    Id = 0,
                    LeaveApplicationId = command.LeaveApplicationId,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    LeaveId = _context.MstLeaves.OrderByDescending(x => x.Leave).FirstOrDefault()?.Id ?? 0,
                    Date = DateTime.Now,
                    NumberOfHours = 8,
                    WithPay = false,
                    DebitToLedger = false,
                    Remarks = "NA",
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
