using Microsoft.AspNetCore.Mvc;
using FoodSathi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FoodSathi.Controllers
{
    public class PaymentController : Controller
    {
        private readonly MenuDbContext _context;

        public PaymentController(MenuDbContext context)
        {
            _context = context;
        }

        // 💳 Card Payment
        [HttpGet]
        public async Task<IActionResult> PayByCard(int id, decimal amount)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            ViewBag.OrderId = id;
            ViewBag.Amount = amount;
            return View("CardPayment");
        }

        [HttpPost]
        public async Task<IActionResult> PayByCardConfirm(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.PaymentMethod = "Card";
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Card payment successful!";
            return RedirectToAction("OrderConfirmation", "Order", new { id = orderId });
        }

        // 💼 Wallet Payment
        [HttpGet]
        public async Task<IActionResult> PayByWallet(int id, decimal amount)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            ViewBag.OrderId = id;
            ViewBag.Amount = amount;
            return View("WalletPayment");
        }

        [HttpPost]
        public async Task<IActionResult> PayByWalletConfirm(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();

            order.PaymentMethod = "Wallet";
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "💼 Wallet payment successful!";
            return RedirectToAction("OrderConfirmation", "Order", new { id = orderId });
        }

        // 💵 Cash on Delivery
        [HttpGet]
        public async Task<IActionResult> CashOnDelivery(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.PaymentMethod = "Cash on Delivery";
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "💵 Order placed successfully! Please pay cash upon delivery.";
            return RedirectToAction("OrderConfirmation", "Order", new { id });
        }
    }
}
