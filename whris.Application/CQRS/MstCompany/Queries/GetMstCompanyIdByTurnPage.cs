using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.MstCompany.Queries
{
    public class GetMstCompanyIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public string? Action { get; set; }

        public class GetMstCompanyIdByTurnPageHandler : IRequestHandler<GetMstCompanyIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetMstCompanyIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetMstCompanyIdByTurnPage request, CancellationToken cancellationToken)
            {
                var companyString = _context.MstCompanies
                    .Find(request.Id)?.Company;

                var result = new Data.Models.MstCompany();

                if (request.Action == "prev")
                {
                    result = await _context.MstCompanies
                        .Where(x => x.Company.CompareTo(companyString) < 0)
                        .OrderByDescending(x => x.Company)
                        .FirstOrDefaultAsync(); ;
                }
                else if (request.Action == "next")
                {
                    result = await _context.MstCompanies
                        .Where(x => x.Company.CompareTo(companyString) > 0)
                        .OrderBy(x => x.Company)
                        .FirstOrDefaultAsync();
                }

                return result?.Id ?? 0;
            }
        }
    }
}
