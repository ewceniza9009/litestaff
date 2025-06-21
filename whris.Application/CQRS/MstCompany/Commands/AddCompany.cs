using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstCompany.Commands
{
    public class AddCompany : IRequest<MstCompanyDetailDto>
    {
        public class AddCompanyHandler : IRequestHandler<AddCompany, MstCompanyDetailDto>
        {
            private readonly HRISContext _context;
            public AddCompanyHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<MstCompanyDetailDto> Handle(AddCompany command, CancellationToken cancellationToken)
            {
                var newCompany = new MstCompanyDetailDto()
                {
                    Id = 0,
                    Company = "NA",
                    Address = "NA",
                    Sssnumber = "NA",
                    Phicnumber = "NA",
                    Hdmfnumber = "NA",
                    Tin = "NA",
                    IsLocked = true
                };

                return await Task.Run(() => newCompany);
            }
        }
    }
}
