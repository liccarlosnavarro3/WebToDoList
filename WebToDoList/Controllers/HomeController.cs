using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebToDoList.Models;
using WebToDoList.Service;

namespace WebToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherService _weatherService;

        public HomeController(ILogger<HomeController> logger, WeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        public async Task<IActionResult> Index()
        {
            var weather = await _weatherService.GetWeatherAsync();
            ViewBag.Weather = weather;
            return View();
        }

        public IActionResult Privacy()
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
