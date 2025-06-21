using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstUser.Queries
{
    public class GetMstUsersBySearch : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public string? Search { get; set; }

        public class GetMstUserBySearchHandler : IRequestHandler<GetMstUsersBySearch, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetMstUserBySearchHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetMstUsersBySearch request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();
                var search = request?.Search?.ToLower() ?? "";

                if (request == null)
                {
                    return result;
                }

                result = await _context.MstUsers
                    .Where(x => x.FullName.ToLower().Contains(search))
                    .OrderBy(x => x.FullName)
                    .Select(x => new MstUserDto()
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        FullName = x.FullName,
                        IsLocked = x.IsLocked,
                    })
                    .ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
