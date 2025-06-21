using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using static whris.Application.Queries.TrnDtr.GetEmployees;

namespace whris.Application.CQRS.MstCompany.Queries
{
    public class GetMstCompanyById : IRequest<MstCompanyDetailDto>
    {
        public int? Id { get; set; }

        public class GetMstCompanyByIdHandler : IRequestHandler<GetMstCompanyById, MstCompanyDetailDto>
        {
            private readonly HRISContext _context;
            public GetMstCompanyByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstCompanyDetailDto> Handle(GetMstCompanyById request, CancellationToken cancellationToken)
            {
                var company = await _context.MstCompanies
                    .Include(x => x.MstBranches)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new MstCompanyDetailDto();

                if (company is not null)
                {
                    company.OldImageLogo = company.ImageLogo;
                    await _context.SaveChangesAsync();

                    if (company.ImageLogo is null)
                    {
                        company.ImageLogo = "noimage.jpg";
                    }
                }

                var mappingProfile = new MappingProfileForMstCompanyDetail();
                mappingProfile.mapper.Map(company, result);

                return result;
            }
        }
    }
}
