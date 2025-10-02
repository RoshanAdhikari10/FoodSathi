using Microsoft.AspNetCore.Mvc;
using FoodSathi.Models;

namespace FoodSathi.Controllers
{
    public class PaymentController : Controller
    {
        private readonly MenuDbContext _context;

        public PaymentController(MenuDbContext context)
        {
            _context = context;
        }

        // ✅ Show payment page
        [HttpGet("Payment/Pay/{orderId}")]
        public IActionResult Pay(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order == null) return NotFound();

            return View(order); // will load Views/Payment/Pay.cshtml
        }

        // ✅ Handle payment submission
        [HttpPost]
        public IActionResult ConfirmPayment(int orderId, string method)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order == null) return NotFound();

            // Mark as paid (add a PaidDate or Status column in your Order model if you want)
            // Example:
            // order.Status = "Paid";
            // order.PaymentMethod = method;
            // _context.SaveChanges();

            return RedirectToAction("Success", new { orderId = orderId });
        }

        // ✅ Payment success page
        public IActionResult Success(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);
            if (order == null) return NotFound();

            return View(order); // Views/Payment/Success.cshtml
        }
    }
}
