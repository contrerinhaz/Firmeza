using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Firmeza.Web.Models;


namespace Firmeza.Web.Controllers
{
    // Controller for the main landing page and general errors
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
