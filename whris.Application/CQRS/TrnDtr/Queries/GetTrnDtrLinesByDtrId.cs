using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Queries
{
    public class GetTrnDtrLinesByDtrId : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int? Id { get; set; }

        public class GetTrnDtrLinesByDtrIdHandler : IRequestHandler<GetTrnDtrLinesByDtrId, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetTrnDtrLinesByDtrIdHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetTrnDtrLinesByDtrId request, CancellationToken cancellationToken)
            {
                var dtrLines = new List<Data.Models.TrnDtrline>();
                var result = new List<TrnDtrLineDto>();
                var finalResult = new DataSourceResult();

                var requestFilterCount = request.Request?.Filters.Count() ?? 0;
                var filterEmployee = request.Request?.Filters.Count() == 0 ? null : (request.Request?.Filters[0] as Kendo.Mvc.FilterDescriptor)?.Value.ToString();

                //(PageNumber - 1) * RecordsPerPage
                var page = ((request.Request?.Page ?? 0) - 1);
                var pageSize = request.Request?.PageSize ?? 0;

                var resultTotal = 0;

                if (requestFilterCount> 0 && !string.IsNullOrEmpty(filterEmployee))
                {
                    dtrLines = await _context.TrnDtrlines
                        .Include(x => x.Employee)
                        .Where(x => x.Dtrid == request.Id && x.Employee.IsLocked && x.Employee.FullName.Contains(filterEmployee))
                        .OrderBy(x => x.Employee.FullName)
                        .ThenBy(x => x.Date)
                        .Skip(page * pageSize)
                        .Take(pageSize)                        
                        .ToListAsync();

                    resultTotal = _context.TrnDtrlines
                        .Include(x => x.Employee)
                        .Where(x => x.Dtrid == request.Id && x.Employee.IsLocked && x.Employee.FullName.Contains(filterEmployee))
                        .Count();
                }
                else 
                {
                    dtrLines = await _context.TrnDtrlines
                        .Include(x => x.Employee)
                        .Where(x => x.Dtrid == request.Id && x.Employee.IsLocked)
                        .OrderBy(x => x.Employee.FullName)
                        .ThenBy(x => x.Date)
                        .Skip(page * pageSize)
                        .Take(pageSize)                       
                        .ToListAsync();

                    resultTotal = _context.TrnDtrlines
                        .Include(x => x.Employee)
                        .Where(x => x.Dtrid == request.Id && x.Employee.IsLocked)
                        .Count();
                }

                var mappingProfile = new MappingProfileForTrnDtrLine();
                mappingProfile.mapper.Map(dtrLines, result);

                var aggregateResults = result.ToDataSourceResult(request.Request).AggregateResults;

                finalResult.AggregateResults = aggregateResults;
                finalResult.Data = result;
                finalResult.Total = resultTotal;

                return finalResult;
            }
        }
    }
}
