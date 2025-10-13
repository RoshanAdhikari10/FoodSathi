using FoodSathi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FoodSathi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MenuDbContext _menuContext;

        public HomeController(ILogger<HomeController> logger, MenuDbContext menuContext)
        {
            _logger = logger;
            _menuContext = menuContext;
        }

        // ✅ Home Page (Dynamic Featured Dishes)
        public async Task<IActionResult> Index()
        {
   
            var featuredItems = await _menuContext.MenuItems
                .OrderByDescending(m => m.ItemID)
                .Take(3)
                .ToListAsync();

            return View(featuredItems);
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
