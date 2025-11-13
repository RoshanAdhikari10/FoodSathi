using FoodSathi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace FoodSathi.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly MenuDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenuItemsController(MenuDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

    
        public async Task<IActionResult> Index()
        {
            var items = await _context.MenuItems
                .Include(m => m.Category) 
                .ToListAsync();

          
            ViewBag.Categories = await _context.MenuItems
                .Select(m => m.Category.Name)  
                .Distinct()                     
                .ToListAsync();

            return View(items);
        }



     
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.ItemID == id);

            if (menuItem == null) return NotFound();

            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryID", "Name");
            return View();
        }


        [Authorize(Roles = "Admin")]
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemID,ItemName,Description,Price,ImageFile,CategoryID,IsAvailable")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                
                if (menuItem.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid() + "_" + menuItem.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await menuItem.ImageFile.CopyToAsync(stream);
                    }

                    menuItem.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Add(menuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryID", "Name", menuItem.CategoryID);
            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return NotFound();

          
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryID", "Name", menuItem.CategoryID);

            return View(menuItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ItemID,ItemName,Description,Price,ImageFile,ImagePath,CategoryID,IsAvailable")] MenuItem menuItem)
        {
            if (id != menuItem.ItemID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingItem = await _context.MenuItems.AsNoTracking().FirstOrDefaultAsync(m => m.ItemID == id);

                    if (menuItem.ImageFile != null)
                    {
                        if (!string.IsNullOrEmpty(existingItem?.ImagePath))
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingItem.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                                System.IO.File.Delete(oldImagePath);
                        }

                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid() + "_" + menuItem.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await menuItem.ImageFile.CopyToAsync(stream);
                        }

                        menuItem.ImagePath = "/images/" + uniqueFileName;
                    }
                    else
                    {
                        menuItem.ImagePath = existingItem?.ImagePath;
                    }

                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MenuItems.Any(e => e.ItemID == menuItem.ItemID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

           
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryID", "Name", menuItem.CategoryID);
            return View(menuItem);
        }


        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.ItemID == id);

            if (menuItem == null) return NotFound();

            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
     
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                if (!string.IsNullOrEmpty(menuItem.ImagePath))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, menuItem.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
