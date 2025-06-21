using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Commands
{
    public class UploadEmployees : IRequest<int>
    {
        public required List<TmpUploadEmployee> Employees { get; set; }

        public class UploadEmployeesHandler : IRequestHandler<UploadEmployees, int>
        {
            private readonly HRISContext _context;
            public UploadEmployeesHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(UploadEmployees command, CancellationToken cancellationToken)
            {
                foreach (var employee in command.Employees) 
                {
                    var newEmployee = new Data.Models.MstEmployee()
                    {
                        Id = 0,
                        IdNumber = employee.BiometricIdNumber,
                        BiometricIdNumber = employee.BiometricIdNumber,
                        LastName = employee.LastName,
                        FirstName = employee.FirstName,
                        MiddleName = employee.MiddleName,
                        ExtensionName = "NA",
                        FullName = employee.FullName,
                        Address = "NA",
                        ZipCodeId = _context.MstZipCodes.FirstOrDefault()?.Id ?? 0,
                        PhoneNumber = "NA",
                        CellphoneNumber = "NA",
                        EmailAddress = "NA",
                        DateOfBirth = DateTime.Now,
                        PlaceOfBirth = "NA",
                        PlaceOfBirthZipCodeId = _context.MstZipCodes.FirstOrDefault()?.Id ?? 0,
                        DateHired = DateTime.Now,
                        DateResigned = DateTime.Now,
                        Sex = "Male",
                        CivilStatus = "SINGLE",
                        CitizenshipId = _context.MstCitizenships.FirstOrDefault()?.Id ?? 0,
                        ReligionId = _context.MstReligions.FirstOrDefault()?.Id ?? 0,
                        Height = 0,
                        Weight = 0,
                        Gsisnumber = "NA",
                        Sssnumber = "NA",
                        Hdmfnumber = "NA",
                        Phicnumber = "NA",
                        Tin = "NA",
                        TaxCodeId = _context.MstTaxCodes.FirstOrDefault()?.Id ?? 0,
                        AtmaccountNumber = "NA",
                        CompanyId = _context.MstCompanies.FirstOrDefault()?.Id ?? 0,
                        BranchId = _context.MstBranches.FirstOrDefault()?.Id ?? 0,
                        DepartmentId = _context.MstDepartments.FirstOrDefault()?.Id ?? 0,
                        DivisionId = _context.MstDivisions.FirstOrDefault()?.Id ?? 0,
                        PositionId = _context.MstPositions.FirstOrDefault()?.Id ?? 0,
                        PayrollGroupId = _context.MstPayrollGroups.FirstOrDefault()?.Id ?? 0,
                        AccountId = _context.MstAccounts.FirstOrDefault()?.Id ?? 0,
                        PayrollTypeId = _context.MstPayrollTypes.FirstOrDefault()?.Id ?? 0,
                        ShiftCodeId = _context.MstShiftCodes.FirstOrDefault()?.Id ?? 0,
                        FixNumberOfDays = 0,
                        FixNumberOfHours = 0,
                        MonthlyRate = employee.DailyRate * 24,
                        PayrollRate = employee.DailyRate * 24,
                        DailyRate = employee.DailyRate,
                        AbsentDailyRate = employee.DailyRate,
                        HourlyRate = employee.DailyRate / 8,
                        NightHourlyRate = (employee.DailyRate / 8) * 0.15m,
                        OvertimeHourlyRate = employee.DailyRate / 8,
                        OvertimeNightHourlyRate = (employee.DailyRate / 8) * 0.15m,
                        TardyHourlyRate = employee.DailyRate / 8,
                        EntryUserId = 1,
                        EntryDateTime = DateTime.Now,
                        UpdateUserId = 1,
                        UpdateDateTime = DateTime.Now,
                        IsLocked = true,
                        TaxTable = "NA",
                        HdmfaddOn = 0,
                        SssaddOn = 0,
                        Hdmftype = "NA",
                        SssisGrossAmount = false,
                        IsMinimumWageEarner = false,
                        IsExemptedInMandatoryDeductions = false,
                    };

                    await _context.MstEmployees.AddAsync(newEmployee);
                }

                await _context.SaveChangesAsync();

                return await Task.Run(() => 0);
            }
        }
    }
}
