using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnTrnLogById.Commands
{
    public class DeleteTrnLogById : IRequest<int>
    {
        public int Id { get; set; }

        public class DeleteTrnLogByIdHandler : IRequestHandler<DeleteTrnLogById, int>
        {
            private readonly HRISContext _context;
            public DeleteTrnLogByIdHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteTrnLogById command, CancellationToken cancellationToken)
            {
                var deleteTrnLogById = _context.TrnLogs.Find(command.Id) ?? new Data.Models.TrnLog();

                //_context.TrnLogs.Remove(deleteTrnLogById);
                deleteTrnLogById.LogType = "X";
                _context.SaveChanges();

                return await Task.Run(() => command.Id);
            }
        }
    }
}
