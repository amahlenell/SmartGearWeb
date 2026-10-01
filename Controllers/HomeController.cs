using Microsoft.AspNetCore.Mvc;
using SmartGearWeb.Services;

namespace SmartGearWeb.Controllers
{
    public class HomeController : Controller
    {
        // The service is injected through the constructor — ASP.NET Core's
        // built-in DI container resolves IStockNotificationService to
        // StockNotificationService automatically, based on the Program.cs registration.
        private readonly IStockNotificationService _stockService;

        public HomeController(IStockNotificationService stockService)
        {
            _stockService = stockService;
        }

        public IActionResult Index()
        {
            var message = _stockService.GetLowStockMessage("Rugby jersey - medium", 3);
            ViewBag.StockMessage = message;
            return View();
        }
    }
}
