using IMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IMS.Controllers
{
    public class HomeController : Controller
    {

        private readonly ImsContext _db;

        public HomeController(ImsContext db)
        {
            _db = db;
        }
 
        public IActionResult Index()
        {
            var order_status = _db.Orders.Count(o => o.Status == "Pending");
            ViewBag.OrderStatus = order_status;
            var out_of_stoke = _db.Products.Count(p => p.Quantity == 0);
            ViewBag.OutOfStoke = out_of_stoke;
            var minimum_limit_exceed = _db.Products.Count(o => o.Quantity <= 10);
            ViewBag.MinLimExceed = minimum_limit_exceed;
            var sell_pending_order = _db.Sales.Count(s => s.Status=="Pending");
            ViewBag.PendingSale = sell_pending_order; 

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetMonthlyTurnover()
        {
            var currentDate = DateTime.Today;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var dateOnlyFirstDayOfMonth = new DateOnly(firstDayOfMonth.Year, firstDayOfMonth.Month, firstDayOfMonth.Day);
            var dateOnlyLastDayOfMonth = new DateOnly(lastDayOfMonth.Year, lastDayOfMonth.Month, lastDayOfMonth.Day);


            var monthlyTurnover = _db.OrderDetails
                .Where(o => o.Order.OrderDate >= dateOnlyFirstDayOfMonth && o.Order.OrderDate <= dateOnlyLastDayOfMonth)
                .Sum(od => od.Quantity * od.UnitPrice);

            return Json(monthlyTurnover);
        }

        [HttpGet]
        public IActionResult GetYearlyTurnover()
        {
            var currentDate = DateTime.Today;
            var firstDayOfYear = new DateTime(currentDate.Year, 1, 1);
            var dateOnlyFirstDayOfYear = new DateOnly(firstDayOfYear.Year, firstDayOfYear.Month, firstDayOfYear.Day);
            var dateOnlyCurrentDate = new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day);

            var yearlyTurnover = _db.OrderDetails
                .Where(o => o.Order.OrderDate >= dateOnlyFirstDayOfYear && o.Order.OrderDate <= dateOnlyCurrentDate)
                .Sum(o => o.Quantity * o.UnitPrice);

            return Json(yearlyTurnover);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
