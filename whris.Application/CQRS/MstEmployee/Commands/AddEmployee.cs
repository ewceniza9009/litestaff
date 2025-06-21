using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Commands
{
    public class AddEmployee : IRequest<MstEmployeeDetailDto>
    {
        public class AddEmployeeHandler : IRequestHandler<AddEmployee, MstEmployeeDetailDto>
        {
            private readonly HRISContext _context;
            public AddEmployeeHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<MstEmployeeDetailDto> Handle(AddEmployee command, CancellationToken cancellationToken)
            {
                var newEmployee = new MstEmployeeDetailDto()
                {
                    Id = 0,
                    IdNumber = "NA",
                    BiometricIdNumber = "NA",
                    LastName = "NA",
                    FirstName = "NA",
                    MiddleName = "NA",
                    ExtensionName = "NA",
                    FullName = "NA",
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
                    MonthlyRate = 0,
                    PayrollRate = 0,
                    DailyRate = 0,
                    AbsentDailyRate = 0,
                    HourlyRate = 0,
                    NightHourlyRate = 0,
                    OvertimeHourlyRate = 0,
                    OvertimeNightHourlyRate = 0,
                    TardyHourlyRate = 0,
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
                    IsFlex = false,
                    EmploymentType = 1
                };

                return await Task.Run(() => newEmployee);
            }
        }
    }
}
