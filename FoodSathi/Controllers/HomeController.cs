using FoodSathi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FoodSathi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MenuDbContext _context;

        // ✅ Inject both logger and MenuDbContext
        public HomeController(ILogger<HomeController> logger, MenuDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ✅ Home Page
        public IActionResult Index()
        {
            return View();
        }

        // ✅ Offer Page
        public IActionResult Offer()
        {
            return View();
        }

        // ✅ Menu Page (Fetch items from DB)
        public async Task<IActionResult> Menu()
        {
            var items = await _context.MenuItems.ToListAsync();
            return View(items);
        }

        // ✅ Error Page
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
