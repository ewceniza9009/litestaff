using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Queries
{
    public class GetMstEmployeeMemoById : IRequest<MstEmployeeMemoDto>
    {
        public int? Id { get; set; }

        public class GetMstEmployeeMemoByIdHandler : IRequestHandler<GetMstEmployeeMemoById, MstEmployeeMemoDto>
        {
            private readonly HRISContext _context;
            public GetMstEmployeeMemoByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstEmployeeMemoDto> Handle(GetMstEmployeeMemoById request, CancellationToken cancellationToken)
            {
                var result = await _context.MstEmployeeMemos
                    .Select(x => new MstEmployeeMemoDto()
                    {
                        Id = x.Id,
                        EmployeeId = x.EmployeeId,
                        MemoDate = x.MemoDate,
                        MemoSubject = x.MemoSubject,
                        MemoContent = x.MemoContent,
                        PreparedBy = x.PreparedBy,
                        ApprovedBy = x.ApprovedBy,
                        FilePath = x.FilePath
                    })
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                return result ?? new MstEmployeeMemoDto() ;
            }
        }
    }
}
