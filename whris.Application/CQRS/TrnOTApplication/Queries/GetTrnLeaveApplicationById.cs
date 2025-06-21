using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnOTApplication.Queries
{
    public class GetTrnOTApplicationById : IRequest<TrnOTApplicationDetailDto>
    {
        public int? Id { get; set; }

        public class GetTrnOTApplicationByIdHandler : IRequestHandler<GetTrnOTApplicationById, TrnOTApplicationDetailDto>
        {
            private readonly HRISContext _context;
            public GetTrnOTApplicationByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnOTApplicationDetailDto> Handle(GetTrnOTApplicationById request, CancellationToken cancellationToken)
            {
                var otApplication = await _context.TrnOverTimes
                    .Include(x => x.TrnOverTimeLines)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new TrnOTApplicationDetailDto();

                var mappingProfile = new MappingProfileForTrnOTApplicationDetail();
                mappingProfile.mapper.Map(otApplication, result);

                foreach (var line in result.TrnOverTimeLines) 
                {
                    line.OvertimeRate = Lookup.GetEmployeeOvertimeRateById(line.EmployeeId);
                    line.OvertimeAmount = line.OvertimeHours * line.OvertimeRate;
                }

                return result;
            }
        }
    }
}
