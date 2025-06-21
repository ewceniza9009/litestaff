using MediatR;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.CQRS.MstSysTable.Commands
{
    public class SaveSysTable : IRequest<int>
    {
        public SysTableDto? SysTable { get; set; }

        public class SaveSysTableHandler : IRequestHandler<SaveSysTable, int>
        {
            private readonly HRISContext _context;
            public SaveSysTableHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveSysTable command, CancellationToken cancellationToken)
            {
                var mappingProfilePayrollGroup = new MappingProfile<MstPayrollGroupDto, MstPayrollGroup>();
                var mappingProfilePayrollType = new MappingProfile<MstPayrollTypeDto, MstPayrollType>();
                var mappingProfileDepartment = new MappingProfile<MstDepartmentDto, MstDepartment>();
                var mappingProfilePosition = new MappingProfile<MstPositionDto, MstPosition>();
                var mappingProfileAccount = new MappingProfile<MstAccountDto, MstAccount>();
                var mappingProfileReligion = new MappingProfile<MstReligionDto, MstReligion>();
                var mappingProfileCitizenship = new MappingProfile<MstCitizenshipDto, MstCitizenship>();
                var mappingProfileZipCode = new MappingProfile<MstZipCodeDto, MstZipCode>();
                var mappingProfileDivision = new MappingProfile<MstDivisionDto, MstDivision>();

                foreach (var item in command.SysTable?.MstPayrollGroups ?? new List<MstPayrollGroupDto>()) 
                {
                    var oldItem = _context.MstPayrollGroups
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstPayrollGroup();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstPayrollGroups.AddRangeAsync(mappingProfilePayrollGroup.mapper.Map(item, new MstPayrollGroup()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfilePayrollGroup.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstPayrollGroups.Remove(oldItem);
                }

                foreach (var item in command.SysTable?.MstPayrollTypes ?? new List<MstPayrollTypeDto>())
                {
                    var oldItem = _context.MstPayrollTypes
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstPayrollType();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstPayrollTypes.AddRangeAsync(mappingProfilePayrollType.mapper.Map(item, new MstPayrollType()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfilePayrollType.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstPayrollTypes.Remove(oldItem);
                }

                foreach (var item in command.SysTable?.MstDepartments ?? new List<MstDepartmentDto>())
                {
                    var oldItem = _context.MstDepartments
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstDepartment();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstDepartments.AddRangeAsync(mappingProfileDepartment.mapper.Map(item, new MstDepartment()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileDepartment.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstDepartments.Remove(oldItem);

                }

                foreach (var item in command.SysTable?.MstPositions ?? new List<MstPositionDto>())
                {
                    var oldItem = _context.MstPositions
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstPosition();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstPositions.AddRangeAsync(mappingProfilePosition.mapper.Map(item, new MstPosition()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfilePosition.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstPositions.Remove(oldItem);
                }

                foreach (var item in command.SysTable?.MstAccounts ?? new List<MstAccountDto>())
                {
                    var oldItem = _context.MstAccounts
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstAccount();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstAccounts.AddRangeAsync(mappingProfileAccount.mapper.Map(item, new MstAccount()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileAccount.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstAccounts.Remove(oldItem);
                }

                foreach (var item in command.SysTable?.MstReligions ?? new List<MstReligionDto>())
                {
                    var oldItem = _context.MstReligions
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstReligion();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstReligions.AddRangeAsync(mappingProfileReligion.mapper.Map(item, new MstReligion()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileReligion.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstReligions.Remove(oldItem);
                }

                foreach (var item in command.SysTable?.MstCitizenships ?? new List<MstCitizenshipDto>())
                {
                    var oldItem = _context.MstCitizenships
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstCitizenship();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstCitizenships.AddRangeAsync(mappingProfileCitizenship.mapper.Map(item, new MstCitizenship()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileCitizenship.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstCitizenships.Remove(oldItem);
                }

                foreach (var item in command.SysTable?.MstZipCodes ?? new List<MstZipCodeDto>())
                {
                    var oldItem = _context.MstZipCodes
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstZipCode();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstZipCodes.AddRangeAsync(mappingProfileZipCode.mapper.Map(item, new MstZipCode()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileZipCode.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstZipCodes.Remove(oldItem);
                }

                foreach (var item in command.SysTable?.MstDivisions ?? new List<MstDivisionDto>())
                {
                    var oldItem = _context.MstDivisions
                        .FirstOrDefault(x => x.Id == item.Id)
                        ?? new MstDivision();

                    if (item.Id == 0 && !item.IsDeleted) await _context.MstDivisions.AddRangeAsync(mappingProfileDivision.mapper.Map(item, new MstDivision()));
                    if (item.Id > 0 && !item.IsDeleted) mappingProfileDivision.mapper.Map(item, oldItem);
                    if (item.Id > 0 && item.IsDeleted) _context.MstDivisions.Remove(oldItem);
                }

                await _context.SaveChangesAsync();

                return 0;
            }
        }
    }
}
