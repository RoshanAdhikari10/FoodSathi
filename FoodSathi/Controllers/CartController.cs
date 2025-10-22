using FoodSathi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var cart = _context.Carts.Include(c => c.MenuItem).ToList();
            return View(cart);
        }

        // Add item to cart
        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity)
        {
            var item = _context.MenuItems.FirstOrDefault(m => m.ItemID == itemId);
            if (item == null) return NotFound();

            var cartItem = _context.Carts.FirstOrDefault(c => c.ItemID == itemId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                _context.Update(cartItem);
            }
            else
            {
                var newCartItem = new Cart
                {
                    ItemID = item.ItemID,
                    Quantity = quantity
                };
                _context.Carts.Add(newCartItem);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Remove single item
        public IActionResult Remove(int cartId)
        {
            var cartItem = _context.Carts.Find(cartId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Clear all cart items
        public IActionResult Clear()
        {
            var allItems = _context.Carts.ToList();
            _context.Carts.RemoveRange(allItems);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
