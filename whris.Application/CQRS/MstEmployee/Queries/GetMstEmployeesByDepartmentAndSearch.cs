using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.MstEmployee.Queries
{
    public class GetMstEmployeesByDepartmentAndSearch : IRequest<DataSourceResult>
    {
        [DataSourceRequest]
        public DataSourceRequest? Request { get; set; }
        public int? DepartmentId { get; set; }
        public string? Search { get; set; }

        public class GetMstEmployeesByDepartmentAndSearchHandler : IRequestHandler<GetMstEmployeesByDepartmentAndSearch, DataSourceResult>
        {
            private readonly HRISContext _context;
            public GetMstEmployeesByDepartmentAndSearchHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<DataSourceResult> Handle(GetMstEmployeesByDepartmentAndSearch request, CancellationToken cancellationToken)
            {
                var result = new DataSourceResult();
                var search = request?.Search?.ToLower() ?? "";

                if (request == null) 
                {
                    return result;
                }

                if (request.DepartmentId == null && string.IsNullOrEmpty(request.Search?.Trim()))
                {
                    var preResultWithNoDepartmentAndSearch = _context.MstEmployees
                        .OrderBy(x => x.FullName)
                        .Select(x => new MstEmployeeDto()
                        {
                            Id = x.Id,
                            BiometricIdNumber = x.BiometricIdNumber,
                            FullName = x.FullName,
                            CellphoneNumber = x.CellphoneNumber,
                            EmailAddress = x.EmailAddress,
                            DepartmentId = x.DepartmentId,
                            DepartmentName = x.DepartmentName,
                            BranchName = x.BranchName,
                            NewDailyRate = x.NewDailyRate,
                            NewAllowance = x.NewAllowance,
                            IsLocked = x.IsLocked,
                        }).ToList();
                    return await preResultWithNoDepartmentAndSearch.ToDataSourceResultAsync(request.Request);
                }

                if (request.DepartmentId == null)
                {
                    var preResultWithNoDepartment = _context.MstEmployees
                        .Where(x => x.FullName.ToLower().Contains(search))
                        .OrderBy(x => x.FullName)
                        .Select(x => new MstEmployeeDto()
                        {
                            Id = x.Id,
                            BiometricIdNumber = x.BiometricIdNumber,
                            FullName = x.FullName,
                            CellphoneNumber = x.CellphoneNumber,
                            EmailAddress = x.EmailAddress,
                            DepartmentId = x.DepartmentId,
                            DepartmentName = x.DepartmentName,
                            BranchName = x.BranchName,
                            NewDailyRate = x.NewDailyRate,
                            NewAllowance = x.NewAllowance,
                            IsLocked = x.IsLocked,
                        }).ToList();
                    return await preResultWithNoDepartment.ToDataSourceResultAsync(request.Request);
                }

                var preResult = _context.MstEmployees
                    .Where(x => x.DepartmentId == request.DepartmentId &&
                        x.FullName.ToLower().Contains(search))
                    .OrderBy(x => x.FullName)
                    .Select(x => new MstEmployeeDto()
                    {
                        Id = x.Id,
                        BiometricIdNumber = x.BiometricIdNumber,
                        FullName = x.FullName,
                        CellphoneNumber = x.CellphoneNumber,
                        EmailAddress = x.EmailAddress,
                        DepartmentId = x.DepartmentId,
                        DepartmentName = x.DepartmentName,
                        BranchName = x.BranchName,
                        NewDailyRate = x.NewDailyRate,
                        NewAllowance = x.NewAllowance,
                        IsLocked = x.IsLocked,
                    }).ToList();

                result = await preResult.ToDataSourceResultAsync(request.Request);

                return result;

            }
        }
    }
}
