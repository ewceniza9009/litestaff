using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLastWithholdingTax.Commands
{
    public class AddLastWithholdingTax : IRequest<TrnLastWithholdingTaxDetailDto>
    {
        public int PayrollGroupId { get; set; }

        public class AddLastWithholdingTaxHandler : IRequestHandler<AddLastWithholdingTax, TrnLastWithholdingTaxDetailDto>
        {
            private readonly HRISContext _context;
            public AddLastWithholdingTaxHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<TrnLastWithholdingTaxDetailDto> Handle(AddLastWithholdingTax command, CancellationToken cancellationToken)
            {
                var periodId = _context.MstPeriods
                    .OrderBy(x => x.Period)
                    .LastOrDefault()?.Id ?? 0;

                var newLastWithholdingTax = new TrnLastWithholdingTaxDetailDto()
                {
                    Id = 0,
                    PeriodId = periodId,
                    Lwtnumber = "NA",
                    Lwtdate = DateTime.Now,
                    PayrollGroupId = command.PayrollGroupId,
                    Remarks = "NA",
                    IsLocked = true
                };

                return await Task.Run(() => newLastWithholdingTax);
            }
        }
    }
}
