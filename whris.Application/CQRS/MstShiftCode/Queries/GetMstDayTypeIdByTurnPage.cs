using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Data.Data;

namespace whris.Application.CQRS.MstShiftCode.Queries
{
    public class GetMstShiftCodeIdByTurnPage : IRequest<int>
    {
        public int? Id { get; set; }
        public string? Action { get; set; }

        public class GetMstShiftCodeIdByTurnPageHandler : IRequestHandler<GetMstShiftCodeIdByTurnPage, int>
        {
            private readonly HRISContext _context;
            public GetMstShiftCodeIdByTurnPageHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(GetMstShiftCodeIdByTurnPage request, CancellationToken cancellationToken)
            {
                var ShiftCodeString = _context.MstShiftCodes
                    .Find(request.Id)?.ShiftCode;

                var result = new Data.Models.MstShiftCode();

                if (request.Action == "prev")
                {
                    result = await _context.MstShiftCodes
                        .Where(x => x.ShiftCode.CompareTo(ShiftCodeString) < 0)
                        .OrderByDescending(x => x.ShiftCode)
                        .FirstOrDefaultAsync(); ;
                }
                else if (request.Action == "next")
                {
                    result = await _context.MstShiftCodes
                        .Where(x => x.ShiftCode.CompareTo(ShiftCodeString) > 0)
                        .OrderBy(x => x.ShiftCode)
                        .FirstOrDefaultAsync();
                }

                return result?.Id ?? 0;
            }
        }
    }
}
