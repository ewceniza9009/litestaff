using Microsoft.AspNetCore.Mvc;
using whris.Application.Common;
using whris.Application.Mobile;
using whris.Application.Mobile.RepPayroll;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.UI.Controllers
{
    public class MobileRepPayrollController : Controller
    {
        private readonly HRISContext _context;
        private readonly ILogger<MobileRepPayrollController> _logger;

        public MobileRepPayrollController(HRISContext context, ILogger<MobileRepPayrollController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // --- All GET methods are now async to handle traffic ---

        public async Task<JsonResult> IsLoginValid(string Code)
        {
            var login = new Login() { MobileCode = Code };
            return Json(await login.ResultAsync());
        }

        public async Task<JsonResult> GetPayrollNumbers(string Code)
        {
            return Json(await Common.GetPayrollNumbers2Async(Code));
        }

        public async Task<JsonResult> GetPaySlip(int PayrollId, string Code)
        {
            var payslip = new Payslip()
            {
                PayrollId = PayrollId,
                MobileCode = Code
            };
            return Json(await payslip.ResultAsync());
        }

        public async Task<JsonResult> GetDTRs(string Code)
        {
            return Json(await Common.GetDTRs2Async(Code));
        }

        public async Task<JsonResult> GetDTRSlip(int DTRId, string Code)
        {
            var dtrslip = new DTRSlip()
            {
                DTRId = DTRId,
                MobileCode = Code
            };
            return Json(await dtrslip.ResultAsync());
        }


        [HttpPost]
        public async Task<IActionResult> LogBiometricData([FromBody] LogBiometricDataRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.BiometricIdNumber) || request.LogDateTime == null || string.IsNullOrEmpty(request.LogType))
            {
                return BadRequest("Invalid input data.");
            }

            // YOUR LOGIC UNCHANGED: This takes the clock time as is.
            DateTime clientLocalTime = request.LogDateTime?.DateTime ?? DateTime.Now;

            var logEntry = new TrnLog
            {
                BiometricIdNumber = request.BiometricIdNumber,
                LogDateTime = clientLocalTime,
                LogType = request.LogType
            };

            _context.TrnLogs.Add(logEntry);

            try
            {
                // Use async save for better performance
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Log entry saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving single biometric log.");
                _logger.LogError(ex.InnerException?.Message, "Error inner exception single biometric log");
                _logger.LogError(ex.StackTrace, "Error stack trace single biometric log");
                return StatusCode(500, "An error occurred while saving the log entry.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertOfflineLogs([FromBody] List<LogBiometricDataRequestDto> request)
        {
            if (request == null || !request.Any())
            {
                return BadRequest("Invalid input data.");
            }

            // YOUR LOGIC UNCHANGED: This projects all logs using your exact time rule.
            var logEntries = request.Select(item => new TrnLog
            {
                BiometricIdNumber = item.BiometricIdNumber,
                LogDateTime = item.LogDateTime?.DateTime ?? DateTime.Now,
                LogType = item.LogType
            });

            // 1. PERFORMANCE UPGRADE: Add all logs to the context in memory first.
            _context.TrnLogs.AddRange(logEntries);

            try
            {
                // 2. PERFORMANCE UPGRADE: Save all logs in a single database transaction.
                await _context.SaveChangesAsync();
                return Ok(new { Message = "All log entries saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving offline biometric logs.");
                return StatusCode(500, "An error occurred while saving log entries.");
            }
        }
    }

    public class LogBiometricDataRequestDto
    {
        public string? BiometricIdNumber { get; set; }
        public DateTimeOffset? LogDateTime { get; set; }
        public string? LogType { get; set; }
    }
}