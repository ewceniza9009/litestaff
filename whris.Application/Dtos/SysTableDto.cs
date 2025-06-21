namespace whris.Application.Dtos
{
    public class SysTableDto
    {
        public int Id { get; set; }
        public List<MstPayrollGroupDto> MstPayrollGroups { get; set; } = new List<MstPayrollGroupDto>();
        public List<MstPayrollTypeDto> MstPayrollTypes { get; set; } = new List<MstPayrollTypeDto>();
        public List<MstDepartmentDto> MstDepartments { get; set; } = new List<MstDepartmentDto>();
        public List<MstPositionDto> MstPositions { get; set; } = new List<MstPositionDto>();
        public List<MstAccountDto> MstAccounts { get; set; } = new List<MstAccountDto>();
        public List<MstReligionDto> MstReligions { get; set; } = new List<MstReligionDto>();
        public List<MstCitizenshipDto> MstCitizenships { get; set; } = new List<MstCitizenshipDto>();
        public List<MstZipCodeDto> MstZipCodes { get; set; } = new List<MstZipCodeDto>();
        public List<MstDivisionDto> MstDivisions { get; set; } = new List<MstDivisionDto>();
    }
}
