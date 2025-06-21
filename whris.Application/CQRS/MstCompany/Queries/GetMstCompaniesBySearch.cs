using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstCompany.Queries
{
    public  class GetMstCompaniesBySearch : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public string? Search { get; set; }

        public class GetMstCompaniesBySearchHandler : IRequestHandler<GetMstCompaniesBySearch, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetMstCompaniesBySearchHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetMstCompaniesBySearch request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();
                var search = request?.Search?.ToLower() ?? "";

                if (request == null)
                {
                    return result;
                }

                result = await _context.MstCompanies
                    .Where(x => x.Company.ToLower().Contains(search))
                    .OrderBy(x => x.Company)
                    .Select(x => new MstCompanyDto()
                    {
                        Id = x.Id,
                        Company = x.Company,
                        Address = x.Address,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
