using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Application.Mappers;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Commands
{
    public class ApproveEmployeeShiftCodes : IRequest
    {
        public int Id { get; set; }
        public List<MstEmployeeShiftCodeDto> EmployeeCodes { get; set; } = new List<MstEmployeeShiftCodeDto>();

        public class ApproveEmployeeShiftCodesHandler : IRequestHandler<ApproveEmployeeShiftCodes>
        {
            private readonly HRISContext _context;

            public ApproveEmployeeShiftCodesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(ApproveEmployeeShiftCodes request, CancellationToken cancellationToken)
            {
                var employeeShiftCodeService = new EmployeeShiftCodeService();

                var localEmployeeShiftCodes = await employeeShiftCodeService.GetByEmployeeIdAsync(request.Id);

                var localEmployeeShiftCodeIdsToBeDeleted = localEmployeeShiftCodes.Where(x => x.IsDeleted).Select(x => x.Id);
                var localEmployeeShiftCodesToBeAdded = localEmployeeShiftCodes.Where(x => x.Status == "New");
                var localEmployeeShiftCodesToBeModified = localEmployeeShiftCodes.Where(x => x.Status == "Modified");

                var deleteRange = _context.MstEmployeeShiftCodes.Where(x => localEmployeeShiftCodeIdsToBeDeleted.Contains(x.Id));                

                _context.MstEmployeeShiftCodes.RemoveRange(deleteRange);

                foreach (var localEmpShiftCode in localEmployeeShiftCodesToBeAdded) 
                {
                    await _context.MstEmployeeShiftCodes.AddAsync(new Data.Models.MstEmployeeShiftCode
                    {
                        EmployeeId = localEmpShiftCode.EmployeeId,
                        ShiftCodeId = localEmpShiftCode.ShiftCodeId,
                    });
                }

                foreach (var localEmpShiftCode in localEmployeeShiftCodesToBeModified)
                {
                    var empShiftCode = await _context.MstEmployeeShiftCodes.FirstOrDefaultAsync(x => x.Id == localEmpShiftCode.Id);

                    if (empShiftCode is not null) 
                    {
                        empShiftCode.ShiftCodeId = localEmpShiftCode.ShiftCodeId;
                    }
                }

                await _context.SaveChangesAsync();

                var employee = await _context.MstEmployees
                    .Include(x => x.MstEmployeeShiftCodes)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                var result = new MstEmployeeDetailDto();

                var mappingProfile = new MappingProfileForMstEmployeeDetail();
                mappingProfile.mapper.Map(employee, result);

                await employeeShiftCodeService.ApproveEmplyeeShiftCodes(request.Id, result.MstEmployeeShiftCodes);

                return await Task.Run(() => Unit.Value);
            }
        }
    }
}
