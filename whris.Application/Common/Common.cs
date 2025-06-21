using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using whris.Application.Dtos;
using whris.Application.Library;
using whris.Application.Queries.TrnPayrollOtherDeduction;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.Application.Common
{
    public class Common
    {
        static HRISContext HRISContext 
        {
            get => new HRISContext();
        }

        public static JsonResult GetDepartmentList()
        {
            return new JsonResult(HRISContext.MstDepartments
                .OrderBy(x => x.Department)
                .Select(x => new MstDepartmentDto()
                {
                    Id = x.Id,
                    Department = x.Department
                })
                .ToList());
        }

        public static JsonResult GetZipCodes()
        {
            return new JsonResult(HRISContext.MstZipCodes
                .OrderBy(x => x.ZipCode)
                .Select(x => new MstZipCode()
                {
                    Id = x.Id,
                    ZipCode = x.ZipCode
                })
                .ToList());
        }

        public static JsonResult GetCitizenships()
        {
            return new JsonResult(HRISContext.MstCitizenships
                .OrderBy(x => x.Citizenship)
                .Select(x => new MstCitizenship()
                {
                    Id = x.Id,
                    Citizenship = x.Citizenship
                })
                .ToList());
        }

        public static JsonResult GetReligions()
        {
            return new JsonResult(HRISContext.MstReligions
                .OrderBy(x => x.Religion)
                .Select(x => new MstReligion()
                {
                    Id = x.Id,
                    Religion = x.Religion
                })
                .ToList());
        }

        public static JsonResult GetTaxCodes()
        {
            return new JsonResult(HRISContext.MstTaxCodes
                .OrderBy(x => x.TaxCode)
                .Select(x => new MstTaxCode()
                {
                    Id = x.Id,
                    TaxCode = x.TaxCode
                })
                .ToList());
        }

        public static JsonResult GetCompanies()
        {
            return new JsonResult(HRISContext.MstCompanies
                .OrderBy(x => x.Company)
                .Select(x => new MstCompany()
                {
                    Id = x.Id,
                    Company = x.Company
                })
                .ToList());
        }

        public static JsonResult GetBranches()
        {
            return new JsonResult(HRISContext.MstBranches
                .OrderBy(x => x.Branch)
                .Select(x => new MstBranch()
                {
                    Id = x.Id,
                    Branch = x.Branch
                })
                .ToList());
        }

        public static JsonResult GetDepartments()
        {
            return new JsonResult(HRISContext.MstDepartments
                .OrderBy(x => x.Department)
                .Select(x => new MstDepartment()
                {
                    Id = x.Id,
                    Department = x.Department
                })
                .ToList());
        }

        public static JsonResult GetPositions()
        {
            return new JsonResult(HRISContext.MstPositions
                .OrderBy(x => x.Position)
                .Select(x => new MstPosition()
                {
                    Id = x.Id,
                    Position = x.Position
                })
                .ToList());
        }

        public static JsonResult GetDivisions()
        {
            return new JsonResult(HRISContext.MstDivisions
                .OrderBy(x => x.Division)
                .Select(x => new MstDivision()
                {
                    Id = x.Id,
                    Division = x.Division
                })
                .ToList());
        }

        public static JsonResult GetPayrollGroups()
        {
            return new JsonResult(HRISContext.MstPayrollGroups
                .OrderBy(x => x.PayrollGroup)
                .Select(x => new MstPayrollGroup()
                {
                    Id = x.Id,
                    PayrollGroup = x.PayrollGroup
                })
                .ToList());
        }

        public static JsonResult GetPayrollTypes()
        {
            return new JsonResult(HRISContext.MstPayrollTypes
                .OrderBy(x => x.PayrollType)
                .Select(x => new MstPayrollType()
                {
                    Id = x.Id,
                    PayrollType = x.PayrollType
                })
                .ToList());
        }

        public static JsonResult GetGLAccounts()
        {
            return new JsonResult(HRISContext.MstAccounts
                .OrderBy(x => x.Account)
                .Select(x => new MstAccount()
                {
                    Id = x.Id,
                    Account = x.Account
                })
                .ToList());
        }

        public static JsonResult GetShiftCodes()
        {
            return new JsonResult(HRISContext.MstShiftCodes
                .Where(x => x.IsLocked)
                .OrderBy(x => x.ShiftCode)
                .Select(x => new MstShiftCode()
                {
                    Id = x.Id,
                    ShiftCode = x.ShiftCode
                })
                .ToList());
        }

        public static JsonResult GetDayTypes()
        {
            return new JsonResult(HRISContext.MstDayTypes
                .Where(x => x.IsLocked)
                .OrderBy(x => x.DayType)
                .Select(x => new MstDayType()
                {
                    Id = x.Id,
                    DayType = x.DayType
                })
                .ToList());
        }

        public static JsonResult GetUsers()
        {
            return new JsonResult(HRISContext.MstUsers
                .Where(x => x.IsLocked)
                .OrderBy(x => x.FullName)
                .Select(x => new MstUser()
                {
                    Id = x.Id,
                    FullName = x.FullName
                })
                .ToList());
        }

        public static JsonResult GetAspUsers() 
        {
            return new JsonResult(HRISContext.AspNetUsers
                .OrderBy(x => x.Email)
                .Select(x => new AspNetUser()
                {
                    Id = x.Id,
                    Email = x.Email
                })
                .ToList());
        }

        public static JsonResult GetForms()
        {
            return new JsonResult(HRISContext.SysForms
                .OrderBy(x => x.Remarks)
                .Select(x => new SysForm()
                {
                    Id = x.Id,
                    Remarks = x.Remarks
                })
                .ToList());
        }

        public static JsonResult GetPeriods() 
        {
            return new JsonResult(HRISContext.MstPeriods
                .OrderByDescending(x => x.Period)
                .Select(x => new MstPeriod()
                {
                    Id = x.Id,
                    Period = x.Period
                })
                .ToList());
        }

        public static JsonResult GetChangeShifts()
        {
            return new JsonResult(HRISContext.TrnChangeShifts
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Csnumber)
                .Select(x => new TrnChangeShift()
                {
                    Id = x.Id,
                    Csnumber = $"{x.Csnumber} - {x.Remarks}"
                })
                .ToList());
        }

        public static JsonResult GetOvertimes() 
        {
            return new JsonResult(HRISContext.TrnOverTimes
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Otnumber)
                .Select(x => new TrnOverTime()
                {
                    Id = x.Id,
                    Otnumber = x.Otnumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList()); ;
        }

        public static JsonResult GetLeaves()
        {
            return new JsonResult(HRISContext.TrnLeaveApplications
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Lanumber)
                .Select(x => new TrnLeaveApplication()
                {
                    Id = x.Id,
                    Lanumber = x.Lanumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList());
        }

        public static JsonResult GetEmployees(int? payrollGroupId = null)
        {
            if (payrollGroupId is not null) 
            {
                return new JsonResult(HRISContext.MstEmployees
                   .OrderBy(x => x.FullName)
                   .Where(x => x.PayrollGroupId == payrollGroupId && x.IsLocked)
                   .Select(x => new MstEmployeeDto()
                   {
                       Id = x.Id,
                       FullName = x.FullName,
                       OvertimeHourlyRate = x.OvertimeHourlyRate,
                       DepartmentId = x.DepartmentId,
                       PayrollGroupId = x.PayrollGroupId
                   })
                   .ToList());
            }

            return new JsonResult(HRISContext.MstEmployees
                .OrderBy(x => x.FullName)
                .Where(x => x.IsLocked)
                .Select(x => new MstEmployeeDto()
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    OvertimeHourlyRate = x.OvertimeHourlyRate,
                    DepartmentId = x.DepartmentId,
                    PayrollGroupId = x.PayrollGroupId                
                })
                .ToList());
        }

        public static JsonResult GetAllEmployees(int? payrollGroupId = null)
        {
            if (payrollGroupId is not null)
            {
                return new JsonResult(HRISContext.MstEmployees
                   .OrderBy(x => x.FullName)
                   .Where(x => x.PayrollGroupId == payrollGroupId)
                   .Select(x => new MstEmployeeDto()
                   {
                       Id = x.Id,
                       FullName = x.FullName,
                       OvertimeHourlyRate = x.OvertimeHourlyRate,
                       DepartmentId = x.DepartmentId,
                       PayrollGroupId = x.PayrollGroupId
                   })
                   .ToList());
            }

            return new JsonResult(HRISContext.MstEmployees
                .OrderBy(x => x.FullName)
                .Select(x => new MstEmployeeDto()
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    OvertimeHourlyRate = x.OvertimeHourlyRate,
                    DepartmentId = x.DepartmentId,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList());
        }

        public static JsonResult GetSetupLeaves() 
        {
            return new JsonResult(HRISContext.MstLeaves
                .OrderBy(x => x.Leave)
                .Select(x => new MstLeave()
                {
                    Id = x.Id,
                    Leave = x.Leave
                })
                .ToList());
        }

        public static JsonResult GetDeductions()
        {
            return new JsonResult(HRISContext.MstOtherDeductions
                .OrderBy(x => x.OtherDeduction)
                .Select(x => new MstOtherDeduction()
                {
                    Id = x.Id,
                    OtherDeduction = x.OtherDeduction
                })
                .ToList());
        }

        public static JsonResult GetLoans(int? employeeId = null)
        {
            var loans = new EmployeeLoanList()
            {
                EmployeeId = employeeId
            };

            return new JsonResult(loans.Result());
        }

        public static JsonResult GetIncomes()
        {
            return new JsonResult(HRISContext.MstOtherIncomes
                .OrderBy(x => x.OtherIncome)
                .Select(x => new MstOtherIncome()
                {
                    Id = x.Id,
                    OtherIncome = x.OtherIncome
                })
                .ToList());
        }

        public static JsonResult GetDeductionLoans()
        {
            return new JsonResult(HRISContext.MstOtherDeductions
                .OrderBy(x => x.OtherDeduction)
                .Where(x => x.LoanType)
                .Select(x => new MstOtherDeduction()
                {
                    Id = x.Id,
                    OtherDeduction = x.OtherDeduction
                })
                .ToList());
        }

        public static JsonResult GetMonths()
        {
            return new JsonResult(HRISContext.MstMonths
                .Select(x => new MstMonth()
                {
                    Id = x.Id,
                    Month = x.Month
                })
                .ToList());
        }

        public static JsonResult GetDTRs()
        {
            return new JsonResult(HRISContext.TrnDtrs
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Dtrnumber)
                .Select(x => new TrnDtr()
                {
                    Id = x.Id,
                    Dtrnumber = x.Dtrnumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList());
        }

        public static JsonResult GetPayrollOtherIncomes()
        {
            return new JsonResult(HRISContext.TrnPayrollOtherIncomes
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Poinumber)
                .Select(x => new TrnPayrollOtherIncome()
                {
                    Id = x.Id,
                    Poinumber = x.Poinumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList());
        }

        public static JsonResult GetPayrollOtherDeductions()
        {
            return new JsonResult(HRISContext.TrnPayrollOtherDeductions
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Podnumber)
                .Select(x => new TrnPayrollOtherDeduction()
                {
                    Id = x.Id,
                    Podnumber = x.Podnumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList());
        }

        public static JsonResult GetLastWithholdingTaxes()
        {
            return new JsonResult(HRISContext.TrnLastWithholdingTaxes
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.Lwtnumber)
                .Select(x => new TrnLastWithholdingTax()
                {
                    Id = x.Id,
                    Lwtnumber = x.Lwtnumber + " - " + x.Remarks,
                    PayrollGroupId = x.PayrollGroupId
                })
                .ToList());
        }

        public static JsonResult GetPayrollNumbers(int? payrollGroupId = null)
        {
            if (payrollGroupId is not null) 
            {
                return new JsonResult(HRISContext.TrnPayrolls
                    .Where(x => x.IsLocked && x.PayrollGroupId == payrollGroupId)
                    .OrderByDescending(x => x.PayrollNumber)
                    .Select(x => new TrnPayrollDto()
                    {
                        Id = x.Id,
                        PayrollNumber = x.PayrollNumber + " - " + x.Remarks
                    })
                    .ToList());
            }

            return new JsonResult(HRISContext.TrnPayrolls
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.PayrollNumber)
                .Select(x => new TrnPayrollDto()
                {
                    Id = x.Id,
                    PayrollNumber = x.PayrollNumber + " - " + x.Remarks
                })
                .ToList());
        }

        public static JsonResult GetPayrollNumbersWithRemarks()
        {
            return new JsonResult(HRISContext.TrnPayrolls
                .Where(x => x.IsLocked)
                .OrderByDescending(x => x.PayrollNumber)
                .Select(x => new TrnPayrollDto()
                {
                    Id = x.Id,
                    Remarks = x.PayrollNumber + " - " + x.Remarks
                })
                .ToList());
        }

        public static List<TrnPayrollDto> GetPayrollNumbers2(string Code)
        {
            var payrollGroupId = MobileUtils.GetPayrollGroupId(Code);

            return HRISContext.TrnPayrolls
                .Where(x => x.IsLocked && x.IsApproved && x.PayrollGroupId == payrollGroupId)
                .OrderByDescending(x => x.PayrollNumber)
                .Select(x => new TrnPayrollDto()
                {
                    Id = x.Id,
                    PayrollNumber = x.Remarks ?? "NA"
                })
                .ToList();
        }

        public static JsonResult GetDTRs2(string Code)
        {
            var payrollGroupId = MobileUtils.GetPayrollGroupId(Code);

            return new JsonResult(HRISContext.TrnDtrs
                .Where(x => x.IsLocked && x.IsApproved && x.PayrollGroupId == payrollGroupId)
                .OrderByDescending(x => x.Dtrnumber)
                .Select(x => new TrnDtr()
                {
                    Id = x.Id,
                    Dtrnumber = x.Dtrnumber + " - " + x.Remarks
                })
                .ToList());
        }

        public static List<MstEmployeeDto> GetEmployeesWithSalary() 
        {
            return HRISContext.MstEmployees
                .OrderBy(x => x.FullName)
                .Select(x => new MstEmployeeDto()
                {
                    Id = x.Id,
                    BiometricIdNumber = x.BiometricIdNumber,
                    FullName = x.FullName,
                    CellphoneNumber = x.CellphoneNumber,
                    EmailAddress = x.EmailAddress,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.DepartmentName,
                    IsLocked = x.IsLocked,
                    Gsisnumber = x.Gsisnumber,
                    Sssnumber = x.Sssnumber,
                    Hdmfnumber = x.Hdmfnumber,
                    Phicnumber = x.Phicnumber,
                    Tin = x.Tin,
                    AtmaccountNumber = x.AtmaccountNumber,
                    Company = Lookup.GetCompanyNameById(x.CompanyId),
                    Branch = Lookup.GetBranchNameById(x.BranchId),
                    Department = Lookup.GetDepartmentNameById(x.DepartmentId),
                    Position = Lookup.GetPositionNameById(x.PositionId),
                    MonthlyRate = x.MonthlyRate,
                    DailyRate = x.DailyRate,
                    Allowance = x.Allowance,
                })
                .ToList();
        }
    }
}
