using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Application.Queries.TrnDtr;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtrMissingPunches.Queries
{
    public class GetTrnDtrMissingPunchesById : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int Id { get; set; }


        public class GetTrnDtrMissingPunchesByIdHandler : IRequestHandler<GetTrnDtrMissingPunchesById, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnDtrMissingPunchesByIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnDtrMissingPunchesById request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();

                if (request == null)
                {
                    return result;
                }

                var missingPunches = new MissingPunches()
                {
                    DTRId = request.Id
                };

                result = await missingPunches.Result().ToDataSourceResultAsync(request.Request);

                return result;
            }
        }
    }
}
