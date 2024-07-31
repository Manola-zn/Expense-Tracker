using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.IdentitySample.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ExpenseTrackerApp.Controllers
{
    public class DashboardController : Controller
    {
        private ExpenseTrackerDBEntities db = new ExpenseTrackerDBEntities();
        private DateTime _startDate = DateTime.Today.AddDays(-6); 
        private DateTime _endDate = DateTime.Today;

        public ActionResult FilterDates(DateTime? date_start, DateTime? date_end)
        {
            if (date_start.HasValue && date_end.HasValue)
            {
                if (date_start.Value > date_end.Value)
                {
                    ModelState.AddModelError("", "Start date cannot be greater than end date.");
                    return View("Index");
                }
                else if (!date_start.HasValue || !date_end.HasValue)
                {
                    ModelState.AddModelError("", "Both start date and end date are required.");
                    return View("Index");
                }

                else if (date_start.Value > DateTime.Now || date_end.Value > DateTime.Now)
                {
                    ModelState.AddModelError("", "Dates cannot be in the future.");
                    return View("Index");
                }
                else
                {
                    _startDate = date_start.Value;
                    _endDate = date_end.Value;

                    ViewBag.StartDate = _startDate;
                    ViewBag.EndDate = _endDate;

                    TempData["StartDate"] = _startDate;
                    TempData["EndDate"] = _endDate;
                }
            }
            else
            {
                ViewBag.StartDate = _startDate;
                ViewBag.EndDate = _endDate;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            ViewBag.StartDate = TempData["StartDate"] != null ? ((DateTime)TempData["StartDate"]).ToString("yyyy-MM-dd") : _startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = TempData["EndDate"] != null ? ((DateTime)TempData["EndDate"]).ToString("yyyy-MM-dd") : _endDate.ToString("yyyy-MM-dd");

            //Last 7 Days
            DateTime StartDate = DateTime.Parse(ViewBag.StartDate);
            DateTime EndDate = DateTime.Parse(ViewBag.EndDate);

            List<Transaction> SelectedTransactions = db.Transactions
               .Include(x => x.Category)
               .Where(y => y.transactionDate >= StartDate && y.transactionDate <= EndDate)
               .ToList();

            //Total Income
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.categoryType == "Income")
                .Sum(j => j.transactionAmount);
            ViewBag.TotalIncome = TotalIncome.ToString("C", CultureInfo.GetCultureInfo("en-ZA"));

            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.categoryType == "Expense")
                .Sum(j => j.transactionAmount);
            ViewBag.TotalExpense = TotalExpense.ToString("C", CultureInfo.GetCultureInfo("en-ZA"));

            //Balance
            int Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C", CultureInfo.GetCultureInfo("en-ZA"));

            //Doughnut Chart - Expense By Category
            var doughnutChartData = SelectedTransactions
                                    .Where(i => i.Category.categoryType == "Expense")
                                    .GroupBy(j => j.Category.categoryID)
                                    .Select(k => new
                                    {
                                        categoryTitleWithIcon = k.First().Category.categoryIcon + " " + k.First().Category.categoryTitle,
                                        amount = k.Sum(j => j.transactionAmount),
                                        formattedAmount = k.Sum(j => j.transactionAmount).ToString("C", CultureInfo.GetCultureInfo("en-ZA")),
                                    })
                                   .OrderByDescending(l => l.amount)
                                   .ToList();

            // Pass data to ViewBag
            ViewBag.DoughnutChartData = Newtonsoft.Json.JsonConvert.SerializeObject(doughnutChartData);

            //Spline Chart - Income vs Expense

            //Income
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.Category.categoryType == "Income")
                .GroupBy(j => j.transactionDate)
                .Select(k => new SplineChartData()
                {
                    day = k.First().transactionDate.ToString("dd-MM"),
                    income = k.Sum(l => l.transactionAmount)
                })
                .ToList();

            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.categoryType == "Expense")
                .GroupBy(j => j.transactionDate)
                .Select(k => new SplineChartData()
                {
                    day = k.First().transactionDate.ToString("dd-MM"),
                    expense = k.Sum(l => l.transactionAmount)
                })
                .ToList();

            //Combine Income & Expense
            int numberOfDays = (EndDate - StartDate).Days + 1;

            List<string> dateList = new List<string>();
            DateTime currentDate = StartDate; //Assuming StartDate is defined somewhere

            for (int i = 0; i < numberOfDays; i++)
            {
                dateList.Add(currentDate.ToString("dd-MM"));
                currentDate = currentDate.AddDays(1);
            }

            string[] dateArray = dateList.ToArray();

            ViewBag.SplineChartData = from day in dateArray
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            //Recent Transactions
            ViewBag.RecentTransactions = db.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.transactionDate)
                .Take(5)
                .ToList();
                                  
            return View();
        }
        public class SplineChartData
        {
            public string day;
            public int income;
            public int expense;

        }
    }
}
