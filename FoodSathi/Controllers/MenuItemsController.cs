using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodSathi.Models;

namespace FoodSathi.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly MenuDbContext _context;

        public MenuItemsController(MenuDbContext context)
        {
            _context = context;
        }

        // ✅ Home/Menu page
        public async Task<IActionResult> Menu(string category, string search)
        {
            var items = from m in _context.MenuItems select m;

            if (!string.IsNullOrEmpty(category))
                items = items.Where(m => m.Category == category);

            if (!string.IsNullOrEmpty(search))
                items = items.Where(m => m.ItemName.Contains(search) || m.Description.Contains(search));

            ViewBag.Categories = await _context.MenuItems
                .Select(m => m.Category)
                .Distinct()
                .ToListAsync();

            return View(await items.ToListAsync());
        }

        // ✅ CRUD Actions (same as yours)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.ItemID == id);
            if (menuItem == null) return NotFound();

            return View(menuItem);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemID,ItemName,Description,Price,ImageURL,IsAvailable,Category")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menuItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Menu item added successfully!";
                return RedirectToAction(nameof(Menu));
            }
            return View(menuItem);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return NotFound();

            return View(menuItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemID,ItemName,Description,Price,ImageURL,IsAvailable,Category")] MenuItem menuItem)
        {
            if (id != menuItem.ItemID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "✅ Menu item updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MenuItems.Any(e => e.ItemID == menuItem.ItemID))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Menu));
            }
            return View(menuItem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.ItemID == id);
            if (menuItem == null) return NotFound();

            return View(menuItem);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
                TempData["SuccessMessage"] = "🗑️ Menu item deleted successfully!";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Menu));
        }
    }
}
