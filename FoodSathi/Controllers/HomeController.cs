using FoodSathi.Data;
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
        private readonly ApplicationDbContext _context; // ✅ Feedback database

        public HomeController(
            ILogger<HomeController> logger,
            MenuDbContext menuContext,
            ApplicationDbContext context)
        {
            _logger = logger;
            _menuContext = menuContext;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // ✅ Home Page (Dynamic Featured Dishes + Top Feedbacks)
        public async Task<IActionResult> Index()
        {
            // 🍽️ Fetch top 3 latest menu items
            var featuredItems = await _menuContext.MenuItems
                .OrderByDescending(m => m.ItemID)
                .Take(3)
                .ToListAsync();

            // 💬 Fetch top 3 feedbacks with rating ≥ 4
            var topFeedbacks = await _context.Feedbacks
                .Where(f => f.Rating >= 4)
                .OrderByDescending(f => f.Rating)
                .ThenByDescending(f => f.SubmittedDate)
                .Take(3)
                .ToListAsync();

            ViewBag.TopFeedbacks = topFeedbacks;

            return View(featuredItems);
        }

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
