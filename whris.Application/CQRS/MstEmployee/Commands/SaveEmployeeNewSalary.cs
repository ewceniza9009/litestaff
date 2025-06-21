using MediatR;
using System.Security.Cryptography;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Mappers;
using whris.Data.Data;
using static whris.Application.Queries.TrnDtr.GetEmployees;

namespace whris.Application.CQRS.MstEmployee.Commands
{
    public class SaveEmployeeNewSalary : IRequest<int>
    {
        public MstEmployeeDetailDto? Employee { get; set; }

        public class SaveEmployeeNewSalaryHandler : IRequestHandler<SaveEmployeeNewSalary, int>
        {
            private readonly HRISContext _context;
            public SaveEmployeeNewSalaryHandler(HRISContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(SaveEmployeeNewSalary command, CancellationToken cancellationToken)
            {
                var resultId = 0;                

                if (command.Employee?.Id != 0)
                {
                    var employee = await _context.MstEmployees.FindAsync(command?.Employee?.Id ?? 0);

                    if(employee != null) 
                    {
                        employee.MstEmployeeSalaryHistories.Add(new Data.Models.MstEmployeeSalaryHistory()
                        {
                            ChangeDate = DateTime.Now,
                            MonthlyRate = employee.MonthlyRate,
                            PayrollRate = employee.PayrollRate,
                            DailyRate = employee.DailyRate,
                            AbsentDailyRate = employee.AbsentDailyRate,
                            HourlyRate = employee.HourlyRate,
                            NightHourlyRate = employee.NightHourlyRate,
                            OvertimeHourlyRate = employee.OvertimeHourlyRate,
                            OvertimeNightHourlyRate = employee.OvertimeNightHourlyRate,
                            TardyHourlyRate = employee.TardyHourlyRate,
                            Allowance = employee.Allowance ?? 0,
                        });

                        employee.MonthlyRate = employee.NewMonthlyRate ?? 0;
                        employee.PayrollRate = employee.NewPayrollRate ?? 0;
                        employee.DailyRate = employee.NewDailyRate ?? 0;
                        employee.AbsentDailyRate = employee.NewAbsentDailyRate ?? 0;
                        employee.HourlyRate = employee.NewHourlyRate ?? 0;
                        employee.NightHourlyRate = employee.NewNightHourlyRate ?? 0;
                        employee.OvertimeHourlyRate = employee.NewOvertimeHourlyRate ?? 0;
                        employee.OvertimeNightHourlyRate = employee.NewOvertimeNightHourlyRate ?? 0;
                        employee.TardyHourlyRate = employee.NewTardyHourlyRate ?? 0;
                        employee.Allowance = employee.NewAllowance ?? 0;

                        employee.NewMonthlyRate = 0;
                        employee.NewPayrollRate = 0;
                        employee.NewDailyRate = 0;
                        employee.NewAbsentDailyRate = 0;
                        employee.NewHourlyRate = 0;
                        employee.NewNightHourlyRate = 0;
                        employee.NewOvertimeHourlyRate = 0;
                        employee.NewOvertimeNightHourlyRate = 0;
                        employee.NewTardyHourlyRate = 0;
                        employee.NewAllowance = 0;                        

                        Utilities.UpdateEntityAuditFields(employee);
                    }                    

                    await _context.SaveChangesAsync();

                    resultId = employee?.Id ?? 0;
                }

                return resultId;
            }
        }
    }
}
