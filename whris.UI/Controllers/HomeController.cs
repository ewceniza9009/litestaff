using DevExpress.CodeParser;
using DevExpress.PivotGrid.OLAP.AdoWrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Security.Claims;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Application.Queries.Home;
using whris.Application.Queries.TrnDtr;
using whris.Data.Data;
using whris.UI.Authorization;

namespace whris.UI.Controllers
{
    [Authorize]
    [Secure2]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DisplayUser()
        {
            var result = "";

            using (var context = new HRISContext())
            {
                result = context.MstUsers
                    .FirstOrDefault(x => x.ASPUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    ?.FullName;
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult EarlyBirds()
        {
            var topEarlyBirds = new List<TopEarlyBird>();
            var employeeTimeIns = new Chart();

            var lines = employeeTimeIns.Result()
                .Where(x => x.DtrTimeIn < x.ShiftTimeIn)
                .GroupBy(x => x.EmployeeId )
                .ToList();

            var ctr = 0;

            foreach (var line in lines) 
            {
                ctr++;

                if (ctr > 5) 
                {
                    break;
                }

                topEarlyBirds.Add(new TopEarlyBird
                {
                    EmployeeName = Lookup.GetEmployeeNameById(line.Key),
                    EarlyWorkCount = line.Count()
                });
            }

            return Json(topEarlyBirds);
        }

        [HttpPost]
        public ActionResult OtherIncomeChart()
        {
            var chart = new List<PieChartDto>();
            var otherIncomes = new OtherIncomePieChart().Result();
            var first = true;

            foreach (var item in otherIncomes) 
            {
                var pieChartItem = new PieChartDto(item?.OtherIncomeAccount ?? "NA", Math.Round(item?.Percentage ?? 0m, 2));

                if (first)
                {
                    pieChartItem.Explode = true;
                    first = false;
                }

                var random = new Random();
                var color = string.Format("#{0:X6}", random.Next(0x1000000));

                pieChartItem.Color = color;

                chart.Add(pieChartItem);
            }

            return Json(chart);
        }

        [HttpPost]
        public ActionResult OtherDeductionChart()
        {
            var chart = new List<PieChartDto>();
            var otherDeductions = new OtherDeductionPieChart().Result();
            var first = true;

            foreach (var item in otherDeductions)
            {
                var pieChartItem = new PieChartDto(item?.OtherDeductionAccount ?? "NA", Math.Round(item?.Percentage ?? 0m, 2));

                if (first)
                {
                    pieChartItem.Explode = true;
                    first = false;
                }

                var random = new Random();
                var color = string.Format("#{0:X6}", random.Next(0x1000000));

                pieChartItem.Color = color;

                chart.Add(pieChartItem);
            }

            return Json(chart);
        }

        [HttpPost]
        public ActionResult LoanChart()
        {
            var chart = new List<LoanChartDto>();
            var loans = new LoanLineChart().Result();

            foreach (var item in loans)
            {
                var lineChartItem = new LoanChartDto(item?.Month ?? "NA", decimal.Parse(item?.CashAdvances ?? "0"), decimal.Parse(item?.SSSLoans ?? "0"), Math.Round(item?.Amount ?? 0m, 2));
                chart.Add(lineChartItem);
            }

            return Json(chart);
        }

        [HttpPost]
        public ActionResult LeaveChart()
        {
            var chart = new List<LeaveChartDto>();
            var leaves = new LeaveLineChart().Result();

            foreach (var item in leaves)
            {
                var lineChartItem = new LeaveChartDto(item?.Month ?? "NA", decimal.Parse(item?.WithPay ?? "0"), decimal.Parse(item?.NoPay ?? "0"), Math.Round(item?.NoOfHours ?? 0m, 2));
                chart.Add(lineChartItem);
            }

            return Json(chart);
        }

        [HttpPost]
        public ActionResult OvertimeChart()
        {
            var chart = new List<OvertimeChartDto>();
            var overtimes = new OvertimeLineChart().Result();

            foreach (var item in overtimes)
            {
                var lineChartItem = new OvertimeChartDto(item?.Month ?? "NA", Math.Round(item?.NoOfHours ?? 0m, 2));
                chart.Add(lineChartItem);
            }

            return Json(chart);
        }

        public ActionResult GetMissingPunches(int dtrId)
        {
            var missingPunches = new MissingPunches() 
            {
                DTRId = dtrId
            };            

            return Json(missingPunches.Result());
        }
    }
}
