using FoodSathi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace FoodSathi.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly MenuDbContext _context;

        public CartController(MenuDbContext context)
        {
            _context = context;
        }

        // Show cart items
        public IActionResult Index()
        {
            var userName = User.Identity?.Name ?? "Guest";
            var cart = _context.Carts
                .Include(c => c.MenuItem)
                .Where(c => c.UserName == userName)
                .ToList();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity)
        {
            // ✅ Find the menu item
            var item = _context.MenuItems.FirstOrDefault(m => m.ItemID == itemId);
            if (item == null)
                return NotFound();

            // ✅ Get logged-in user
            var userName = User.Identity?.Name ?? "Guest";

            // ✅ Check if item already exists in cart for the same user
            var cartItem = _context.Carts.FirstOrDefault(c => c.ItemID == itemId && c.UserName == userName);

            if (cartItem != null)
            {
                // Update quantity if item exists
                cartItem.Quantity += quantity;
                _context.Carts.Update(cartItem);
            }
            else
            {
                // ✅ Add a new item to the cart
                var newCartItem = new Cart
                {
                    ItemID = item.ItemID,
                    MenuItem = item,
                    Quantity = quantity,
                    UserName = userName,
                    Address = "Not Provided",
                    DeliveryOption = "Standard"
                };
                _context.Carts.Add(newCartItem);
            }

            _context.SaveChanges();

            // Redirect to cart page
            return RedirectToAction("Index", "Cart");
        }

        // Remove single item
        public IActionResult Remove(int cartId)
        {
            var userName = User.Identity?.Name ?? "Guest";
            var cartItem = _context.Carts
                .FirstOrDefault(c => c.CartID == cartId && c.UserName == userName);

            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Clear all cart items for current user
        public IActionResult Clear()
        {
            var userName = User.Identity?.Name ?? "Guest";
            var allItems = _context.Carts
                .Where(c => c.UserName == userName)
                .ToList();

            _context.Carts.RemoveRange(allItems);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ✅ NEW: Update cart items with delivery information before checkout
        [HttpPost]
        public async Task<IActionResult> UpdateCartDeliveryInfo(string address, string deliveryOption)
        {
            var userName = User.Identity?.Name ?? "Guest";

            var cartItems = await _context.Carts
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
                return Json(new { success = false, message = "Cart is empty" });

            // Update all cart items with delivery info
            foreach (var item in cartItems)
            {
                item.Address = address;
                item.DeliveryOption = deliveryOption;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cart updated successfully" });
        }
    }
}