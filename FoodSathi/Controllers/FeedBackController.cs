using FoodSathi.Data;
using FoodSathi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSathi.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            try
            {
                var feedbacks = await _context.Feedbacks
                    .OrderByDescending(f => f.SubmittedDate)
                    .ToListAsync();

                ViewBag.AvgRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;

               
                if (User.Identity.IsAuthenticated)
                {
                    ViewBag.UserName = User.Identity.Name;
                    ViewBag.UserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
                }

                ViewBag.Success = TempData["Success"] as string;
                return View(feedbacks);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading feedback: " + ex.Message;
                ViewBag.AvgRating = 0;
                return View(new List<Feedback>());
            }
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(Feedback feedback)
        {
            try
            {
               
                if (User.Identity.IsAuthenticated)
                {
                    feedback.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    feedback.Name = User.Identity.Name;
                    feedback.Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? feedback.Email;
                }

                feedback.SubmittedDate = DateTime.Now;

                ModelState.Remove("UserId");
                ModelState.Remove("Name");
                ModelState.Remove("Email");

                if (ModelState.IsValid)
                {
                    _context.Feedbacks.Add(feedback);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Thank you for your feedback!";
                    return RedirectToAction(nameof(Index));
                }

                return View("Index", await _context.Feedbacks.ToListAsync());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving feedback: " + ex.Message);
                return View("Index", await _context.Feedbacks.ToListAsync());
            }
        }

           
        public async Task<IActionResult> Manage()                      
        {
            var feedbacks = await _context.Feedbacks
                .OrderByDescending(f => f.SubmittedDate)
                .ToListAsync();
            return View(feedbacks);
        }

       
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Manage));
        }
    
    }
}
