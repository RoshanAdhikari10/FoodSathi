using FoodSathi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSathi.Controllers
{
    [Authorize(Roles = "User")]
    public class OrderController : Controller
    {
        private readonly MenuDbContext _context;

        public OrderController(MenuDbContext context)
        {
            _context = context;
        }

        // 🛒 From "Buy Now" button
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

            return RedirectToAction("Checkout", new { orderId = order.OrderID });
        }

        // ✅ Checkout for single or multiple items
        [HttpGet]
        public IActionResult Checkout(int? orderId = null, bool fromCart = false)
        {
            var viewModel = new CheckoutViewModel();

            if (fromCart)
            {
                // ✅ From cart
                var cartItems = _context.Carts
                    .Include(c => c.MenuItem)
                    .ToList();

                if (cartItems == null || !cartItems.Any())
                    return RedirectToAction("Menu", "MenuItems");

                viewModel.FromCart = true;
                viewModel.CartItems = cartItems;
                viewModel.TotalAmount = cartItems.Sum(c => c.MenuItem.Price * c.Quantity);
            }
            else
            {
                // ✅ Single item (Buy Now)
                var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);
                if (order == null)
                    return RedirectToAction("Menu", "MenuItems");

                viewModel.FromCart = false;
                viewModel.SingleOrder = order;
                viewModel.TotalAmount = order.TotalPrice;
            }

            return View(viewModel);
        }

        // ✅ Proceed to payment (from checkout)
        [HttpPost]
        public IActionResult ProceedToPayment(int? orderId, bool fromCart)
        {
            if (fromCart)
            {
                var cartItems = _context.Carts.Include(c => c.MenuItem).ToList();
                if (!cartItems.Any())
                    return RedirectToAction("Menu", "MenuItems");

                decimal total = cartItems.Sum(c => c.MenuItem.Price * c.Quantity);
                return RedirectToAction("CartPayment", "Payment", new { amount = total });
            }
            else
            {
                var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);
                if (order == null) return RedirectToAction("Menu", "MenuItems");

                return RedirectToAction("PayByCard", "Payment", new { orderId = order.OrderID, amount = order.TotalPrice });
            }
        }

        // ✅ All orders list
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return View(orders);
        }

        // ✅ Confirmation Page
        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
