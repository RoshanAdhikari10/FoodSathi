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
           
            var item = _context.MenuItems.FirstOrDefault(m => m.ItemID == itemId);
            if (item == null)
                return NotFound();

            
            var userName = User.Identity?.Name ?? "Guest";

            
            var cartItem = _context.Carts.FirstOrDefault(c => c.ItemID == itemId && c.UserName == userName);

            if (cartItem != null)
            {
                
                cartItem.Quantity += quantity;
                _context.Carts.Update(cartItem);
            }
            else
            {
                
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

           
            return RedirectToAction("Index", "Cart");
        }

        
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

        
        [HttpPost]
        public async Task<IActionResult> UpdateCartDeliveryInfo(string address, string deliveryOption)
        {
            var userName = User.Identity?.Name ?? "Guest";

            var cartItems = await _context.Carts
                .Where(c => c.UserName == userName)
                .ToListAsync();

            if (!cartItems.Any())
                return Json(new { success = false, message = "Cart is empty" });

          
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