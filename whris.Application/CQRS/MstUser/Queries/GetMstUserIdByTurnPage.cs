using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.MstUser.Queries
{
    public class GetMstUserIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public string? Action { get; set; }

        public class GetMstUserIdByTurnPageHandler : IRequestHandler<GetMstUserIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetMstUserIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetMstUserIdByTurnPage request, CancellationToken cancellationToken)
            {
                var UserString = _context.MstUsers
                    .Find(request.Id)?.FullName;

                var result = new Data.Models.MstUser();

                if (request.Action == "prev")
                {
                    result = await _context.MstUsers
                        .Where(x => x.FullName.CompareTo(UserString) < 0)
                        .OrderByDescending(x => x.FullName)
                        .FirstOrDefaultAsync(); ;
                }
                else if (request.Action == "next")
                {
                    result = await _context.MstUsers
                        .Where(x => x.FullName.CompareTo(UserString) > 0)
                        .OrderBy(x => x.FullName)
                        .FirstOrDefaultAsync();
                }

                return result?.Id ?? 0;
            }
        }
    }
}
