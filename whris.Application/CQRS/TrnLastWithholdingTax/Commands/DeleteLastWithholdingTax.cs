using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnLastWithholdingTax.Commands
{
    public class DeleteLastWithholdingTax : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteLastWithholdingTaxHandler : IRequestHandler<DeleteLastWithholdingTax, int>
        {
            private readonly HRISContext _context;
            public DeleteLastWithholdingTaxHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteLastWithholdingTax command, CancellationToken cancellationToken)
            {
                var deleteLastWithholdingTax = _context.TrnLastWithholdingTaxes.Find(command.Id) ?? new Data.Models.TrnLastWithholdingTax();

                _context.TrnLastWithholdingTaxes.Remove(deleteLastWithholdingTax);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
