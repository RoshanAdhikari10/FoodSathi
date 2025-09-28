using FoodSathi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FoodSathi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MenuDbContext _menuContext;
        private readonly MenuDbContext _offerContext;

        public HomeController(ILogger<HomeController> logger, MenuDbContext menuContext, MenuDbContext offerContext)
        {
            _logger = logger;
            _menuContext = menuContext;
            _offerContext = offerContext;  // ✅ assign properly
        }

        // Home Page
        public IActionResult Index()
        {
            return View();
        }


        // Error Page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
