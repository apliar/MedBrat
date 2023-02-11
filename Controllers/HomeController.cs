using MedBrat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MedBrat.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 300)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public IActionResult Index() => View();

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public IActionResult About() => View();

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
        public IActionResult Partners()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}