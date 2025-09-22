using Microsoft.AspNetCore.Mvc;

namespace FoodSathi.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
