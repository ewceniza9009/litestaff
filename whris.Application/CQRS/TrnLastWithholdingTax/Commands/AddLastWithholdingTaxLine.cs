using MediatR;
using whris.Application.Dtos;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLastWithholdingTax.Commands
{
    public class AddLastWithholdingTaxLine : IRequest<TrnLastWithholdingTaxLineDto>
    {
        public int LastWithholdingTaxId { get; set; }

        public class AddLastWithholdingTaxLineHandler : IRequestHandler<AddLastWithholdingTaxLine, TrnLastWithholdingTaxLineDto>
        {
            private readonly HRISContext _context;

            public AddLastWithholdingTaxLineHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<TrnLastWithholdingTaxLineDto> Handle(AddLastWithholdingTaxLine command, CancellationToken cancellationToken)
            {
                var result = new TrnLastWithholdingTaxLineDto()
                {
                    Id = 0,
                    LastWithholdingTaxId = command.LastWithholdingTaxId,
                    EmployeeId = _context.MstEmployees.FirstOrDefault()?.Id ?? 0,
                    TaxCodeId = _context.MstTaxCodes.FirstOrDefault()?.Id ?? 0,
                    //LeaveId = _context.MstLeaves.OrderByDescending(x => x.Leave).FirstOrDefault()?.Id ?? 0,
                    //Date = DateTime.Now,
                    //NumberOfHours = 8,
                    //WithPay = false,
                    //DebitToLedger = false,
                    //Remarks = "NA",
                    //IsDeleted = false,
                };

                return await Task.Run(() => result);
            }
        }
    }
}
