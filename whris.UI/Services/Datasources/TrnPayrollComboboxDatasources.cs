using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Models;

namespace whris.UI.Services.Datasources
{
    public class TrnPayrollComboboxDatasources
    {
        public List<MstMonth> MonthCmbDs => (List<MstMonth>)(Common.GetMonths()?.Value ?? new List<MstMonth>());
        public List<TrnDtr> DTRCmbDs = (List<TrnDtr>)(Common.GetDTRs()?.Value ?? new List<TrnDtr>());
        public List<TrnPayrollOtherIncome> PayrollOtherIncomeCmbDs = (List<TrnPayrollOtherIncome>)(Common.GetPayrollOtherIncomes()?.Value ?? new List<TrnPayrollOtherIncome>());
        public List<TrnPayrollOtherDeduction> PayrollOtherDeductionCmbDs = (List<TrnPayrollOtherDeduction>)(Common.GetPayrollOtherDeductions()?.Value ?? new List<TrnPayrollOtherDeduction>());
        public List<TrnLastWithholdingTax> LastWithholdingTaxCmbDs = (List<TrnLastWithholdingTax>)(Common.GetLastWithholdingTaxes()?.Value ?? new List<TrnLastWithholdingTax>());

        public static TrnPayrollComboboxDatasources Instance => new TrnPayrollComboboxDatasources();

        private TrnPayrollComboboxDatasources()
        {

        }
    }
}
