using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Models;

namespace whris.UI.Services.Datasources
{
    public class MstEmployeeComboboxDatasources
    {
        public List<MstZipCode> ZipCodeCmbDs => (List<MstZipCode>)(Common.GetZipCodes()?.Value ?? new List<MstZipCode>());
        public List<MstCitizenship> CitizenShipCmbDs => (List<MstCitizenship>)(Common.GetCitizenships()?.Value ?? new List<MstCitizenship>());
        public List<MstReligion> ReligionCmbDs => (List<MstReligion>)(Common.GetReligions()?.Value ?? new List<MstReligion>());
        public List<MstTaxCode> TaxCodeCmbDs => (List<MstTaxCode>)(Common.GetTaxCodes()?.Value ?? new List<MstTaxCode>());
        public List<MstCompany> CompanyCmbDs => (List<MstCompany>)(Common.GetCompanies()?.Value ?? new List<MstCompany>());
        public List<MstBranch> BranchCmbDs => (List<MstBranch>)(Common.GetBranches()?.Value ?? new List<MstBranch>());
        public List<MstDepartment> DepartmentCmbDs => (List<MstDepartment>)(Common.GetDepartments()?.Value ?? new List<MstDepartment>());
        public List<MstPosition> PostionCmbDs => (List<MstPosition>)(Common.GetPositions()?.Value ?? new List<MstPosition>());
        public List<MstDivision> DivisionCmbDs => (List<MstDivision>)(Common.GetDivisions()?.Value ?? new List<MstDivision>());
        public List<MstPayrollGroup> PayrollGroupCmbDs => (List<MstPayrollGroup>)(Common.GetPayrollGroups()?.Value ?? new List<MstPayrollGroup>());
        public List<MstPayrollType> PayrollTypeCmbDs => (List<MstPayrollType>)(Common.GetPayrollTypes()?.Value ?? new List<MstPayrollType>());
        public List<MstAccount> GlAccountCmbDs => (List<MstAccount>)(Common.GetGLAccounts()?.Value ?? new List<MstAccount>());
        public List<MstShiftCode> ShiftCodeCmbDs => (List<MstShiftCode>)(Common.GetShiftCodes()?.Value ?? new List<MstShiftCode>());
        public List<MstUser> UserCmbDs => (List<MstUser>)(Common.GetUsers()?.Value ?? new List<MstUser>());
        public List<string> GenderCmbDs => new List<string>() { "MALE", "FEMALE" };
        public List<string> CivilStatusCmbDs => new List<string>() { "MARRIED", "SINGLE", "SEPARATED", "WIDOW" };
        public List<string> HDMFTypeCmbDs => new List<string>() { "Percentage", "Value" };
        public List<string> TaxTableCmbDs => new List<string>() { "Semi-Monthly", "Monthly" };
        public List<MstEmploymentTypeDto> EmploymentTypeCmbDs => new List<MstEmploymentTypeDto>()
        {
            new MstEmploymentTypeDto() { Id = 1, EmploymentType = "Regular" }, 
            new MstEmploymentTypeDto() { Id = 2, EmploymentType = "Probationary"}, 
            new MstEmploymentTypeDto() { Id = 3, EmploymentType = "Newly Hired"} 
        };

        public static MstEmployeeComboboxDatasources Instance => new MstEmployeeComboboxDatasources();

        private MstEmployeeComboboxDatasources()
        {

        }
    }
}
