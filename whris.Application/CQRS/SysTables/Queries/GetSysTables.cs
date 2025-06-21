using MediatR;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.SysTables.Queries
{
    public class GetSysTables : IRequest<SysTableDto>
    {
        public class GetSysTableHandler : IRequestHandler<GetSysTables, SysTableDto>
        {
            private readonly HRISContext _context;
            public GetSysTableHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<SysTableDto> Handle(GetSysTables request, CancellationToken cancellationToken)
            {
                var result =  new SysTableDto();

                result.MstPayrollGroups = _context.MstPayrollGroups
                    .Select(x => new MappingProfile<MstPayrollGroup, MstPayrollGroupDto>().mapper.Map<MstPayrollGroupDto>(x))
                    .ToList();

                result.MstPayrollTypes = _context.MstPayrollTypes
                    .Select(x => new MappingProfile<MstPayrollType, MstPayrollTypeDto>().mapper.Map<MstPayrollTypeDto>(x))
                    .ToList();

                result.MstDepartments = _context.MstDepartments
                    .Select(x => new MappingProfile<MstDepartment, MstDepartmentDto>().mapper.Map<MstDepartmentDto>(x))
                    .ToList();

                result.MstPositions = _context.MstPositions
                    .Select(x => new MappingProfile<MstPosition, MstPositionDto>().mapper.Map<MstPositionDto>(x))
                    .ToList();

                result.MstAccounts = _context.MstAccounts
                    .Select(x => new MappingProfile<MstAccount, MstAccountDto>().mapper.Map<MstAccountDto>(x))
                    .ToList();

                result.MstReligions = _context.MstReligions
                    .Select(x => new MappingProfile<MstReligion, MstReligionDto>().mapper.Map<MstReligionDto>(x))
                    .ToList();

                result.MstCitizenships = _context.MstCitizenships
                    .Select(x => new MappingProfile<MstCitizenship, MstCitizenshipDto>().mapper.Map<MstCitizenshipDto>(x))
                    .ToList();

                result.MstZipCodes = _context.MstZipCodes
                   .Select(x => new MappingProfile<MstZipCode, MstZipCodeDto>().mapper.Map<MstZipCodeDto>(x))
                   .ToList();

                result.MstDivisions = _context.MstDivisions
                   .Select(x => new MappingProfile<MstDivision, MstDivisionDto>().mapper.Map<MstDivisionDto>(x))
                   .ToList();

                return await Task.Run(() => result);
            }
        }
    }
}
