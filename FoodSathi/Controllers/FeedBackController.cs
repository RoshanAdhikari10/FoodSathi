using Microsoft.AspNetCore.Mvc;

namespace FoodSathi.Controllers
{
    public class FeedBackController : Controller
    {
        public IActionResult feedback()
        {
            return View();
        }
    }
}
