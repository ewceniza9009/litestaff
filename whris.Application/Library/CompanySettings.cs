using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.Library
{
    public class CompanySettings
    {
        // Make properties public for access
        public int MandatoryDeductionDivisor { get; }
        public bool IsComputeNightOvertimeOnNonRegularDays { get; }
        public bool IsComputePhicByPercentage { get; }
        public decimal PhicPercentage { get; }
        public bool IsHolidayPayLateDeducted { get; }

        // Private constructor to prevent direct instantiation
        private CompanySettings()
        {
            // *** CRUCIAL: Use a 'using' block to load the data safely ***
            using (var context = new HRISContext())
            {
                var company = context.MstCompanies.FirstOrDefault();

                if (company != null)
                {
                    MandatoryDeductionDivisor = company.MandatoryDeductionDivisor;
                    IsComputeNightOvertimeOnNonRegularDays = company.IsComputeNightOvertimeOnNonRegularDays;
                    IsComputePhicByPercentage = company.IsComputePhicByPercentage;
                    PhicPercentage = company.PhicPercentage;
                    IsHolidayPayLateDeducted = company.IsHolidayPayLateDeducted;
                }
            }
            // The context is safely disposed of here.
        }

        // 2. The static part: A lazy-loaded, thread-safe instance
        private static readonly Lazy<CompanySettings> _lazyInstance =
            new Lazy<CompanySettings>(() => new CompanySettings());

        // 3. The global access point
        public static CompanySettings Instance => _lazyInstance.Value;
    }
}
