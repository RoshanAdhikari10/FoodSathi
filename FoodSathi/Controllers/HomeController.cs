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
        private readonly OfferDbContext _offerContext;

        public HomeController(ILogger<HomeController> logger, MenuDbContext menuContext, OfferDbContext offerContext)
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

        // Offer Page
        public async Task<IActionResult> Offer()
        {
            var items = await _offerContext.Offers
                .Where(o => o.IsActive && o.EndDate >= DateTime.Now)
                .ToListAsync();

            return View(items);
        }

        // Menu Page
        public async Task<IActionResult> Menu()
        {
            var items = await _menuContext.MenuItems.ToListAsync();
            return View(items);
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
