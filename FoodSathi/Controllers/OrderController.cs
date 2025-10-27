using FoodSathi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSathi.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class OrderController : Controller
    {
        private readonly MenuDbContext _context;

        public OrderController(MenuDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "User")]
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
                TotalAmount = item.Price * quantity,
                Address = string.Empty,
                DeliveryOption = "Standard",
                PaymentMethod = "Pending",
                OrderDate = DateTime.Now,
                UserName = User.Identity.Name // ✅ Assign logged-in username
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("Checkout", new { orderId = order.OrderID });
        }

        [Authorize(Roles = "User")]
        // ✅ Checkout for single or multiple items
        [HttpGet]
        public IActionResult Checkout(int? orderId)
        {
            if (orderId.HasValue)
            {
                var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);
                if (order == null) return NotFound();

                var model = new CheckoutViewModel
                {
                    FromCart = false,
                    SingleOrder = order,
                    TotalAmount = order.TotalAmount
                };

                return View(model);
            }
            else
            {
                // 🛒 Checkout from Cart
                var userName = User.Identity?.Name ?? "Guest";
                var cartItems = _context.Carts
                    .Include(c => c.MenuItem)
                    .Where(c => c.UserName == userName)
                    .ToList();

                var total = cartItems.Sum(c => c.MenuItem.Price * c.Quantity);

                var model = new CheckoutViewModel
                {
                    FromCart = true,
                    CartItems = cartItems,
                    TotalAmount = total
                };

                return View(model);
            }
        }

        [Authorize(Roles = "User")]
        // ✅ Proceed to payment (from checkout)
        [HttpPost]
        public IActionResult ProceedToPayment(int? orderId, bool fromCart)
        {
            if (fromCart)
            {
                var userName = User.Identity?.Name ?? "Guest";
                var cartItems = _context.Carts
                    .Include(c => c.MenuItem)
                    .Where(c => c.UserName == userName)
                    .ToList();

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
        // ✅ Admin: Manage All Orders
        [Authorize(Roles = "Admin")]
        public IActionResult ManageOrders()
        {
            // Get all orders sorted by most recent
            var orders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders); // Pass data to ManageOrders.cshtml
        }

        [Authorize(Roles = "Admin")]
        // POST: Update payment status via AJAX
        [HttpPost]
        public IActionResult UpdatePayment(int id, string paymentStatus)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.PaymentStatus = paymentStatus;
                _context.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult DetailsPartial(int id)
        {
            System.Diagnostics.Debug.WriteLine($"DetailsPartial called with ID: {id}");

            var order = _context.Orders.Find(id);

            if (order == null)
            {
                System.Diagnostics.Debug.WriteLine($"Order {id} not found");
                return Content("<p class='text-danger'>Order not found!</p>", "text/html");
            }

            System.Diagnostics.Debug.WriteLine($"Order found: {order.ItemName}");
            return PartialView("_OrderDetailsPartial", order);
        }
    


        [Authorize(Roles = "User")]
        // ✅ All orders list
        public IActionResult Orders()
        {
            var userName = User.Identity?.Name ?? "Guest";
            var orders = _context.Orders
                .Where(o => o.UserName == userName)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return View(orders);
        }
        [Authorize(Roles = "User")]
        // ✅ Confirmation Page
        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            // Set ViewBag values for confirmation page
            ViewBag.FromCart = false;
            ViewBag.OrderId = order.OrderID;
            ViewBag.Amount = order.TotalAmount;
            ViewBag.ItemCount = 1;
            ViewBag.PaymentMethod = order.PaymentMethod;
            ViewBag.EstimatedDelivery = "45-60 minutes";

            return View(order);
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            var newOrder = new Order
            {
                Address = model.Address,
                DeliveryOption = model.DeliveryOption,
                PaymentMethod = model.PaymentOption,
                TotalAmount = model.TotalAmount,
                OrderDate = DateTime.Now,
                UserName = User.Identity?.Name ?? "Guest"
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            return RedirectToAction("OrderConfirmation", new { id = newOrder.OrderID });
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult OrderConfirmationFromCart(decimal amount, int itemCount, string paymentMethod)
        {
            ViewBag.FromCart = true;
            ViewBag.Amount = amount;
            ViewBag.ItemCount = itemCount;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.EstimatedDelivery = "45-60 minutes";

            return View("OrderConfirmation");
        }
        [Authorize(Roles = "User")]
        // ✅ NEW: Update order delivery information before payment
        [HttpPost]
        public async Task<IActionResult> UpdateOrderDeliveryInfo(int orderId, string address, string deliveryOption)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            // Update order with delivery info
            order.Address = address;
            order.DeliveryOption = deliveryOption;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Order updated successfully" });
        }
    }
}