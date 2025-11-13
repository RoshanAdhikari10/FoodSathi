using Microsoft.AspNetCore.Mvc;
using FoodSathi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using FoodSathi.Helpers;

namespace FoodSathi.Controllers
{
    public class PaymentController : Controller
    {
        private readonly MenuDbContext _context;

        public PaymentController(MenuDbContext context) 
        {
            _context = context;
        }

   
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

            string userEmail = "chhetrirosun@gmail.com";
            await EmailHelper.SendEmailAsync(userEmail,
                "Payment Successful ✅",
                $"<h3>Thank you {order.UserName}!</h3><p>Your order #{orderId} was successfully paid via Card.</p>");

            await EmailHelper.NotifyAdminAsync(
                "New Payment Received 💳",
                $"<p>User <b>{order.UserName}</b> just paid Rs. {order.TotalAmount} for Order #{orderId} via Card.</p>");

            TempData["Success"] = "✅ Card payment successful! Email sent to you and admin.";
            return RedirectToAction("OrderConfirmation", "Order", new { id = orderId });
        }
        [HttpGet]
        public async Task<IActionResult> PayByCardFromCart()
        {
            var userName = User.Identity?.Name ?? "Guest";
            var cartItems = await _context.Carts
                .Include(c => c.MenuItem)
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            decimal totalAmount = cartItems.Sum(c => c.Quantity * c.MenuItem.Price);
            int itemCount = cartItems.Count;

            ViewBag.TotalAmount = totalAmount;
            ViewBag.ItemCount = itemCount;
            ViewBag.Address = cartItems.First().Address ?? "Not Provided";
            ViewBag.FromCart = true;

            return View("CardPayment"); 
        }

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

            string address = cartItems.First().Address ?? "Not Provided";
            string deliveryOption = cartItems.First().DeliveryOption ?? "Standard";

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
                    ItemImage = c.MenuItem.ImagePath,  
                    Quantity = c.Quantity,
                    TotalPrice = total,
                    TotalAmount = total,
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

            string userEmail = "chhetrirosun@gmail.com";
            await EmailHelper.SendEmailAsync(userEmail,
                "Payment Successful (Cart) ✅",
                $"<h3>Thank you {userName}!</h3><p>Your payment for {itemCount} item(s) totaling Rs. {totalAmount} via Card was successful.</p>");

            await EmailHelper.NotifyAdminAsync(
                "New Cart Payment 💳",
                $"<p>User <b>{userName}</b> paid Rs. {totalAmount} for {itemCount} item(s) via Card.</p>");

            TempData["Success"] = "✅ Cart payment successful! Email sent to you and admin.";
            return RedirectToAction("OrderConfirmationFromCart", "Order", new { amount = totalAmount, itemCount, paymentMethod = "Card" });
        }


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

            string userEmail = "chhetrirosun@gmail.com";
            await EmailHelper.SendEmailAsync(userEmail,
                $"Payment Successful via {walletType} Wallet ✅",
                $"<h3>Thank you {order.UserName}!</h3><p>Your order #{orderId} was successfully paid using {walletType} Wallet.</p>");

            await EmailHelper.NotifyAdminAsync(
                $"New {walletType} Wallet Payment 💼",
                $"<p>User <b>{order.UserName}</b> paid Rs. {order.TotalAmount} for Order #{orderId} using {walletType} Wallet.</p>");

            TempData["Success"] = $"✅ Payment successful via {walletType} Wallet! Email sent to you and admin.";
            return RedirectToAction("OrderConfirmation", "Order", new { id = orderId });
        }

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

            string address = cartItems.First().Address ?? "Not Provided";
            string deliveryOption = cartItems.First().DeliveryOption ?? "Standard";

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
                    ItemImage = c.MenuItem.ImagePath,  
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

            string userEmail = "chhetrirosun@gmail.com";
            await EmailHelper.SendEmailAsync(userEmail,
                $"Wallet Payment Successful ✅",
                $"<h3>Thank you {userName}!</h3><p>Your payment of Rs. {totalAmount} via {walletType} Wallet for {itemCount} item(s) was successful.</p>");

            await EmailHelper.NotifyAdminAsync(
                $"New {walletType} Wallet Payment 💼",
                $"<p>User <b>{userName}</b> paid Rs. {totalAmount} for {itemCount} item(s) using {walletType} Wallet.</p>");

            TempData["Success"] = $"✅ {walletType} Wallet payment successful! Email sent to you and admin.";
            return RedirectToAction("OrderConfirmationFromCart", "Order", new { amount = totalAmount, itemCount, paymentMethod = walletType });
        }


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

        [HttpGet]
        public async Task<IActionResult> PayByWalletFromCart()
        {
            var userName = User.Identity?.Name ?? "Guest";
            var cartItems = await _context.Carts
                .Include(c => c.MenuItem)
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }


            decimal totalAmount = cartItems.Sum(c => c.Quantity * c.MenuItem.Price);

            ViewBag.Amount = totalAmount;
            ViewBag.FromCart = true;

            return View("WalletPayment");
        }

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

            string userEmail = "chhetrirosun@gmail.com";
            await EmailHelper.SendEmailAsync(userEmail,
                "Order Placed - Cash on Delivery 💵",
                $"<h3>Your order #{id} has been placed successfully!</h3><p>Please pay cash upon delivery.</p>");

            await EmailHelper.NotifyAdminAsync(
                "New Cash on Delivery Order 🧾",
                $"<p>User <b>{order.UserName}</b> placed Order #{id} using Cash on Delivery.</p>");

            TempData["Success"] = "💵 Order placed successfully! Email sent to you and admin.";
            return RedirectToAction("OrderConfirmation", "Order", new { id });
        }

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

            string address = cartItems.First().Address ?? "Not Provided";
            string deliveryOption = cartItems.First().DeliveryOption ?? "Standard";

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
                    ItemImage = c.MenuItem.ImagePath,  
                    Quantity = c.Quantity,
                    TotalPrice = total,
                    TotalAmount = total,
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

            string userEmail = "chhetrirosun@gmail.com";
            await EmailHelper.SendEmailAsync(userEmail,
                "Order Placed (Cart) - Cash on Delivery 💵",
                $"<h3>Hi {userName}!</h3><p>Your order for {itemCount} item(s) totaling Rs. {totalAmount} has been placed successfully. Please pay cash on delivery.</p>");

            await EmailHelper.NotifyAdminAsync(
                "New Cash on Delivery (Cart) 🧾",
                $"<p>User <b>{userName}</b> placed {itemCount} item(s) order totaling Rs. {totalAmount} using Cash on Delivery.</p>");

            TempData["Success"] = "💵 Cart order placed successfully! Email sent to you and admin.";
            return RedirectToAction("OrderConfirmationFromCart", "Order", new { amount = totalAmount, itemCount, paymentMethod = "Cash on Delivery" });
        }
    }
}