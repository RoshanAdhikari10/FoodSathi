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

            // Create new order
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

            // Redirect with both OrderId and OrderName
            return RedirectToAction("Checkout", new { orderName = order.ItemName });

        }

        // ✅ Route parameter style
        [HttpGet("Order/Checkout/{orderName}")]
        public IActionResult Checkout(string orderName)
        {
            var order = _context.Orders.FirstOrDefault(o => o.ItemName == orderName);
            if (order == null) return NotFound();

            return View(order);
        }


    }
}
