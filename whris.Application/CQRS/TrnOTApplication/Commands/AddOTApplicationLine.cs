using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Commands
{
    public class AddOTApplicationLine : IRequest<TrnOTApplicationLineDto>
    {
        public int OTApplicationId { get; set; }

        public class AddOTApplicationLineHandler : IRequestHandler<AddOTApplicationLine, TrnOTApplicationLineDto>
        {
            private readonly HRISContext _context;

            public AddOTApplicationLineHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnOTApplicationLineDto> Handle(AddOTApplicationLine command, CancellationToken cancellationToken)
            {
                var result = new TrnOTApplicationLineDto()
                {
                    Id = 0,
                    OverTimeId = command.OTApplicationId,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    Date = DateTime.Now,
                    OvertimeHours = 8,
                    OvertimeRate =  0,
                    OvertimeAmount = 0,
                    OvertimeLimitHours = 8,
                    Remarks = "NA",
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
