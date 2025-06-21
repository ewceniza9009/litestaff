using MediatR;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLastWithholdingTax.Commands
{
    public class AddLastWithholdingTaxLinesByProcess : IRequest<int>
    {
        public int LWTId { get; set; }
        public int PeriodId { get; set; }
        public int PayrollGroupId { get; set; }       
        public int? EmployeeId { get; set; }

        public class AddLastWithholdingTaxLinesProcessHandler : IRequestHandler<AddLastWithholdingTaxLinesByProcess, int>
        {
            private readonly HRISContext _context;
            public AddLastWithholdingTaxLinesProcessHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(AddLastWithholdingTaxLinesByProcess command, CancellationToken cancellationToken)
            {
                await LastWithholdingTax.ProcessLastWithholdingTaxLines(command);

                return await Task.Run(() => 0);
            }
        }
    }
}
