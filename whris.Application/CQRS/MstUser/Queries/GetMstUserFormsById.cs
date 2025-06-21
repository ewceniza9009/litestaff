using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.MstUserForms.Queries
{
    public class GetMstUserFormsById : IRequest<List<MstUserFormDto>>
    {
        public int? Id { get; set; }

        public class GetMstUserFormsByIdHandler : IRequestHandler<GetMstUserFormsById, List<MstUserFormDto>>
        {
            private readonly HRISContext _context;
            public GetMstUserFormsByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<List<MstUserFormDto>> Handle(GetMstUserFormsById request, CancellationToken cancellationToken)
            {
                var mappingProfile = new MappingProfileForMstUserForms();

                var userForms = await _context.MstUserForms
                    .Where(x => x.UserId == request.Id)
                    .ToListAsync();

                var result = userForms.Select(mappingProfile.mapper.Map<MstUserForm, MstUserFormDto>).ToList();

                foreach (var item in result) 
                {
                    item.Id = 0;
                    item.UserId = 0;
                }

                return result;
            }
        }
    }
}
