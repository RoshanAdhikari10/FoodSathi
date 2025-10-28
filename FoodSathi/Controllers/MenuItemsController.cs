using FoodSathi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace FoodSathi.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly MenuDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // SINGLE CONSTRUCTOR - Remove any duplicate constructors!
        public MenuItemsController(MenuDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: MenuItems
        public async Task<IActionResult> Index()
        {
            var categories = await _context.MenuItems
                .Select(m => m.Category)
                .Distinct()
                .ToListAsync();

            var items = await _context.MenuItems.ToListAsync();

            ViewBag.Categories = categories;

            return View(items);
        }

        // GET: MenuItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.ItemID == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        // GET: MenuItems/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: MenuItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemID,ItemName,Description,Price,ImageFile,Category,IsAvailable")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (menuItem.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + menuItem.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file to wwwroot/images
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await menuItem.ImageFile.CopyToAsync(fileStream);
                    }

                    // Store relative path in database
                    menuItem.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Add(menuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        // GET: MenuItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        // POST: MenuItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemID,ItemName,Description,Price,ImageFile,Category,IsAvailable")] MenuItem menuItem)
        {
            if (id != menuItem.ItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing item to preserve old image path if no new image uploaded
                    var existingItem = await _context.MenuItems.AsNoTracking().FirstOrDefaultAsync(m => m.ItemID == id);

                    // Handle new image upload
                    if (menuItem.ImageFile != null)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingItem?.ImagePath))
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingItem.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Upload new image
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + menuItem.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await menuItem.ImageFile.CopyToAsync(fileStream);
                        }

                        menuItem.ImagePath = "/images/" + uniqueFileName;
                    }
                    else
                    {
                        // Keep old image path if no new image uploaded
                        menuItem.ImagePath = existingItem?.ImagePath;
                    }

                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItem.ItemID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        // GET: MenuItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.ItemID == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        // POST: MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(menuItem.ImagePath))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, menuItem.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.MenuItems.Remove(menuItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.ItemID == id);
        }
    }
}