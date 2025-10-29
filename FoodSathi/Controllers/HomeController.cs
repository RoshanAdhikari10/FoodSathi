using FoodSathi.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }


        // ✅ Home Page (Dynamic Featured Dishes)
        public async Task<IActionResult> Index()
        {
            // 🍽️ Fetch top 3 latest menu items
            var featuredItems = await _menuContext.MenuItems
                .OrderByDescending(m => m.ItemID)
                .Take(3)
                .ToListAsync();

            // 💬 Fetch top 3 feedback with high ratings (4 or 5 stars)
            var topFeedbacks = await _menuContext.Feedbacks
                .Where(f => f.Rating >= 4)
                .OrderByDescending(f => f.Rating)
                .ThenByDescending(f => f.Date)
                .Take(3)
                .ToListAsync();

            // Pass both to View
            ViewBag.TopFeedbacks = topFeedbacks;

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
