using Kendo.Mvc.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Queries
{
    public class GetMstEmployeeIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public int? DepartmentId { get; set; }
        public string? Action { get; set; }

        public class GetMstEmployeeIdByTurnPageHandler : IRequestHandler<GetMstEmployeeIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetMstEmployeeIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetMstEmployeeIdByTurnPage request, CancellationToken cancellationToken)
            {
                var employeeString = _context.MstEmployees
                    .Find(request.Id)?.FullName;

                var result = new Data.Models.MstEmployee();

                if (request.Action == "prev")
                {
                    if (request.DepartmentId == 0)
                    {
                        result = await _context.MstEmployees
                        .Where(x => x.FullName.CompareTo(employeeString) < 0)
                        .OrderByDescending(x => x.FullName)
                        .FirstOrDefaultAsync(); ;
                    }
                    else 
                    {
                        result = await _context.MstEmployees
                        .Where(x => x.FullName.CompareTo(employeeString) < 0 && x.DepartmentId == request.DepartmentId)
                        .OrderByDescending(x => x.FullName)
                        .FirstOrDefaultAsync(); ;
                    }
                }
                else if(request.Action == "next")
                {
                    if (request.DepartmentId == 0)
                    {
                        result = await _context.MstEmployees
                        .Where(x => x.FullName.CompareTo(employeeString) > 0)
                        .OrderBy(x => x.FullName)
                        .FirstOrDefaultAsync();
                    }
                    else 
                    {
                        result = await _context.MstEmployees
                            .Where(x => x.FullName.CompareTo(employeeString) > 0 && x.DepartmentId == request.DepartmentId)
                            .OrderBy(x => x.FullName)
                            .FirstOrDefaultAsync();
                    }
                }

                return result?.Id ?? 0;
            }
        }
    }
}
