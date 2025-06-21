using Kendo.Mvc.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using whris.Application.Common;
using whris.Application.CQRS.TrnPayroll.Commands;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Library
{
    internal class Payroll
    {
        public static HRISContext _context => new HRISContext();

        static MstCompany company => _context.MstCompanies.FirstOrDefault() ?? new MstCompany();

        static int mandantoryDeductionDivisor = company?.MandatoryDeductionDivisor ?? 0;
        static bool isComputeNightOvertimeOnNonRegularDays => company?.IsComputeNightOvertimeOnNonRegularDays ?? false;
        static bool isComputePhicByPercentage => company?.IsComputePhicByPercentage ?? false;
        static decimal phicPercentage => company?.PhicPercentage ?? 0;
        static bool isHolidayPayLateDeducted => company?.IsHolidayPayLateDeducted ?? false;

        #region Assign Values
        public static decimal GetRegularWorkingHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m; 
            if (!isRestDay && dayType == 1) { result = numberOfHours;  }
            else { result = 0; } 
            return result;
        }

        public static decimal GetLegalHolidayWorkingHours(bool isRestDay, int dayType, decimal numberOfHours) 
        {
            var result = 0m; 
            if (!isRestDay && dayType == 2) {  result = numberOfHours; } else
            { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayWorkingHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m; 
            if (!isRestDay && dayType == 3) { result = numberOfHours; }
            else { result = 0; } 
            return result;
        }

        public static decimal GetRegularRestDayHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m; 
            if (isRestDay && dayType == 1) { result = numberOfHours;  }
            else { result = 0; } 
            return result;
        }

        public static decimal GetLegalHolidayRestDayHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m; 
            if (isRestDay && dayType == 2) {  result = numberOfHours;  }
            else {  result = 0; } 
            return result;
        }

        public static decimal GetSpecialHolidayRestDayHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m; 
            if (isRestDay && dayType == 3) { result = numberOfHours; }
            else {  result = 0; } 
            return result;
        }

        public static decimal GetRegularOvertimeHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m; 
            if (dayType == 1) {  result = numberOfHours; }
            else { result = 0; } 
            return result;
        }

        public static decimal GetLegalHolidayOvertimeHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m; 
            if (dayType == 2) { result = numberOfHours; }
            else { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayOvertimeHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m;
            if (dayType == 3) { result = numberOfHours; }
            else { result = 0; }
            return result;
        }

        public static decimal GetRegularNightHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m;
            if (dayType == 1) { result = numberOfHours; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayNightHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m;
            if (dayType == 2) { result = numberOfHours; }
            else { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayNightHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m;
            if (dayType == 3) { result = numberOfHours; }
            else { result = 0; }
            return result;
        }

        public static decimal GetRegularNightOvertimeHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m;
            if (dayType == 1) { result = numberOfHours; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayNightOvertimeHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m;
            if (dayType == 2) { result = numberOfHours; }
            else { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayNighOvertimetHours(bool isRestDay, int dayType, decimal numberOfHours)
        {
            var result = 0m;
            if (dayType == 3) { result = numberOfHours; }
            else { result = 0; }
            return result;
        }

        public static decimal GetRegularWorkingAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (!isRestDay && dayType == 1) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayWorkingAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (!isRestDay && dayType == 2) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayWorkingAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (!isRestDay && dayType == 3) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetRegularRestdayAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (isRestDay && dayType == 1) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayRestdayAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (isRestDay && dayType == 2) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayRestdayAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (isRestDay && dayType == 3) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetRegularOvertimeAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (dayType == 1) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayOvertimeAmount(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays) 
            {
                return 0;
            }

            var result = 0m;
            if (dayType == 2) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayOvertimeAmountDeduction(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                var result = 0m;
                if (dayType == 2) { result = amount; }
                else { result = 0; }
                return result;
            }

            return 0;
        }

        public static decimal GetSpecialHolidayOvertimeAmount(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                return 0;
            }

            var result = 0m;
            if (dayType == 3) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayOvertimeAmountDeduction(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                var result = 0m;
                if (dayType == 3) { result = amount; }
                else { result = 0; }
                return result;
            }

            return 0;
        }

        public static decimal GetRegularNightAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (dayType == 1) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayNightAmount(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                return 0;
            }

            var result = 0m;
            if (dayType == 2) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayNightAmountDeduction(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                var result = 0m;
                if (dayType == 2) { result = amount; }
                else { result = 0; }
                return result;
            }

            return 0;
        }

        public static decimal GetSpecialHolidayNightAmount(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                return 0;
            }

            var result = 0m;
            if (dayType == 3) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayNightAmountDeduction(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                var result = 0m;
                if (dayType == 3) { result = amount; }
                else { result = 0; }
                return result;
            }

            return 0;
        }

        public static decimal GetRegularNightOvertimeAmount(bool isRestDay, int dayType, decimal amount)
        {
            var result = 0m;
            if (dayType == 1) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayNightOvertimeAmount(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                return 0;
            }

            var result = 0m;
            if (dayType == 2) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetLegalHolidayNightOvertimeAmountDeduction(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                var result = 0m;
                if (dayType == 2) { result = amount; }
                else { result = 0; }
                return result;
            }

            return 0;
        }

        public static decimal GetSpecialHolidayNightOvertimeAmount(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                return 0;
            }

            var result = 0m;
            if (dayType == 3) { result = amount; }
            else { result = 0; }
            return result;
        }

        public static decimal GetSpecialHolidayNightOvertimeAmountDeduction(bool isRestDay, int dayType, decimal amount)
        {
            if (!isComputeNightOvertimeOnNonRegularDays)
            {
                var result = 0m;
                if (dayType == 3) { result = amount; }
                else { result = 0; }
                return result;
            }

            return 0;
        }

        public static decimal ComputeSSSContribution(TrnPayrollLine line) 
        {
            var amount = 0m;
            var sssContribution = 0m;

            var employee = _context.MstEmployees
                .FirstOrDefault(x => x.Id == line.EmployeeId);

            var isExemptedOnMandatoryDeductions = employee
                ?.IsExemptedInMandatoryDeductions ?? false;
            var isSSSForApprove = employee?.Sssnumber?.Contains("FA") ?? true;

            if(isSSSForApprove) return sssContribution;
            if(isExemptedOnMandatoryDeductions) return sssContribution;

            var basic = 0m;
            var sssIsGrossAmount = _context.MstEmployees
                .FirstOrDefault(x => x.Id == line.EmployeeId)
                ?.SssisGrossAmount ?? false;

            var query = new Queries.TrnPayroll.PayrollDetailMandatoryList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            if (sssIsGrossAmount)
            {
                var totalSalaryAmountOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.TotalSalaryAmountOfTheMonth ?? 0m;

                amount = line.TotalSalaryAmount + (mandantoryDeductionDivisor == 1 ? totalSalaryAmountOfTheMonth : 0);
            }
            else 
            {
                var basicAmountOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.BasicAmountOfTheMonth ?? 0m;

                //basic = line.TotalSalaryAmount -
                //   line.TotalLegalHolidayWorkingAmount -
                //   line.TotalSpecialHolidayWorkingAmount -
                //   line.TotalRegularRestdayAmount -
                //   line.TotalLegalHolidayRestdayAmount -
                //   line.TotalSpecialHolidayRestdayAmount -
                //   line.TotalRegularOvertimeAmount -
                //   line.TotalLegalHolidayOvertimeAmount -
                //   line.TotalSpecialHolidayOvertimeAmount -
                //   line.TotalRegularNightAmount -
                //   line.TotalLegalHolidayNightAmount -
                //   line.TotalSpecialHolidayNightAmount -
                //   line.TotalRegularNightOvertimeAmount -
                //   line.TotalLegalHolidayNightOvertimeAmount -
                //   line.TotalSpecialHolidayNightOvertimeAmount;

                basic = Lookup.GetEmployeeBasic(line.EmployeeId);

				amount = basic + (mandantoryDeductionDivisor == 1 ? basicAmountOfTheMonth : 0);
            }

            var employeeContribution = _context.MstTableSsses
                .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                ?.EmployeeContribution ?? 0m;
            var employeeWISP = _context.MstTableSsses
                .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                ?.EmployeeWISP ?? 0m;
            var SSSAddon = _context.MstEmployees.FirstOrDefault(x => x.Id == line.EmployeeId)?.SssaddOn ?? 0m;
            var sSSContributionOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.SSSContributionOfTheMonth ?? 0m;

            line.OrigSSSContribution = (employeeContribution / mandantoryDeductionDivisor);
            line.SSSWISP = employeeWISP / mandantoryDeductionDivisor;

            sssContribution = (employeeContribution/mandantoryDeductionDivisor) + (employeeWISP/mandantoryDeductionDivisor) + SSSAddon + (mandantoryDeductionDivisor == 1 ? sSSContributionOfTheMonth : 0);

            return sssContribution;
        }

        public static decimal ComputeSSSECContribution(TrnPayrollLine line)
        {
            var amount = 0m;
            var sssEcContribution = 0m;

            var employee = _context.MstEmployees
                .FirstOrDefault(x => x.Id == line.EmployeeId);

            var isExemptedOnMandatoryDeductions = employee
                ?.IsExemptedInMandatoryDeductions ?? false;
            var isSSSForApprove = employee?.Sssnumber?.Contains("FA") ?? true;

            if (isSSSForApprove) return sssEcContribution;
            if (isExemptedOnMandatoryDeductions) return sssEcContribution;

            var basic = 0m;
            var sssIsGrossAmount = _context.MstEmployees
                .FirstOrDefault(x => x.Id == line.EmployeeId)
                ?.SssisGrossAmount ?? false;

            var query = new Queries.TrnPayroll.PayrollDetailMandatoryList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            if (sssIsGrossAmount)
            {
                var totalSalaryAmountOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.TotalSalaryAmountOfTheMonth ?? 0m;

                amount = line.TotalSalaryAmount + (mandantoryDeductionDivisor == 1 ? totalSalaryAmountOfTheMonth : 0);
            }
            else
            {
                var basicAmountOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.BasicAmountOfTheMonth ?? 0m;

				//basic = line.TotalSalaryAmount -
				//   line.TotalLegalHolidayWorkingAmount -
				//   line.TotalSpecialHolidayWorkingAmount -
				//   line.TotalRegularRestdayAmount -
				//   line.TotalLegalHolidayRestdayAmount -
				//   line.TotalSpecialHolidayRestdayAmount -
				//   line.TotalRegularOvertimeAmount -
				//   line.TotalLegalHolidayOvertimeAmount -
				//   line.TotalSpecialHolidayOvertimeAmount -
				//   line.TotalRegularNightAmount -
				//   line.TotalLegalHolidayNightAmount -
				//   line.TotalSpecialHolidayNightAmount -
				//   line.TotalRegularNightOvertimeAmount -
				//   line.TotalLegalHolidayNightOvertimeAmount -
				//   line.TotalSpecialHolidayNightOvertimeAmount;

				basic = Lookup.GetEmployeeBasic(line.EmployeeId);

				amount = basic + (mandantoryDeductionDivisor == 1 ? basicAmountOfTheMonth : 0);
            }

            var employeeEc = _context.MstTableSsses
                .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                ?.EmployeeEc ?? 0m;
            var sSSEcContributionOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.SSSECContributionOfTheMonth ?? 0m;

            sssEcContribution = (employeeEc / mandantoryDeductionDivisor) + (mandantoryDeductionDivisor == 1 ? sSSEcContributionOfTheMonth : 0);

            return sssEcContribution;
        }

        public static decimal ComputeSSSContributionEmployer(TrnPayrollLine line)
        {
            var amount = 0m;
            var sSSECContributionEmployer = 0m;

            var employee = _context.MstEmployees
                .FirstOrDefault(x => x.Id == line.EmployeeId);

            var isExemptedOnMandatoryDeductions = employee
                ?.IsExemptedInMandatoryDeductions ?? false;
            var isSSSForApprove = employee?.Sssnumber?.Contains("FA") ?? true;

            if (isSSSForApprove) return sSSECContributionEmployer;
            if (isExemptedOnMandatoryDeductions) return sSSECContributionEmployer;

            var basic = 0m;

            var query = new Queries.TrnPayroll.PayrollDetailMandatoryList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            var basicAmountOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.BasicAmountOfTheMonth ?? 0m;

			//basic = line.TotalSalaryAmount -
			//    line.TotalLegalHolidayWorkingAmount -
			//    line.TotalSpecialHolidayWorkingAmount -
			//    line.TotalRegularRestdayAmount -
			//    line.TotalLegalHolidayRestdayAmount -
			//    line.TotalSpecialHolidayRestdayAmount -
			//    line.TotalRegularOvertimeAmount -
			//    line.TotalLegalHolidayOvertimeAmount -
			//    line.TotalSpecialHolidayOvertimeAmount -
			//    line.TotalRegularNightAmount -
			//    line.TotalLegalHolidayNightAmount -
			//    line.TotalSpecialHolidayNightAmount -
			//    line.TotalRegularNightOvertimeAmount -
			//    line.TotalLegalHolidayNightOvertimeAmount -
			//    line.TotalSpecialHolidayNightOvertimeAmount;

			basic = Lookup.GetEmployeeBasic(line.EmployeeId);

			amount = basic + (mandantoryDeductionDivisor == 1 ? basicAmountOfTheMonth : 0);
            var employerContribution = _context.MstTableSsses
                .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                ?.EmployerContribution ?? 0m;
            var employerWISP = _context.MstTableSsses
                .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                ?.EmployerWISP ?? 0m;
            var sSSContributionEmployerOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.SSSContributionEmployerOfTheMonth ?? 0m;

            sSSECContributionEmployer = (employerContribution / mandantoryDeductionDivisor) + (employerWISP / mandantoryDeductionDivisor) +  + (mandantoryDeductionDivisor == 1 ? sSSContributionEmployerOfTheMonth : 0);

            return sSSECContributionEmployer;
        }

        public static decimal ComputeSSSECContributionEmployer(TrnPayrollLine line)
        {
            var amount = 0m;
            var sSSECContributionEmployer = 0m;

            var employee = _context.MstEmployees
                 .FirstOrDefault(x => x.Id == line.EmployeeId);

            var isExemptedOnMandatoryDeductions = employee
                ?.IsExemptedInMandatoryDeductions ?? false;
            var isSSSForApprove = employee?.Sssnumber?.Contains("FA") ?? true;

            if (isSSSForApprove) return sSSECContributionEmployer;
            if (isExemptedOnMandatoryDeductions) return sSSECContributionEmployer;

            var basic = 0m;

            var query = new Queries.TrnPayroll.PayrollDetailMandatoryList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            var basicAmountOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.BasicAmountOfTheMonth ?? 0m;

			//basic = line.TotalSalaryAmount -
			//    line.TotalLegalHolidayWorkingAmount -
			//    line.TotalSpecialHolidayWorkingAmount -
			//    line.TotalRegularRestdayAmount -
			//    line.TotalLegalHolidayRestdayAmount -
			//    line.TotalSpecialHolidayRestdayAmount -
			//    line.TotalRegularOvertimeAmount -
			//    line.TotalLegalHolidayOvertimeAmount -
			//    line.TotalSpecialHolidayOvertimeAmount -
			//    line.TotalRegularNightAmount -
			//    line.TotalLegalHolidayNightAmount -
			//    line.TotalSpecialHolidayNightAmount -
			//    line.TotalRegularNightOvertimeAmount -
			//    line.TotalLegalHolidayNightOvertimeAmount -
			//    line.TotalSpecialHolidayNightOvertimeAmount;

			basic = Lookup.GetEmployeeBasic(line.EmployeeId);

			amount = basic + (mandantoryDeductionDivisor == 1 ? basicAmountOfTheMonth : 0);
            var employerEc = _context.MstTableSsses
                .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                ?.EmployerEc ?? 0m;
            var sSSECContributionEmployerOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.SSSECContributionEmployerOfTheMonth ?? 0m;

            sSSECContributionEmployer = (employerEc / mandantoryDeductionDivisor) + (mandantoryDeductionDivisor == 1 ? sSSECContributionEmployerOfTheMonth : 0);

            return sSSECContributionEmployer;
        }

        public static decimal ComputePHICContribution(TrnPayrollLine line)
        {
            var amount = 0m;
            var phicContribution = 0m;

            var employee = _context.MstEmployees
                .FirstOrDefault(x => x.Id == line.EmployeeId);

            var isExemptedOnMandatoryDeductions = employee
                ?.IsExemptedInMandatoryDeductions ?? false;
            var isPHICForApprove = employee?.Phicnumber?.Contains("FA") ?? true;

            if (isPHICForApprove) return phicContribution;
            if (isExemptedOnMandatoryDeductions) return phicContribution;

            var basic = 0m;

            var query = new Queries.TrnPayroll.PayrollDetailMandatoryList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            var basicAmountOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.BasicAmountOfTheMonth ?? 0m;

			//basic = line.TotalSalaryAmount -
			//    line.TotalLegalHolidayWorkingAmount -
			//    line.TotalSpecialHolidayWorkingAmount -
			//    line.TotalRegularRestdayAmount -
			//    line.TotalLegalHolidayRestdayAmount -
			//    line.TotalSpecialHolidayRestdayAmount -
			//    line.TotalRegularOvertimeAmount -
			//    line.TotalLegalHolidayOvertimeAmount -
			//    line.TotalSpecialHolidayOvertimeAmount -
			//    line.TotalRegularNightAmount -
			//    line.TotalLegalHolidayNightAmount -
			//    line.TotalSpecialHolidayNightAmount -
			//    line.TotalRegularNightOvertimeAmount -
			//    line.TotalLegalHolidayNightOvertimeAmount -
			//    line.TotalSpecialHolidayNightOvertimeAmount;

			basic = Lookup.GetEmployeeBasic(line.EmployeeId);

			amount = basic + (mandantoryDeductionDivisor == 1 ? basicAmountOfTheMonth : 0);
            var employeeContribution = _context.MstTablePhics
                .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                ?.EmployeeContribution ?? 0m;
            var phicContributionOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.PHICContributionOfTheMonth ?? 0m;

            phicContribution = (employeeContribution / mandantoryDeductionDivisor) + (mandantoryDeductionDivisor == 1 ? phicContributionOfTheMonth : 0);

            if (isComputePhicByPercentage && phicPercentage > 0) 
            {
                phicContribution = basic * phicPercentage/100;
            }

            return phicContribution;
        }

        public static decimal ComputePHICContributionEmployer(TrnPayrollLine line)
        {
            var amount = 0m;
            var phicContributionEmployer = 0m;

            var employee = _context.MstEmployees
                .FirstOrDefault(x => x.Id == line.EmployeeId);

            var isExemptedOnMandatoryDeductions = employee
                ?.IsExemptedInMandatoryDeductions ?? false;
            var isPHICForApprove = employee?.Phicnumber?.Contains("FA") ?? true;

            if (isPHICForApprove) return phicContributionEmployer;
            if (isExemptedOnMandatoryDeductions) return phicContributionEmployer;

            var basic = 0m;

            var query = new Queries.TrnPayroll.PayrollDetailMandatoryList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            var basicAmountOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.BasicAmountOfTheMonth ?? 0m;

			//basic = line.TotalSalaryAmount -
			//    line.TotalLegalHolidayWorkingAmount -
			//    line.TotalSpecialHolidayWorkingAmount -
			//    line.TotalRegularRestdayAmount -
			//    line.TotalLegalHolidayRestdayAmount -
			//    line.TotalSpecialHolidayRestdayAmount -
			//    line.TotalRegularOvertimeAmount -
			//    line.TotalLegalHolidayOvertimeAmount -
			//    line.TotalSpecialHolidayOvertimeAmount -
			//    line.TotalRegularNightAmount -
			//    line.TotalLegalHolidayNightAmount -
			//    line.TotalSpecialHolidayNightAmount -
			//    line.TotalRegularNightOvertimeAmount -
			//    line.TotalLegalHolidayNightOvertimeAmount -
			//    line.TotalSpecialHolidayNightOvertimeAmount;

			basic = Lookup.GetEmployeeBasic(line.EmployeeId);

			amount = basic + (mandantoryDeductionDivisor == 1 ? basicAmountOfTheMonth : 0);
            var employerContribution = _context.MstTablePhics
                .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                ?.EmployerContribution ?? 0m;
            var phicContributionEmployerOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.PHICContributionEmployerOfTheMonth ?? 0m;

            phicContributionEmployer = (employerContribution / mandantoryDeductionDivisor) + (mandantoryDeductionDivisor == 1 ? phicContributionEmployerOfTheMonth : 0);

            if (isComputePhicByPercentage && phicPercentage > 0)
            {
                phicContributionEmployer = basic * phicPercentage / 100;
            }

            return phicContributionEmployer;
        }

        public static decimal ComputeHDMFContribution(TrnPayrollLine line)
        {
            var amount = 0m;
            var hdmfContribution = 0m;

            var employee = _context.MstEmployees
               .FirstOrDefault(x => x.Id == line.EmployeeId);

            var isExemptedOnMandatoryDeductions = employee
                ?.IsExemptedInMandatoryDeductions ?? false;
            var isHDMFForApprove = employee?.Hdmfnumber?.Contains("FA") ?? true;

            if (isHDMFForApprove) return hdmfContribution;
            if (isExemptedOnMandatoryDeductions) return hdmfContribution;

            var basic = 0m;

            var query = new Queries.TrnPayroll.PayrollDetailMandatoryList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            var basicAmountOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.BasicAmountOfTheMonth ?? 0m;

            //basic = line.TotalSalaryAmount -
            //    line.TotalLegalHolidayWorkingAmount -
            //    line.TotalSpecialHolidayWorkingAmount -
            //    line.TotalRegularRestdayAmount -
            //    line.TotalLegalHolidayRestdayAmount -
            //    line.TotalSpecialHolidayRestdayAmount -
            //    line.TotalRegularOvertimeAmount -
            //    line.TotalLegalHolidayOvertimeAmount -
            //    line.TotalSpecialHolidayOvertimeAmount -
            //    line.TotalRegularNightAmount -
            //    line.TotalLegalHolidayNightAmount -
            //    line.TotalSpecialHolidayNightAmount -
            //    line.TotalRegularNightOvertimeAmount -
            //    line.TotalLegalHolidayNightOvertimeAmount -
            //    line.TotalSpecialHolidayNightOvertimeAmount;

            basic = Lookup.GetEmployeeBasic(line.EmployeeId);

			amount = basic + (mandantoryDeductionDivisor == 1 ? basicAmountOfTheMonth : 0);

            var hdmfType = _context.MstEmployees.FirstOrDefault(x => x.Id == line.EmployeeId)?.Hdmftype ?? "";

            if (hdmfType == "Percentage")
            {
                var employeePercentage = _context.MstTableHdmfs
                    .FirstOrDefault(x => (x.AmountStart <= amount / mandantoryDeductionDivisor) && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                    ?.EmployeePercentage ?? 0;
                var hdmfContributionOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.HDMFContributionOfTheMonth ?? 0m;

                hdmfContribution = (amount * (employeePercentage / 100)) / mandantoryDeductionDivisor;
                hdmfContribution = hdmfContribution - (mandantoryDeductionDivisor == 1 ? hdmfContributionOfTheMonth : 0);
            }
            else 
            {
                var employeeValue = _context.MstTableHdmfs
                   .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                   ?.EmployeeValue ?? 0;

                hdmfContribution = employeeValue / mandantoryDeductionDivisor;
            }

            var hrmdAddOn = _context.MstEmployees
                .FirstOrDefault(x => x.Id == line.EmployeeId)
                ?.HdmfaddOn ?? 0;

            return Math.Round(hdmfContribution, 2) + hrmdAddOn; // > 100 ? 100 : Math.Round(hdmfContribution, 2) + hrmdAddOn;
        }

        public static decimal ComputeHDMFContributionEmployer(TrnPayrollLine line)
        {
            var amount = 0m;
            var hdmfContributionEmployer = 0m;

            var employee = _context.MstEmployees
               .FirstOrDefault(x => x.Id == line.EmployeeId);

            var isExemptedOnMandatoryDeductions = employee
                ?.IsExemptedInMandatoryDeductions ?? false;
            var isHDMFForApprove = employee?.Hdmfnumber?.Contains("FA") ?? true;

            if (isHDMFForApprove) return hdmfContributionEmployer;
            if (isExemptedOnMandatoryDeductions) return hdmfContributionEmployer;

            var basic = 0m;

            var query = new Queries.TrnPayroll.PayrollDetailMandatoryList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            var basicAmountOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.BasicAmountOfTheMonth ?? 0m;

			//basic = line.TotalSalaryAmount -
			//    line.TotalLegalHolidayWorkingAmount -
			//    line.TotalSpecialHolidayWorkingAmount -
			//    line.TotalRegularRestdayAmount -
			//    line.TotalLegalHolidayRestdayAmount -
			//    line.TotalSpecialHolidayRestdayAmount -
			//    line.TotalRegularOvertimeAmount -
			//    line.TotalLegalHolidayOvertimeAmount -
			//    line.TotalSpecialHolidayOvertimeAmount -
			//    line.TotalRegularNightAmount -
			//    line.TotalLegalHolidayNightAmount -
			//    line.TotalSpecialHolidayNightAmount -
			//    line.TotalRegularNightOvertimeAmount -
			//    line.TotalLegalHolidayNightOvertimeAmount -
			//    line.TotalSpecialHolidayNightOvertimeAmount;

			basic = Lookup.GetEmployeeBasic(line.EmployeeId);

			amount = basic + basicAmountOfTheMonth;

            var hdmfType = _context.MstEmployees.FirstOrDefault(x => x.Id == line.EmployeeId)?.Hdmftype ?? "";

            if (hdmfType == "Percentage")
            {
                var employerPercentage = _context.MstTableHdmfs
                    .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                    ?.EmployerPercentage ?? 0;
                var hdmfContributionOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.HDMFContributionOfTheMonth ?? 0m;

                hdmfContributionEmployer = (amount * (employerPercentage / 100)) / mandantoryDeductionDivisor;
                hdmfContributionEmployer = hdmfContributionEmployer - (mandantoryDeductionDivisor == 1 ? hdmfContributionOfTheMonth : 0);
            }
            else
            {
                var employerValue = _context.MstTableHdmfs
                   .FirstOrDefault(x => (x.AmountStart / mandantoryDeductionDivisor) <= amount && (x.AmountEnd / mandantoryDeductionDivisor) >= amount)
                   ?.EmployerValue ?? 0;

                hdmfContributionEmployer = employerValue / mandantoryDeductionDivisor;
            }

            return Math.Round(hdmfContributionEmployer, 2); //> 100 ? 100 : Math.Round(hdmfContributionEmployer, 2);
        }

        public static decimal ComputeTax(TrnPayrollLine line)
        {
            var query = new Queries.TrnPayroll.PayrollDetailWithholdingList();

            query.PayrollId = line.PayrollId;
            query.MonthId = _context.TrnPayrolls.FirstOrDefault(x => x.Id == line.PayrollId)?.MonthId ?? 0;

            var dblTotalSalaryAmount = 0m;
            var dblTotalOtherIncomeTaxable = 0m;
            var dblTaxOfTheMonth = query.Result()
                .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                ?.TaxOfTheMonth ?? 0m;

            var dblCompensationLevel = 0m;
            var dblCompensationLevelTax = 0m;
            var dblCompensationLevelPercentage = 0m;
            var dblExcess = 0m;
            var dblExcessTax = 0m;
            var dblTax = 0m;

            var taxTable = _context.MstEmployees.FirstOrDefault(x => x.Id == line.EmployeeId)?.TaxTable ?? "";

            if (taxTable == "Monthly")
            {
                var totalSalaryAmountOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.TotalSalaryAmountOfTheMonth ?? 0m;

                var sssContributionOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.SSSContributionOfTheMonth ?? 0m;

                var phicContributionOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.PHICContributionOfTheMonth ?? 0m;

                var hdmfContributionOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.HDMFContributionOfTheMonth ?? 0m;

                dblTotalSalaryAmount = line.TotalNetSalaryAmount +
                    totalSalaryAmountOfTheMonth +
                    line.Ssscontribution +
                    line.Phiccontribution +
                    line.Hdmfcontribution +
                    sssContributionOfTheMonth +
                    phicContributionOfTheMonth +
                    hdmfContributionOfTheMonth;
            }
            else 
            {
                dblTotalSalaryAmount = line.TotalNetSalaryAmount +
                    line.Ssscontribution +
                    line.Phiccontribution +
                    line.Hdmfcontribution;
            }

            if (taxTable == "Monthly")
            {
                dblCompensationLevel = _context.MstTableWtaxMonthlies
                    .Where(x => x.TaxCodeId == line.TaxCodeId && x.Amount < dblTotalSalaryAmount)
                    ?.Max(x => (decimal?)x.Amount) ?? 0;

                dblCompensationLevelTax = _context.MstTableWtaxMonthlies
                    .Where(x => x.TaxCodeId == line.TaxCodeId && x.Amount == dblTotalSalaryAmount)
                    ?.Max(x => (decimal?)x.Tax) ?? 0;

                dblCompensationLevelPercentage = _context.MstTableWtaxMonthlies
                    .Where(x => x.TaxCodeId == line.TaxCodeId && x.Amount == dblTotalSalaryAmount)
                    ?.Max(x => (decimal?)x.Percentage) ?? 0;
            }
            else 
            {
                dblCompensationLevel = _context.MstTableWtaxSemiMonthlies
                        .Where(x => x.TaxCodeId == line.TaxCodeId && x.Amount < dblTotalSalaryAmount)
                        ?.Max(x => (decimal?)x.Amount) ?? 0;

                dblCompensationLevelTax = _context.MstTableWtaxSemiMonthlies
                    .Where(x => x.TaxCodeId == line.TaxCodeId && x.Amount == dblTotalSalaryAmount)
                    ?.Max(x => (decimal?)x.Tax) ?? 0;

                dblCompensationLevelPercentage = _context.MstTableWtaxSemiMonthlies
                    .Where(x => x.TaxCodeId == line.TaxCodeId && x.Amount == dblTotalSalaryAmount)
                    ?.Max(x => (decimal?)x.Percentage) ?? 0;
            }

            dblExcess = dblTotalSalaryAmount - dblCompensationLevel;

            if (taxTable == "Monthly")
            {
                var totalOtherIncomeTaxableOfTheMonth = query.Result()
                    .FirstOrDefault(x => x.EmployeeId == line.EmployeeId)
                    ?.TotalOtherIncomeTaxableOfTheMonth ?? 0m;

                dblTotalOtherIncomeTaxable = line.TotalOtherIncomeTaxable + totalOtherIncomeTaxableOfTheMonth;
            }
            else 
            {
                dblTotalOtherIncomeTaxable = line.TotalOtherIncomeTaxable;
            }

            if (dblCompensationLevelPercentage == 0) 
            {
                if (taxTable == "Monthly")
                {
                    dblCompensationLevelPercentage = _context.MstTableWtaxMonthlies
                        .FirstOrDefault(x => x.TaxCodeId == line.TaxCodeId && 
                            x.Amount < dblTotalSalaryAmount + dblTotalOtherIncomeTaxable)
                        ?.Percentage ?? 0;
                }
                else 
                {
                    dblCompensationLevelPercentage = _context.MstTableWtaxSemiMonthlies
                        .FirstOrDefault(x => x.TaxCodeId == line.TaxCodeId &&
                            x.Amount < dblTotalSalaryAmount + dblTotalOtherIncomeTaxable)
                        ?.Percentage ?? 0;
                }
            }

            dblExcessTax = (dblExcess + dblTotalOtherIncomeTaxable) * (dblCompensationLevelPercentage / 100);
            dblTax = dblCompensationLevelTax + dblExcessTax;

            return Math.Round(dblTax, 2);
        }
        #endregion

        #region Edit Payroll Lines based on DTR Line inputs
        internal static async Task ProcessDtrLines(AddPayrollLinesByProcessDtr command)
        {
            try 
            {
                using (var ctx = new HRISContext())
                {
                    if (command.EmployeeId is null)
                    {
                        var result = ctx.Database.ExecuteSqlRaw($"DELETE FROM TrnPayrollLine WHERE PayrollId={command.PayrollId}");
                    }
                    else
                    {
                        var result = ctx.Database.ExecuteSqlRaw($"DELETE FROM TrnPayrollLine WHERE PayrollId={command.PayrollId} AND EmployeeId={command.EmployeeId}");
                    }

                    ctx.SaveChanges();

                    var trnDtrLines = ctx.TrnDtrlines
                        .Include(x => x.Employee)
                        .Where(x => x.Dtrid == command.DtrId)
                        .GroupBy(x => new
                        {
                            x.EmployeeId,
                            x.Employee.PayrollTypeId,
                            x.Employee.TaxCodeId,
                            x.Employee.AccountId,
                            x.Employee.HourlyRate,
                            x.Employee.DailyRate,
                            x.Employee.PayrollRate,
                            x.RestDay,
                            x.DayTypeId
                        })
                        .ToList();

                    if (command.EmployeeId is not null)
                    {
                        trnDtrLines = ctx.TrnDtrlines
                        .Include(x => x.Employee)
                        .Where(x => x.Dtrid == command.DtrId && x.EmployeeId == command.EmployeeId)
                        .GroupBy(x => new
                        {
                            x.EmployeeId,
                            x.Employee.PayrollTypeId,
                            x.Employee.TaxCodeId,
                            x.Employee.AccountId,
                            x.Employee.HourlyRate,
                            x.Employee.DailyRate,                           
                            x.Employee.PayrollRate,
                            x.RestDay,
                            x.DayTypeId
                        })
                        .ToList();
                    }

                    var insertedEmployeeIds = new List<int>();
                    var newPayrollLine = new TrnPayrollLine();
                    var dtrPerEmpCounter = 0;

                    var employeeDic = _context.MstEmployees
                        .Select(x => new { Id = x.Id, Locked = x.IsLocked })
                        .ToDictionary(x => x.Id, x => x.Locked);

                    foreach (var line in trnDtrLines)
                    {
                        var dtrPerEmpCount = trnDtrLines.Where(x => x.Key.EmployeeId == line.Key.EmployeeId).Count();
                        var employmentType = ctx.MstEmployees.FirstOrDefault(x => x.Id == line.Key.EmployeeId)?.EmploymentType ?? 0;

                        if (employeeDic[line.Key.EmployeeId] == false)
                        {
                            continue;
                        }

                        dtrPerEmpCounter++;

                        if (dtrPerEmpCounter == 1)
                        {
                            newPayrollLine = new TrnPayrollLine();
                        }

                        newPayrollLine.PayrollId = command.PayrollId;
                        newPayrollLine.EmployeeId = line.Key.EmployeeId;
                        newPayrollLine.PayrollTypeId = line.Key.PayrollTypeId;
                        newPayrollLine.TaxCodeId = line.Key.TaxCodeId;
                        newPayrollLine.AccountId = line.Key.AccountId;

                        newPayrollLine.PayrollRate = line.Key.PayrollRate;                        

                        var regAmount = line.Where(x => x.NetAmount != x.RegularAmount).Sum(x => x.RegularAmount);
                        var legal = line.Sum(x => x.NetAmount) == line.Sum(x => x.RegularAmount) ?
                            line.Sum(x => x.RegularAmount) :
                            (line.Sum(x => x.NetAmount) + line.Sum(x => x.TardyAmount)) - regAmount;
                        var specAmount = line.Where(x => x.NetAmount != x.RegularAmount).Sum(x => x.RegularAmount);
                        var special = (line.Sum(x => x.NetAmount) + line.Sum(x => x.TardyAmount)) - specAmount;

                        var legalNet = legal - line.Sum(x => x.OvertimeAmount) -
                            line.Sum(x => x.NightAmount) -
                            line.Sum(x => x.OvertimeNightAmount);

                        var specialNet = special - line.Sum(x => x.OvertimeAmount) -
                            line.Sum(x => x.NightAmount) -
                            line.Sum(x => x.OvertimeNightAmount);

                        if (line.Key.PayrollTypeId == 3 && line.Key.DayTypeId > 1 && line.Sum(x => x.NetAmount) > 0)
                        {
                            legal = line.Sum(x => x.RegularAmount);
                            legalNet = line.Sum(x => x.RegularAmount);

                            special = line.Sum(x => x.RegularAmount) * 0.3m;
                            specialNet = line.Sum(x => x.RegularAmount) * 0.3m;
                        }                        

                        //var legalHours = line.Sum(x => x.RegularHours);
                        //var specialHours = line.Sum(x => x.RegularHours);

                        if (line.Key.HourlyRate == 0)
                        {
                            var empName = _context.MstEmployees.FirstOrDefault(x => x.Id == line.Key.EmployeeId)?.FullName ?? "";
                            throw new Exception($"{empName} has no hourly rate");
                        }

                        var legalHours = legalNet / line.Key.HourlyRate;
                        var specialHours = specialNet / (line.Key.HourlyRate * 0.3m); //Hardcoded please fix in the future

                        var holidayHoursWithNoPayLegal = 0m;
                        var holidayHoursWithNoPaySpecial = 0m;

                        if (line.Key.DayTypeId == 2)
                        {
                            holidayHoursWithNoPayLegal = line.Sum(x => x.RegularHours) - legalHours;
                        }

                        if (line.Key.DayTypeId == 3)
                        {
                            holidayHoursWithNoPaySpecial = line.Sum(x => x.RegularHours) - specialHours;
                        }

                        var totalRegularWorkingHours = GetRegularWorkingHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.RegularHours)) + holidayHoursWithNoPayLegal + holidayHoursWithNoPaySpecial;
                        var totalLegalHolidayWorkingHours = GetLegalHolidayWorkingHours(line.Key.RestDay, line.Key.DayTypeId, legalHours);
                        var totalSpecialHolidayWorkingHours = GetSpecialHolidayWorkingHours(line.Key.RestDay, line.Key.DayTypeId, specialHours);

                        var dayTypeRestDayMultiplier = ctx.MstDayTypes.Where(x => x.Id == line.Key.DayTypeId).FirstOrDefault()?.RestdayDays ?? 1;

                        var totalRegularRestdayHours = GetRegularRestDayHours(line.Key.RestDay, line.Key.DayTypeId, (line.Key.RestDay && dayTypeRestDayMultiplier == 1 ? 0 : line.Sum(x => x.RegularHours)));
                        var totalLegalHolidayRestdayHours = GetLegalHolidayRestDayHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.RegularHours));
                        var totalSpecialHolidayRestdayHours = GetSpecialHolidayRestDayHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.RegularHours));
                        var totalRegularOvertimeHours = GetRegularOvertimeHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeHours));
                        var totalLegalHolidayOvertimeHours = GetLegalHolidayOvertimeHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeHours));
                        var totalSpecialHolidayOvertimeHours = GetSpecialHolidayOvertimeHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeHours));
                        var totalRegularNightHours = GetRegularNightHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.NightHours));
                        var totalLegalHolidayNightHours = GetLegalHolidayNightHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.NightHours));
                        var totalSpecialHolidayNightHours = GetSpecialHolidayNightHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.NightHours));
                        var totalRegularNightOvertimeHours = GetRegularNightOvertimeHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeNightHours));
                        var totalLegalHolidayNightOvertimeHours = GetLegalHolidayNightOvertimeHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeNightHours));
                        var totalSpecialHolidayNighOvertimetHours = GetSpecialHolidayNighOvertimetHours(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeNightHours));
                        var totalTardyLateHours = line.Sum(x => x.TardyLateHours);
                        var totalTardyUndertimeHours = line.Sum(x => x.TardyUndertimeHours);
                        var totalRegularWorkingAmount = GetRegularWorkingAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.TotalAmount));

                        var totalLegalHolidayWorkingAmount = GetLegalHolidayWorkingAmount(line.Key.RestDay, line.Key.DayTypeId,
                                line.Sum(x => x.NetAmount) == 0 ? 0 : legal -
                                line.Sum(x => x.OvertimeAmount) -
                                line.Sum(x => x.NightAmount) -
                                line.Sum(x => x.OvertimeNightAmount));                       

                        var totalSpecialHolidayWorkingAmount = GetSpecialHolidayWorkingAmount(line.Key.RestDay, line.Key.DayTypeId,
                                line.Sum(x => x.NetAmount) == 0 ? 0 : special -
                                line.Sum(x => x.OvertimeAmount) -
                                line.Sum(x => x.NightAmount) -
                                line.Sum(x => x.OvertimeNightAmount));

                        if (line.Key.PayrollTypeId == 3) 
                        {
                            totalLegalHolidayWorkingAmount = GetLegalHolidayWorkingAmount(line.Key.RestDay, line.Key.DayTypeId,
                               line.Sum(x => x.NetAmount) == 0 ? 0 : legal);

                            totalSpecialHolidayWorkingAmount = GetSpecialHolidayWorkingAmount(line.Key.RestDay, line.Key.DayTypeId,
                                line.Sum(x => x.NetAmount) == 0 ? 0 : special);
                        }

                        var totalRegularRestdayAmount = GetRegularRestdayAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.TotalAmount) -
                                line.Sum(x => x.OvertimeAmount) -
                                line.Sum(x => x.NightAmount) -
                                line.Sum(x => x.OvertimeNightAmount));
                        var totalLegalHolidayRestdayAmount = GetLegalHolidayRestdayAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.TotalAmount) -
                                line.Sum(x => x.OvertimeAmount) -
                                line.Sum(x => x.NightAmount) -
                                line.Sum(x => x.OvertimeNightAmount));
                        var totalSpecialHolidayRestdayAmount = GetSpecialHolidayRestdayAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.TotalAmount) -
                                line.Sum(x => x.OvertimeAmount) -
                                line.Sum(x => x.NightAmount) -
                                line.Sum(x => x.OvertimeNightAmount));
                        var totalRegularOvertimeAmount = GetRegularOvertimeAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeAmount));
                        var totalLegalHolidayOvertimeAmount = GetLegalHolidayOvertimeAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeAmount));
                        var totalLegalHolidayOvertimeAmountDeduction = GetLegalHolidayOvertimeAmountDeduction(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeAmount));
                        var totalSpecialHolidayOvertimeAmount = GetSpecialHolidayOvertimeAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeAmount));
                        var totalSpecialHolidayOvertimeAmountDeduction = GetSpecialHolidayOvertimeAmountDeduction(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeAmount));
                        var totalRegularNightAmount = GetRegularNightAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.NightAmount));
                        var totalLegalHolidayNightAmount = GetLegalHolidayNightAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.NightAmount));
                        var totalLegalHolidayNightAmountDeduction = GetLegalHolidayNightAmountDeduction(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.NightAmount));
                        var totalSpecialHolidayNightAmount = GetSpecialHolidayNightAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.NightAmount));
                        var totalSpecialHolidayNightAmountDeduction = GetSpecialHolidayNightAmountDeduction(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.NightAmount));
                        var totalRegularNightOvertimeAmount = GetRegularNightOvertimeAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeNightAmount));
                        var totalLegalHolidayNightOvertimeAmount = GetLegalHolidayNightOvertimeAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeNightAmount));
                        var totalLegalHolidayNightOvertimeAmountDeduction = GetLegalHolidayNightOvertimeAmountDeduction(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeNightAmount));
                        var totalSpecialHolidayNightOvertimeAmount = GetSpecialHolidayNightOvertimeAmount(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeNightAmount));
                        var totalSpecialHolidayNightOvertimeAmountDeduction = GetSpecialHolidayNightOvertimeAmountDeduction(line.Key.RestDay, line.Key.DayTypeId, line.Sum(x => x.OvertimeNightAmount));
                        var totalSalaryAmount = line.Sum(x => x.TotalAmount);

                        var holidayTotalLateHours = 0m;
                        var holidayTotalUnderTimeHours = 0m;
                        var holidayTotalLateAmount = 0m;

                        if (isHolidayPayLateDeducted)
                        {
                            holidayTotalLateHours = (line.Where(x => x.DayTypeId > 1)?.Sum(x => x.TardyLateHours) ?? 0);
                            holidayTotalUnderTimeHours = (line.Where(x => x.DayTypeId > 1)?.Sum(x => x.TardyUndertimeHours) ?? 0);
                            holidayTotalLateAmount = line.Where(x => x.DayTypeId > 1)?.Sum(x => x.TardyAmount) ?? 0m;
                        }

                        totalTardyLateHours = totalTardyLateHours + holidayTotalLateHours;
                        holidayTotalUnderTimeHours = holidayTotalUnderTimeHours + holidayTotalUnderTimeHours;

                        var totalTardyAmount = line.Sum(x => x.TardyAmount) + holidayTotalLateAmount;
                        var totalAbsentAmount = line.Sum(x => x.AbsentAmount);
                        var totalNetSalaryAmount = line.Sum(x => x.NetAmount) - holidayTotalLateAmount;

                        newPayrollLine.TotalRegularWorkingHours += totalRegularWorkingHours;
                        newPayrollLine.TotalLegalHolidayWorkingHours += totalLegalHolidayWorkingHours;
                        newPayrollLine.TotalSpecialHolidayWorkingHours += totalSpecialHolidayWorkingHours;
                        newPayrollLine.TotalRegularRestdayHours += totalRegularRestdayHours;
                        newPayrollLine.TotalLegalHolidayRestdayHours += totalLegalHolidayRestdayHours;
                        newPayrollLine.TotalSpecialHolidayRestdayHours += totalSpecialHolidayRestdayHours;
                        newPayrollLine.TotalRegularOvertimeHours += totalRegularOvertimeHours;
                        newPayrollLine.TotalLegalHolidayOvertimeHours += totalLegalHolidayOvertimeHours;
                        newPayrollLine.TotalSpecialHolidayOvertimeHours += totalSpecialHolidayOvertimeHours;
                        newPayrollLine.TotalRegularNightHours += totalRegularNightHours;
                        newPayrollLine.TotalLegalHolidayNightHours += totalLegalHolidayNightHours;
                        newPayrollLine.TotalSpecialHolidayNightHours += totalSpecialHolidayNightHours;
                        newPayrollLine.TotalRegularNightOvertimeHours += totalRegularNightOvertimeHours;
                        newPayrollLine.TotalLegalHolidayNightOvertimeHours += totalLegalHolidayNightOvertimeHours;
                        newPayrollLine.TotalSpecialHolidayNighOvertimetHours += totalSpecialHolidayNighOvertimetHours;
                        newPayrollLine.TotalTardyLateHours += totalTardyLateHours;
                        newPayrollLine.TotalTardyUndertimeHours += totalTardyUndertimeHours;
                        newPayrollLine.TotalRegularWorkingAmount += (totalSalaryAmount -
                            totalRegularRestdayAmount -
                            totalLegalHolidayWorkingAmount -
                            totalSpecialHolidayWorkingAmount -
                            totalLegalHolidayRestdayAmount -
                            totalSpecialHolidayRestdayAmount -
                            totalRegularNightAmount -
                            totalRegularOvertimeAmount -
                            totalLegalHolidayNightAmount -
                            totalLegalHolidayNightAmountDeduction -
                            totalSpecialHolidayNightAmount -
                            totalSpecialHolidayNightAmountDeduction -
                            totalLegalHolidayOvertimeAmount -
                            totalLegalHolidayOvertimeAmountDeduction -
                            totalSpecialHolidayOvertimeAmount -
                            totalSpecialHolidayOvertimeAmountDeduction -
                            totalLegalHolidayNightOvertimeAmountDeduction -
                            totalSpecialHolidayNightOvertimeAmountDeduction); //totalRegularWorkingAmount;
                        newPayrollLine.TotalLegalHolidayWorkingAmount += totalLegalHolidayWorkingAmount;
                        newPayrollLine.TotalSpecialHolidayWorkingAmount += totalSpecialHolidayWorkingAmount;
                        newPayrollLine.TotalRegularRestdayAmount += totalRegularRestdayAmount;
                        newPayrollLine.TotalLegalHolidayRestdayAmount += totalLegalHolidayRestdayAmount;
                        newPayrollLine.TotalSpecialHolidayRestdayAmount += totalSpecialHolidayRestdayAmount;
                        newPayrollLine.TotalRegularOvertimeAmount += totalRegularOvertimeAmount;
                        newPayrollLine.TotalLegalHolidayOvertimeAmount += totalLegalHolidayOvertimeAmount;
                        newPayrollLine.TotalSpecialHolidayOvertimeAmount += totalSpecialHolidayOvertimeAmount;
                        newPayrollLine.TotalRegularNightAmount += totalRegularNightAmount;
                        newPayrollLine.TotalLegalHolidayNightAmount += totalLegalHolidayNightAmount;
                        newPayrollLine.TotalSpecialHolidayNightAmount += totalSpecialHolidayNightAmount;
                        newPayrollLine.TotalRegularNightOvertimeAmount += totalRegularNightOvertimeAmount;
                        newPayrollLine.TotalLegalHolidayNightOvertimeAmount += totalLegalHolidayNightOvertimeAmount;
                        newPayrollLine.TotalSpecialHolidayNightOvertimeAmount += totalSpecialHolidayNightOvertimeAmount;
                        newPayrollLine.TotalSalaryAmount += totalSalaryAmount;
                        newPayrollLine.TotalTardyAmount += totalTardyAmount;
                        newPayrollLine.TotalAbsentAmount += totalAbsentAmount;
                        newPayrollLine.TotalNetSalaryAmount += totalNetSalaryAmount - totalSpecialHolidayNightAmountDeduction;

                        if (dtrPerEmpCounter == dtrPerEmpCount)
                        {
                            if (line.Key.PayrollTypeId != 1)
                            {
                                var dblPayrollRate = 0m;
                                var dblDailyRate = 0m;
                                var totalNetSalaryAmount2 = 0m;

                                var dblTotalLegalHolidayWorkingAmount = 0m;
                                var dblTotalSpecialHolidayWorkingAmount = 0m;
                                var dblTotalRegularRestdayAmount = 0m;
                                var dblTotalLegalHolidayRestdayAmount = 0m;
                                var dblTotalSpecialHolidayRestdayAmount = 0m;
                                var dblTotalRegularOvertimeAmount = 0m;
                                var dblTotalLegalHolidayOvertimeAmount = 0m;
                                var dblTotalSpecialHolidayOvertimeAmount = 0m;
                                var dblTotalRegularNightAmount = 0m;
                                var dblTotalLegalHolidayNightAmount = 0m;
                                var dblTotalSpecialHolidayNightAmount = 0m;
                                var dblTotalRegularNightOvertimeAmount = 0m;
                                var dblTotalLegalHolidayNightOvertimeAmount = 0m;
                                var dblTotalSpecialHolidayNightOvertimeAmount = 0m;

                                dblDailyRate = line.Key.DailyRate;

                                dblTotalLegalHolidayWorkingAmount = newPayrollLine.TotalLegalHolidayWorkingAmount - dblDailyRate; ;
                                if (dblTotalLegalHolidayWorkingAmount < 0) dblTotalLegalHolidayWorkingAmount = 0;


                                dblTotalSpecialHolidayWorkingAmount = newPayrollLine.TotalSpecialHolidayWorkingAmount - dblDailyRate;
                                if (dblTotalSpecialHolidayWorkingAmount < 0) dblTotalSpecialHolidayWorkingAmount = 0;


                                dblTotalRegularRestdayAmount = newPayrollLine.TotalRegularRestdayAmount - dblDailyRate;
                                if (dblTotalRegularRestdayAmount < 0) dblTotalRegularRestdayAmount = 0;


                                dblTotalLegalHolidayRestdayAmount = newPayrollLine.TotalLegalHolidayRestdayAmount - dblDailyRate;
                                if (dblTotalLegalHolidayRestdayAmount < 0) dblTotalLegalHolidayRestdayAmount = 0;


                                dblTotalSpecialHolidayRestdayAmount = newPayrollLine.TotalSpecialHolidayRestdayAmount - dblDailyRate;
                                if (dblTotalSpecialHolidayRestdayAmount < 0) dblTotalSpecialHolidayRestdayAmount = 0;


                                dblTotalRegularOvertimeAmount = newPayrollLine.TotalRegularOvertimeAmount - dblDailyRate;
                                if (dblTotalRegularOvertimeAmount < 0) dblTotalRegularOvertimeAmount = 0;


                                dblTotalLegalHolidayOvertimeAmount = newPayrollLine.TotalLegalHolidayOvertimeAmount - dblDailyRate;
                                if (dblTotalLegalHolidayOvertimeAmount < 0) dblTotalLegalHolidayOvertimeAmount = 0;


                                dblTotalSpecialHolidayOvertimeAmount = newPayrollLine.TotalSpecialHolidayOvertimeAmount - dblDailyRate;
                                if (dblTotalSpecialHolidayOvertimeAmount < 0) dblTotalSpecialHolidayOvertimeAmount = 0;


                                dblTotalRegularNightAmount = newPayrollLine.TotalRegularNightAmount - dblDailyRate;
                                if (dblTotalRegularNightAmount < 0) dblTotalRegularNightAmount = 0;


                                dblTotalLegalHolidayNightAmount = newPayrollLine.TotalLegalHolidayNightAmount - dblDailyRate;
                                if (dblTotalLegalHolidayNightAmount < 0) dblTotalLegalHolidayNightAmount = 0;


                                dblTotalSpecialHolidayNightAmount = newPayrollLine.TotalSpecialHolidayNightAmount - dblDailyRate;
                                if (dblTotalSpecialHolidayNightAmount < 0) dblTotalSpecialHolidayNightAmount = 0;


                                dblTotalRegularNightOvertimeAmount = newPayrollLine.TotalRegularNightOvertimeAmount - dblDailyRate;
                                if (dblTotalRegularNightOvertimeAmount < 0) dblTotalRegularNightOvertimeAmount = 0;


                                dblTotalLegalHolidayNightOvertimeAmount = newPayrollLine.TotalLegalHolidayNightOvertimeAmount - dblDailyRate;
                                if (dblTotalLegalHolidayNightOvertimeAmount < 0) dblTotalLegalHolidayNightOvertimeAmount = 0;


                                dblTotalSpecialHolidayNightOvertimeAmount = newPayrollLine.TotalSpecialHolidayNightOvertimeAmount - dblDailyRate;
                                if (dblTotalSpecialHolidayNightOvertimeAmount < 0) dblTotalSpecialHolidayNightOvertimeAmount = 0;

                                dblPayrollRate = line.Key.PayrollRate +
                                                 newPayrollLine.TotalLegalHolidayWorkingAmount +
                                                 newPayrollLine.TotalSpecialHolidayWorkingAmount +
                                                 newPayrollLine.TotalRegularRestdayAmount +
                                                 newPayrollLine.TotalLegalHolidayRestdayAmount +
                                                 newPayrollLine.TotalSpecialHolidayRestdayAmount +
                                                 newPayrollLine.TotalRegularOvertimeAmount +
                                                 newPayrollLine.TotalLegalHolidayOvertimeAmount +
                                                 newPayrollLine.TotalSpecialHolidayOvertimeAmount +
                                                 newPayrollLine.TotalRegularNightAmount +
                                                 newPayrollLine.TotalLegalHolidayNightAmount +
                                                 newPayrollLine.TotalSpecialHolidayNightAmount +
                                                 newPayrollLine.TotalRegularNightOvertimeAmount +
                                                 newPayrollLine.TotalLegalHolidayNightOvertimeAmount +
                                                 newPayrollLine.TotalSpecialHolidayNightOvertimeAmount;

                                totalNetSalaryAmount2 = dblPayrollRate - (newPayrollLine.TotalTardyAmount + newPayrollLine.TotalAbsentAmount);

                                //if (line.Key.PayrollTypeId == 2)
                                //{
                                //    totalNetSalaryAmount2 = dblPayrollRate;
                                //}

                                newPayrollLine.TotalSalaryAmount = dblPayrollRate;
                                newPayrollLine.TotalNetSalaryAmount = totalNetSalaryAmount2;

                                if (line.Key.PayrollTypeId == 2) 
                                {
                                    newPayrollLine.TotalSalaryAmount = line.Key.PayrollRate;
                                    newPayrollLine.TotalAbsentAmount = 0;
                                    newPayrollLine.TotalNetSalaryAmount = line.Key.PayrollRate;
                                }
                            }

                            await ctx.TrnPayrollLines.AddAsync(newPayrollLine);

                            dtrPerEmpCounter = 0;
                        }
                    }

                    await ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("hourly rate")) 
                {
                    throw new Exception(ex.Message);
                }

                Console.WriteLine("Message: " + ex.Message);
            }           
        }

        internal static async Task ProcessPayrollOtherIncome(EditPayrollLinesByOtherIncome command)
        {
            using (var ctx = new HRISContext())
            {
                var payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId);

                if (command.EmployeeId is not null)
                {
                    payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId && x.EmployeeId == command.EmployeeId);
                }

                var query = new Queries.TrnPayroll.PayrollDetailOtherIncomeList();

                query.PayrollOtherIncomeId = command.PayrollOtherIncomeId;

                foreach (var line in payrollLines)
                {
                    query.IsTaxable = true;
                    var totalAmount = query.Result()
                       .Where(x => x.EmployeeId == line.EmployeeId)
                       ?.Sum(x => x.TotalAmount) ?? 0m;

                    query.IsTaxable = false;
                    var totalAmountNonTax = query.Result()
                       .Where(x => x.EmployeeId == line.EmployeeId)
                       ?.Sum(x => x.TotalAmount) ?? 0m;

                    line.TotalOtherIncomeTaxable = totalAmount;
                    line.TotalOtherIncomeNonTaxable = totalAmountNonTax;
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task ProcessPayrollOtherDeduction(EditPayrollLinesByOtherDeduction command)
        {
            using (var ctx = new HRISContext())
            {
                var payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId);

                if (command.EmployeeId is not null)
                {
                    payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId && x.EmployeeId == command.EmployeeId);
                }

                foreach (var line in await payrollLines.ToListAsync())
                {
                    var totalAmount = ctx.TrnPayrollOtherDeductionLines
                        .Where(x => x.EmployeeId == line.EmployeeId &&
                            x.PayrollOtherDeductionId == command.PayrollOtherDeductionId)
                        ?.Sum(x => x.Amount) ?? 0;

                    line.TotalOtherDeduction = totalAmount;
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task ProcessSSS(EditPayrollLinesByMandatory command) 
        {
            using (var ctx = new HRISContext()) 
            {
                var payrollGroupId = ctx.TrnPayrolls
                    .Where(x => x.Id == command.PayrollId)
                    .Select(x => x.PayrollGroupId)
                    .FirstOrDefault();

                mandantoryDeductionDivisor = 2;

                if (payrollGroupId == 52) 
                {
                    mandantoryDeductionDivisor = 4;
                }

                if (payrollGroupId == 55) 
                {
                    mandantoryDeductionDivisor = 1;
                }

                if (command.IsProcessInMonth) 
                {
                    mandantoryDeductionDivisor = 1;
                }

                var payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId);

                if (command.EmployeeId is not null) 
                {
                    payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId && x.EmployeeId == command.EmployeeId);
                }

                foreach (var line in payrollLines) 
                {
                    line.Ssscontribution = ComputeSSSContribution(line);
                    line.Ssseccontribution = ComputeSSSECContribution(line);
                    line.SsscontributionEmployer = ComputeSSSContributionEmployer(line);
                    line.SsseccontributionEmployer = ComputeSSSECContributionEmployer(line);
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task ProcessPHIC(EditPayrollLinesByMandatory command)
        {
            using (var ctx = new HRISContext())
            {
                var payrollGroupId = ctx.TrnPayrolls
                   .Where(x => x.Id == command.PayrollId)
                   .Select(x => x.PayrollGroupId)
                   .FirstOrDefault();

                mandantoryDeductionDivisor = 2;

                if (payrollGroupId == 52)
                {
                    mandantoryDeductionDivisor = 4;
                }

                if (payrollGroupId == 55)
                {
                    mandantoryDeductionDivisor = 1;
                }

                if (command.IsProcessInMonth)
                {
                    mandantoryDeductionDivisor = 1;
                }

                var payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId);

                if (command.EmployeeId is not null)
                {
                    payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId && x.EmployeeId == command.EmployeeId);
                }

                foreach (var line in payrollLines)
                {
                    line.Phiccontribution = ComputePHICContribution(line);
                    line.PhiccontributionEmployer = ComputePHICContributionEmployer(line);
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task ProcessHDMF(EditPayrollLinesByMandatory command)
        {
            using (var ctx = new HRISContext())
            {
                var payrollGroupId = ctx.TrnPayrolls
                   .Where(x => x.Id == command.PayrollId)
                   .Select(x => x.PayrollGroupId)
                   .FirstOrDefault();

                mandantoryDeductionDivisor = 2;

                if (payrollGroupId == 52)
                {
                    mandantoryDeductionDivisor = 4;
                }

                if (payrollGroupId == 55)
                {
                    mandantoryDeductionDivisor = 1;
                }

                if (command.IsProcessInMonth)
                {
                    mandantoryDeductionDivisor = 1;
                }

                var payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId);

                if (command.EmployeeId is not null)
                {
                    payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId && x.EmployeeId == command.EmployeeId);
                }

                foreach (var line in payrollLines)
                {
                    line.Hdmfcontribution = ComputeHDMFContribution(line);
                    line.HdmfcontributionEmployer = ComputeHDMFContributionEmployer(line);
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task ProcessWithholdingTax(EditPayrollLinesByWithholding command) 
        {
            using (var ctx = new HRISContext())
            {
                var payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId);

                if (command.EmployeeId is not null)
                {
                    payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId && x.EmployeeId == command.EmployeeId);
                }

                foreach (var line in payrollLines)
                {
                    line.GrossIncome = line.TotalNetSalaryAmount + line.TotalOtherIncomeTaxable;
                    line.Tax = ComputeTax(line);
                }

                await ctx.SaveChangesAsync();
            }
        }

        internal static async Task ProcessTotals(EditPayrollLinesByTotals command)
        {
            using (var ctx = new HRISContext())
            {
                var payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId);

                if (command.EmployeeId is not null)
                {
                    payrollLines = ctx.TrnPayrollLines.Where(x => x.PayrollId == command.PayrollId && x.EmployeeId == command.EmployeeId);
                }

                foreach (var line in payrollLines)
                {
                    var grossIncome = line.TotalNetSalaryAmount + line.TotalOtherIncomeTaxable;
                    var grossIncomeWithNonTaxable = grossIncome + line.TotalOtherIncomeNonTaxable;
                    var mandatoryDeduction = line.Ssscontribution + line.Phiccontribution + line.Hdmfcontribution;

                    line.GrossIncome = grossIncome;
                    line.GrossIncomeWithNonTaxable = grossIncomeWithNonTaxable;
                    line.NetIncome = grossIncomeWithNonTaxable - mandatoryDeduction - line.Tax - line.TotalOtherDeduction;
                }

                await ctx.SaveChangesAsync();
            }
        }
        #endregion
    }
}
