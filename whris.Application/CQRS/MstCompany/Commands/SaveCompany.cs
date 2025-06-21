using MediatR;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.MstCompany.Commands
{
    public class SaveCompany : IRequest<int>
    {
        public MstCompanyDetailDto? Company { get; set; }

        public class SaveCompanyHandler : IRequestHandler<SaveCompany, int>
        {
            private readonly HRISContext _context;
            public SaveCompanyHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveCompany command, CancellationToken cancellationToken)
            {
                var resultId = 0;
                var newCompany = command?.Company ?? new MstCompanyDetailDto();
                newCompany.MstBranches.RemoveAll(x => x.IsDeleted && x.Id == 0);

                Utilities.UpdateEntityAuditFields(newCompany);

                var mappingProfile = new MappingProfileForMstCompanyDetailReverse();

                if (newCompany.Id == 0)
                {
                    var addedCompany = mappingProfile.mapper.Map<Data.Models.MstCompany>(newCompany);
                    await _context.MstCompanies.AddAsync(addedCompany ?? new Data.Models.MstCompany());

                    await _context.SaveChangesAsync();

                    resultId = addedCompany?.Id ?? 0;
                }
                else 
                {
                    var oldCompany = await _context.MstCompanies.FindAsync(command?.Company?.Id ?? 0);
                    mappingProfile.mapper.Map(newCompany, oldCompany);

                    await _context.SaveChangesAsync();

                    resultId = oldCompany?.Id ?? 0;
                }

                var deletedBranchIds = newCompany.MstBranches.Where(x => x.IsDeleted).Select(x => x.Id).ToList();
                var deletedBranchRange = _context.MstBranches.Where(x => deletedBranchIds.Contains(x.Id)).ToList();

                _context.MstBranches.RemoveRange(deletedBranchRange);

                await _context.SaveChangesAsync();

                return resultId;
            }
        }
    }
}
