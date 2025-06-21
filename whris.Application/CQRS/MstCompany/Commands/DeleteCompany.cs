using MediatR;
using whris.Data.Data;

namespace whris.Application.CQRS.MstCompany.Commands
{
    public class DeleteCompany : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteCompanyHandler : IRequestHandler<DeleteCompany, int>
        {
            private readonly HRISContext _context;
            public DeleteCompanyHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteCompany command, CancellationToken cancellationToken)
            {
                var deleteCompany = _context.MstCompanies.Find(command.Id) ?? new Data.Models.MstCompany();

                _context.MstCompanies.Remove(deleteCompany);
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
