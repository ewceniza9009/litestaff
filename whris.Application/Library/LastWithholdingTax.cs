using whris.Application.CQRS.TrnLastWithholdingTax.Commands;
using whris.Application.Dtos;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Library
{
    public class LastWithholdingTax
    {
        internal static async Task ProcessLastWithholdingTaxLines(AddLastWithholdingTaxLinesByProcess command)
        {
            using (var ctx = new HRISContext())
            {
                if (command.EmployeeId is null)
                {
                    var lines = ctx.TrnLastWithholdingTaxLines
                        .Where(x => x.LastWithholdingTaxId == command.LWTId);

                    ctx.TrnLastWithholdingTaxLines.RemoveRange(lines);
                }
                else
                {
                    var lines = ctx.TrnLastWithholdingTaxLines
                        .Where(x => x.LastWithholdingTaxId == command.LWTId &&
                            x.EmployeeId == command.EmployeeId);

                    ctx.TrnLastWithholdingTaxLines.RemoveRange(lines);
                }

                ctx.SaveChanges();

                var trnLastWithTax = ctx.TrnLastWithholdingTaxes.FirstOrDefault(x => x.Id == command.LWTId);

                if (trnLastWithTax is not null) 
                {
                    var tax = 0m;
                    var lastWithholdingTax = new Queries.TrnLastWithholdingTax.Main();

                    lastWithholdingTax.LastWithholdingTaxId = command.LWTId;
                    lastWithholdingTax.PeriodId = command.PeriodId;
                    lastWithholdingTax.PayrollGroupId = command.PayrollGroupId;
                    lastWithholdingTax.EmployeeId = command.EmployeeId;

                    var result = lastWithholdingTax.Result();

                    foreach (var lwt in result) 
                    {
                        var exemption = ctx.MstTaxCodes.FirstOrDefault(x => x.Id == lwt.TaxCodeId)?.ExemptionAmount ?? 0m;
                        var dependentExemption = ctx.MstTaxCodes.FirstOrDefault(x => x.Id == lwt.TaxCodeId)?.DependentAmount ?? 0m;

                        var taxableIncome = lwt.SumOfTotalNetSalaryAmount +
                            lwt.SumOfTotalOtherIncomeTaxable -
                            lwt.SumOfSSSContribution -
                            lwt.SumOfPHICContribution -
                            lwt.SumOfHDMFContribution -
                            lwt.TotalOtherDeduction1 -
                            lwt.SumOfTotalNetSalaryAmount -
                            exemption - 
                            dependentExemption;

                        if (taxableIncome < 0)
                        {
                            taxableIncome = 0;
                        }

                        var taxBracket = ctx.MstTableWtaxYearlies
                            ?.FirstOrDefault(x => x.AmountStart <= taxableIncome && x.AmountEnd >= taxableIncome)
                            ?.AmountStart ?? 0;

                        var taxableIncomeLessBracket = taxableIncome - taxBracket;

                        if (taxableIncomeLessBracket <= 0)
                        {
                            tax = 0;
                        }
                        else 
                        {
                            var percentage = ctx.MstTableWtaxYearlies
                                ?.FirstOrDefault(x => x.AmountStart <= taxableIncome && x.AmountEnd >= taxableIncome)
                                ?.Percentage ?? 0;

                            var additionalAmount = ctx.MstTableWtaxYearlies
                                ?.FirstOrDefault(x => x.AmountStart <= taxableIncome && x.AmountEnd >= taxableIncome)
                                ?.AdditionalAmount ?? 0;

                            tax = ((taxableIncomeLessBracket * percentage)/100) + additionalAmount;
                        }

                        var isMinimumWager = ctx.MstEmployees.FirstOrDefault(x => x.Id == lwt.EmployeeId)?.IsMinimumWageEarner ?? false;

                        trnLastWithTax.TrnLastWithholdingTaxLines.Add(new TrnLastWithholdingTaxLine
                        {
                            LastWithholdingTaxId = lwt.LastWithholdingTaxId,
                            EmployeeId = lwt.EmployeeId,
                            TaxCodeId = lwt.TaxCodeId,
                            TotalNetSalaryAmount = lwt.SumOfTotalNetSalaryAmount,
                            TotalOtherIncomeTaxable = lwt.SumOfTotalOtherIncomeTaxable,
                            TotalSsscontribution = lwt.SumOfSSSContribution,
                            TotalSsseccontribution = lwt.SumOfSSSECContribution,
                            TotalPhiccontribution = lwt.SumOfPHICContribution,
                            TotalHdmfcontribution = lwt.SumOfHDMFContribution,
                            TotalOtherDeductionTaxable = lwt.TotalOtherDeduction1,
                            Exemption = exemption + dependentExemption,
                            Tax = Math.Round(!isMinimumWager ? 0 : tax, 2),
                            TaxWithheld = lwt.SumOfTax,
                            LastTax = Math.Round(!isMinimumWager ? tax - lwt.SumOfTax : 0 - lwt.SumOfTax, 2),
                        });
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}
