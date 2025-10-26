using Microsoft.AspNetCore.Mvc;
using FoodSathi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace FoodSathi.Controllers
{
    public class PaymentController : Controller
    {
        private readonly MenuDbContext _context;

        public PaymentController(MenuDbContext context)
        {
            _context = context;
        }

        // ====================================================
        // 💳 CARD PAYMENT
        // ====================================================

        // ✅ GET - Single Order
        [HttpGet]
        public async Task<IActionResult> PayByCard(int? id, decimal? amount)
        {
            if (id == null || amount == null)
                return BadRequest("Missing order ID or amount.");

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found.");

            ViewBag.OrderId = id.Value;
            ViewBag.Amount = amount.Value;
            ViewBag.FromCart = false;

            return View("CardPayment");
        }

        // ✅ POST - Single Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayByCardConfirm(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound("Order not found.");

            order.UserName = User.Identity?.Name ?? "Guest";
            order.PaymentMethod = "Card";
            order.PaymentStatus = "Paid";

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Card payment successful!";
            return RedirectToAction("OrderConfirmation", "Order", new { id = orderId });
        }

        // ✅ GET - From Cart
        [HttpGet]
        public async Task<IActionResult> PayByCardFromCart()
        {
            var userName = User.Identity?.Name ?? "Guest";

            var cartItems = await _context.Carts
                .Include(c => c.MenuItem)
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            ViewBag.Amount = cartItems.Sum(c => c.Quantity * c.MenuItem.Price);
            ViewBag.FromCart = true;

            return View("CardPayment");
        }

        // ✅ POST - From Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayByCardConfirmFromCart()
        {
            var userName = User.Identity?.Name ?? "Guest";

            var cartItems = await _context.Carts
                .Include(c => c.MenuItem)
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            // Get delivery info from cart items (first item as reference)
            var firstCart = cartItems.First();
            string address = firstCart.Address ?? "Not Provided";
            string deliveryOption = firstCart.DeliveryOption ?? "Standard";

            decimal totalAmount = 0;
            int itemCount = 0;

            foreach (var c in cartItems)
            {
                var itemTotal = c.Quantity * c.MenuItem.Price;
                totalAmount += itemTotal;
                itemCount++;

                var order = new Order
                {
                    ItemID = c.ItemID,
                    ItemName = c.MenuItem.ItemName,
                    Quantity = c.Quantity,
                    TotalPrice = itemTotal,
                    TotalAmount = itemTotal,
                    Address = address,
                    DeliveryOption = deliveryOption,
                    PaymentMethod = "Card",
                    PaymentStatus = "Paid",
                    UserName = userName,
                    OrderDate = DateTime.Now
                };
                _context.Add(order);
            }

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmationFromCart", "Order", new
            {
                amount = totalAmount,
                itemCount = itemCount,
                paymentMethod = "Card"
            });
        }

        // ====================================================
        // 💼 WALLET PAYMENT
        // ====================================================

        // ✅ GET - Single Order
        [HttpGet]
        public async Task<IActionResult> PayByWallet(int? id, decimal? amount)
        {
            if (id == null || amount == null)
                return BadRequest("Missing order ID or amount.");

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found.");

            ViewBag.OrderId = id.Value;
            ViewBag.Amount = amount.Value;
            ViewBag.FromCart = false;

            return View("WalletPayment");
        }

        // ✅ POST - Single Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayByWalletConfirm(int orderId, string walletType)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound("Order not found.");

            order.UserName = User.Identity?.Name ?? "Guest";
            order.PaymentMethod = walletType;
            order.PaymentStatus = "Paid";

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"✅ Payment successful via {walletType} Wallet!";
            return RedirectToAction("OrderConfirmation", "Order", new { id = orderId });
        }

        // ✅ GET - From Cart
        [HttpGet]
        public async Task<IActionResult> PayByWalletFromCart()
        {
            var userName = User.Identity?.Name ?? "Guest";

            var cartItems = await _context.Carts
                .Include(c => c.MenuItem)
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            ViewBag.Amount = cartItems.Sum(c => c.Quantity * c.MenuItem.Price);
            ViewBag.FromCart = true;

            return View("WalletPayment");
        }

        // ✅ POST - From Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayByWalletConfirmFromCart(string walletType)
        {
            var userName = User.Identity?.Name ?? "Guest";

            var cartItems = await _context.Carts
                .Include(c => c.MenuItem)
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            // Get delivery info from cart items
            var firstCart = cartItems.First();
            string address = firstCart.Address ?? "Not Provided";
            string deliveryOption = firstCart.DeliveryOption ?? "Standard";

            decimal totalAmount = 0;
            int itemCount = 0;

            foreach (var c in cartItems)
            {
                var total = c.Quantity * c.MenuItem.Price;
                totalAmount += total;
                itemCount++;

                var order = new Order
                {
                    ItemID = c.ItemID,
                    ItemName = c.MenuItem.ItemName,
                    Quantity = c.Quantity,
                    TotalPrice = total,
                    TotalAmount = total,
                    Address = address,
                    DeliveryOption = deliveryOption,
                    PaymentMethod = walletType,
                    PaymentStatus = "Paid",
                    UserName = userName,
                    OrderDate = DateTime.Now
                };

                _context.Add(order);
            }

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmationFromCart", "Order", new
            {
                amount = totalAmount,
                itemCount = itemCount,
                paymentMethod = walletType
            });
        }


        // ====================================================
        // 💵 CASH ON DELIVERY
        // ====================================================

        // ✅ Single Order
        [HttpGet]
        public async Task<IActionResult> CashOnDelivery(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found.");

            order.UserName = User.Identity?.Name ?? "Guest";
            order.PaymentMethod = "Cash on Delivery";
            order.PaymentStatus = "Pending";

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "💵 Order placed successfully! Please pay cash upon delivery.";
            return RedirectToAction("OrderConfirmation", "Order", new { id });
        }

        // ✅ From Cart
        [HttpGet]
        public async Task<IActionResult> CashOnDeliveryFromCart()
        {
            var userName = User.Identity?.Name ?? "Guest";

            var cartItems = await _context.Carts
                .Include(c => c.MenuItem)
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            // Get delivery info from cart items
            var firstCart = cartItems.First();
            string address = firstCart.Address ?? "Not Provided";
            string deliveryOption = firstCart.DeliveryOption ?? "Standard";

            decimal totalAmount = 0;
            int itemCount = 0;

            foreach (var c in cartItems)
            {
                var itemTotal = c.Quantity * c.MenuItem.Price;
                totalAmount += itemTotal;
                itemCount++;

                var order = new Order
                {
                    ItemID = c.ItemID,
                    ItemName = c.MenuItem.ItemName,
                    Quantity = c.Quantity,
                    TotalPrice = itemTotal,
                    TotalAmount = itemTotal,
                    Address = address,
                    DeliveryOption = deliveryOption,
                    PaymentMethod = "Cash on Delivery",
                    PaymentStatus = "Pending",
                    UserName = userName,
                    OrderDate = DateTime.Now
                };
                _context.Add(order);
            }

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmationFromCart", "Order", new
            {
                amount = totalAmount,
                itemCount = itemCount,
                paymentMethod = "Cash on Delivery"
            });
        }
    }
}