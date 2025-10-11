using Microsoft.AspNetCore.Mvc;

namespace FoodSathi.Controllers
{
    public class PaymentController : Controller
    {
        // 💳 Card Payment
        public IActionResult PayByCard(int id, decimal amount)
        {
            ViewBag.OrderId = id;
            ViewBag.Amount = amount;
            return View("CardPayment");
        }

        // 💼 Wallet Payment (eSewa/Khalti)
        public IActionResult PayByWallet(int id, decimal amount)
        {
            ViewBag.OrderId = id;
            ViewBag.Amount = amount;
            return View("WalletPayment");
        }

        // 💵 Cash on Delivery
        public IActionResult CashOnDelivery(int id)
        {
            TempData["Success"] = "Order placed successfully! Pay cash upon delivery.";
            return RedirectToAction("OrderConfirmation", "Order", new { id });
        }
    }
}
