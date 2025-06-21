using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using whris.Application.Common;

namespace whris.Application.CQRS.MstEmployee.Queries
{
    public class GetMstEmployeeById : IRequest<MstEmployeeDetailDto>
    {
        public int? Id { get; set; }

        public class GetMstEmployeeByIdHandler : IRequestHandler<GetMstEmployeeById, MstEmployeeDetailDto>
        {
            private readonly HRISContext _context;
            public GetMstEmployeeByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<MstEmployeeDetailDto> Handle(GetMstEmployeeById request, CancellationToken cancellationToken)
            {
                var employee = await _context.MstEmployees
                    .Include(x => x.MstEmployeeMemos)
                    .Include(x => x.MstEmployeeShiftCodes)
                    .Include(x => x.MstEmployeeSalaryHistories)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new MstEmployeeDetailDto();

                if (employee is not null) 
                {
                    employee.OldPictureFilePath = employee.PictureFilePath;

                    await _context.SaveChangesAsync();

                    if(employee.PictureFilePath is null) 
                    {
                        employee.PictureFilePath = "noimage.jpg";
                    }
                }

                var mappingProfile = new MappingProfileForMstEmployeeDetail();
                mappingProfile.mapper.Map(employee, result);

                var employeeShiftCodeService = new EmployeeShiftCodeService();
                var empShiftCodes = await employeeShiftCodeService.GetByEmployeeIdAsync(result.Id);

                if (empShiftCodes.Count == 0)
                {
                    await employeeShiftCodeService.AddOrUpdateByEmployeeIdAsync(result.MstEmployeeShiftCodes);
                }

                empShiftCodes = await employeeShiftCodeService.GetByEmployeeIdAsync(result.Id);

                result.MstEmployeeShiftCodes = new List<MstEmployeeShiftCodeDto>();

                foreach (var shiftCode in empShiftCodes) 
                {
                    result.MstEmployeeShiftCodes.Add(shiftCode);
                }

                return result;
            }
        }
    }
}
