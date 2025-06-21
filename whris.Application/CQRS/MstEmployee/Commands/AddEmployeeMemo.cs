using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Commands
{
    public class AddEmployeeMemo : IRequest<MstEmployeeMemoDto>
    {
        public int EmployeeId { get; set; }

        public class AddEmployeeMemoHandler : IRequestHandler<AddEmployeeMemo, MstEmployeeMemoDto>
        {
            private readonly HRISContext _context;

            public AddEmployeeMemoHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<MstEmployeeMemoDto> Handle(AddEmployeeMemo command, CancellationToken cancellationToken)
            {
                var result = new MstEmployeeMemoDto()
                {
                    Id = 0,
                    EmployeeId = command.EmployeeId,
                    MemoDate = DateTime.Now.Date,                    
                    MemoSubject = "NA",
                    MemoContent = "NA",
                    PreparedBy = 1,
                    ApprovedBy = 1,
                    FilePath = "",
                };

                return await Task.Run(() => result);
            }
        }
    }
}
