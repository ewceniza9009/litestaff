using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstCompany.Commands
{
    public class AddBranch : IRequest<MstBranchDto>
    {
        public int CompanyId { get; set; }

        public class AddBranchHandler : IRequestHandler<AddBranch, MstBranchDto>
        {
            private readonly HRISContext _context;

            public AddBranchHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstBranchDto> Handle(AddBranch command, CancellationToken cancellationToken)
            {
                var result = new MstBranchDto()
                {
                    Id = 0,
                    CompanyId = command.CompanyId,
                    Branch = "NA",
                    IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
