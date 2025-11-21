using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Firmeza.Web.Models;
using Firmeza.Web.Models.ViewModels;
using Firmeza.Web.Repositories;
using Firmeza.Web.Interfaces;

namespace Firmeza.Web.Controllers
{
    // Controller for the main landing page and general errors
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ISaleRepository _saleRepo;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepo, ISaleRepository saleRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
            _saleRepo = saleRepo;
        }

        // Displays the home page with dashboard statistics
        public async Task<IActionResult> Index()
        {
            var products = await _productRepo.GetAll();
            var sales = await _saleRepo.GetAllAsync();

            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthSales = sales.Where(s => s.Date >= monthStart && s.Date <= now).ToList();

            var model = new DashboardViewModel
            {
                TotalProducts = products.Count,
                TotalSalesThisMonth = monthSales.Count,
                TotalRevenueThisMonth = monthSales.Sum(s => s.Total)
            };

            return View(model);
        }

        // Displays the privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Handles error display
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
