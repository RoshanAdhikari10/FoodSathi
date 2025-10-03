using Microsoft.AspNetCore.Mvc;
using FoodSathi.Models;

namespace FoodSathi.Controllers
{
    public class OrderController : Controller
    {
        private readonly MenuDbContext _context;

        public OrderController(MenuDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult BuyNow(int itemId, int quantity)
        {
            var item = _context.MenuItems.Find(itemId);
            if (item == null) return NotFound();

            var order = new Order
            {
                ItemID = item.ItemID,
                ItemName = item.ItemName,
                Quantity = quantity,
                TotalPrice = item.Price * quantity,
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // ✅ Redirect with ItemName in URL
            return RedirectToAction("Checkout", new { orderName = order.ItemName });
        }




        [HttpGet("Order/Checkout/{orderName}")]
        public IActionResult Checkout(string orderName)
        {
            var order = _context.Orders
                .Where(o => o.ItemName == orderName)
                .OrderByDescending(o => o.OrderDate)  // ✅ Always latest
                .FirstOrDefault();

            if (order == null) return NotFound();

            return View(order);
        }


    }
}
