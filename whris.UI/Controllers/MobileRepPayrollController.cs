using Microsoft.AspNetCore.Mvc;
using System.Linq;
using whris.Application.Common;
using whris.Application.Library;
using whris.Application.Mobile;
using whris.Application.Mobile.RepPayroll;
using whris.Data.Data;
using whris.Data.Models;

namespace whris.UI.Controllers
{
    public class MobileRepPayrollController : Controller
    {

        private readonly HRISContext _context;

        public MobileRepPayrollController(HRISContext context)
        {
            _context = context;
        }

        public JsonResult IsLoginValid(string Code) 
        {
            var login = new Login() { MobileCode = Code };

            return Json(login.Result());
        }

        public JsonResult GetPayrollNumbers(string Code) 
        {
            return Json(Common.GetPayrollNumbers2(Code));
        }

        public JsonResult GetPaySlip(int PayrollId, string Code)
        {
            var payslip = new Payslip()
            {
                PayrollId = PayrollId,
                MobileCode = Code
            };

            var result = payslip.Result();

            return Json(result);
        }

        public JsonResult GetDTRs(string Code)
        {
            return Json(Common.GetDTRs2(Code));
        }

        public JsonResult GetDTRSlip(int DTRId, string Code)
        {
            var dtrslip = new DTRSlip()
            {
                DTRId = DTRId,
                MobileCode = Code
            };

            var result = dtrslip.Result();

            return Json(result);
        }

        [HttpPost]
        public IActionResult LogBiometricData([FromBody] LogBiometricDataRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.BiometricIdNumber) || request.LogDateTime == null || string.IsNullOrEmpty(request.LogType))
            {
                return BadRequest("Invalid input data.");
            }

            // Convert the DateTimeOffset to a DateTime.
            // This will take the clock time as is, losing the offset info.
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
                _context.SaveChanges();
                return Ok(new { Message = "Log entry saved successfully." });
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while saving the log entry: {ex.Message}";

                if (ex.InnerException != null)
                {
                    errorMessage += $"\nInner exception: {ex.InnerException.Message}";
                }

                // Optional: include stack trace for debugging (not recommended in production)
                errorMessage += $"\nStack trace: {ex.StackTrace}";

                return StatusCode(500, errorMessage);
            }
        }

        [HttpPost]
        public IActionResult InsertOfflineLogs([FromBody] List<LogBiometricDataRequestDto> request)
        {
            if (request == null || (request?.Count ?? 0) == 0)
            {
                return BadRequest("Invalid input data.");
            }

            foreach (var item in request) 
            {
                // Convert the DateTimeOffset to a DateTime.
                // This will take the clock time as is, losing the offset info.
                DateTime clientLocalTime = item.LogDateTime?.DateTime ?? DateTime.Now;

                var logEntry = new TrnLog
                {
                    BiometricIdNumber = item.BiometricIdNumber,
                    LogDateTime = clientLocalTime,
                    LogType = item.LogType                 
                };

                _context.TrnLogs.Add(logEntry);
            }            

            try
            {
                _context.SaveChanges();
                return Ok(new { Message = "Log entry saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving the log entry: {ex.Message}");
            }
        }
    }

    public class LogBiometricDataRequestDto
    {
        public string? BiometricIdNumber { get; set; }
        public DateTimeOffset? LogDateTime { get; set; }  // Captures the offset information from the client.
        public string? LogType { get; set; }
    }
}
