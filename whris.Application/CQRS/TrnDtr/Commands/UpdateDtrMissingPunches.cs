using MediatR;
using Microsoft.EntityFrameworkCore;
using whris.Application.Library;
using whris.Application.Queries.TrnDtr;
using whris.Data.Data;

namespace whris.Application.CQRS.TrnDtr.Commands
{
    public class UpdateDtrMissingPunches : IRequest<int>
    {
        public List<MissingPunches.Record>? MissingPunches { get; set; }
        public class AddDtrLinesHandler : IRequestHandler<UpdateDtrMissingPunches, int>
        {
            private readonly HRISContext _context;
            public AddDtrLinesHandler(HRISContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(UpdateDtrMissingPunches command, CancellationToken cancellationToken)
            {
                if (command.MissingPunches is not null && command.MissingPunches?.Count > 0) 
                {
                    foreach (var line in command.MissingPunches) 
                    {
                        var lineItem = _context.TrnDtrlines.FirstOrDefault(x => x.Id == line.Id);
                        var lineTimeIn1 = DateTime.Parse($"{DateOnly.FromDateTime(line.Date)} {TimeOnly.FromDateTime(line.TimeIn1 ?? new DateTime(1990, 09, 15))}");
                        var lineTimeOut1 = DateTime.Parse($"{DateOnly.FromDateTime(line.Date)} {TimeOnly.FromDateTime(line.TimeOut1 ?? new DateTime(1990, 09, 15))}");

                        if (line.TimeOut1 is not null && lineTimeIn1 > lineTimeOut1) 
                        {
                            lineTimeOut1 = lineTimeOut1.AddDays(1);
                        }

                        var lineTimeIn2 = DateTime.Parse($"{DateOnly.FromDateTime(line.Date)} {TimeOnly.FromDateTime(line.TimeIn2 ?? new DateTime(1990, 09, 15))}");

                        if (line.TimeIn2 is not null && lineTimeIn1 > lineTimeIn2)
                        {
                            lineTimeIn2 = lineTimeIn2.AddDays(1);
                        }

                        var lineTimeOut2 = DateTime.Parse($"{DateOnly.FromDateTime(line.Date)} {TimeOnly.FromDateTime(line.TimeOut2 ?? new DateTime(1990, 09, 15))}");

                        if (line.TimeOut2 is not null && lineTimeIn1 > lineTimeOut2)
                        {
                            lineTimeOut2 = lineTimeOut2.AddDays(1);
                        }

                        if (lineItem is not null) 
                        {
                            lineItem.TimeIn1 = line.TimeIn1 is not null ? lineTimeIn1 : null;
                            lineItem.TimeOut1 = line.TimeOut1 is not null ? lineTimeOut1 : null;
                            lineItem.TimeIn2 = line.TimeIn2 is not null ? lineTimeIn2 : null;
                            lineItem.TimeOut2 = line.TimeOut2 is not null ? lineTimeOut2 : null;
                        }

                        lineItem.Dtrremarks = line.Dtrremarks;
                    }
                }

                _context.SaveChanges();

                return await Task.Run(() => 0);
            }
        }
    }
}
