using MediatR;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Commands
{
    public class SaveEmployee : IRequest<int>
    {
        public MstEmployeeDetailDto? Employee { get; set; }

        public class SaveEmployeeHandler : IRequestHandler<SaveEmployee, int>
        {
            private readonly HRISContext _context;
            public SaveEmployeeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveEmployee command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var newEmployee = command?.Employee ?? new MstEmployeeDetailDto();
                newEmployee.MstEmployeeMemos.RemoveAll(x => x.IsDeleted && x.Id == 0);
                //newEmployee.MstEmployeeShiftCodes.RemoveAll(x => x.IsDeleted && x.Id == 0);

                if (newEmployee.Id != 0) 
                {
                    var employeeShiftCodeService = new EmployeeShiftCodeService();
                    var modifiedEmployeeShiftCodes = newEmployee
                        .MstEmployeeShiftCodes.Where(x => x.Status != "Approved")
                        .ToList();

                    await employeeShiftCodeService.AddOrUpdateByEmployeeIdAsync(modifiedEmployeeShiftCodes, true);
                }

                newEmployee.MstEmployeeShiftCodes = new List<MstEmployeeShiftCodeDto>();

                Utilities.UpdateEntityAuditFields(newEmployee);

                var mappingProfile = new MappingProfileForMstEmployeeDetailReverse();

                if (newEmployee.Id == 0)
                {
                    var addedEmployee = mappingProfile.mapper.Map<Data.Models.MstEmployee>(newEmployee);
                    await _context.MstEmployees.AddAsync(addedEmployee ?? new Data.Models.MstEmployee());

                    await _context.SaveChangesAsync();

                    resultId = addedEmployee?.Id ?? 0;
                }
                else
                {
                    var oldEmployee = await _context.MstEmployees.FindAsync(command?.Employee?.Id ?? 0);

                    if (oldEmployee != null)
                    {
                        newEmployee.MonthlyRate = oldEmployee.MonthlyRate;
                        newEmployee.PayrollRate = oldEmployee.PayrollRate;
                        newEmployee.DailyRate = oldEmployee.DailyRate;
                        newEmployee.AbsentDailyRate = oldEmployee.AbsentDailyRate;
                        newEmployee.HourlyRate = oldEmployee.HourlyRate;
                        newEmployee.NightHourlyRate = oldEmployee.NightHourlyRate;
                        newEmployee.OvertimeHourlyRate = oldEmployee.OvertimeHourlyRate;
                        newEmployee.OvertimeNightHourlyRate = oldEmployee.OvertimeNightHourlyRate;
                        newEmployee.TardyHourlyRate = oldEmployee.TardyHourlyRate;
                        newEmployee.Allowance = oldEmployee.Allowance;
                    }

                    mappingProfile.mapper.Map(newEmployee, oldEmployee);

                    await _context.SaveChangesAsync();

                    resultId = oldEmployee?.Id ?? 0;
                }

                var deletedEmpMemoIds = newEmployee.MstEmployeeMemos.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedEmpMemoRange = _context.MstEmployeeMemos.Where(x => deletedEmpMemoIds.Contains(x.Id)).ToList();
                _context.MstEmployeeMemos.RemoveRange(deletedEmpMemoRange);

                //var deleteEmpShiftCodeIds = newEmployee.MstEmployeeShiftCodes.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                //var deletedEmpShiftCodeRange = _context.MstEmployeeShiftCodes.Where(x => deleteEmpShiftCodeIds.Contains(x.Id)).ToList();
                //_context.MstEmployeeShiftCodes.RemoveRange(deletedEmpShiftCodeRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
