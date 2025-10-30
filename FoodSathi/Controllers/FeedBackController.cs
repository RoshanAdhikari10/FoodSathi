// ============================================
// 3. Controllers/FeedbackController.cs
// ============================================
using FoodSathi.Data;
using FoodSathi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeedbackApp.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Feedback/Index - Show all feedback
        public async Task<IActionResult> Index()
        {
            try
            {
                var feedbacks = await _context.Feedbacks
                    .OrderByDescending(f => f.SubmittedDate)
                    .ToListAsync();
                return View(feedbacks);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading feedback: " + ex.Message;
                return View(new List<Feedback>());
            }
        }

        // GET: Feedback/Create
        public IActionResult Create()
        {
            // Check if user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserName = User.Identity.Name;
                ViewBag.UserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
            }
            return View();
        }

        // POST: Feedback/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            try
            {
                // If user is authenticated, get their info
                if (User.Identity.IsAuthenticated)
                {
                    feedback.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    feedback.Name = User.Identity.Name;
                    feedback.Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? feedback.Email;
                }

                feedback.SubmittedDate = DateTime.Now;

                // Remove validation for fields that are auto-filled
                ModelState.Remove("UserId");
                ModelState.Remove("Name");
                ModelState.Remove("Email");

                if (ModelState.IsValid)
                {
                    _context.Add(feedback);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Thank you for your feedback!";
                    return RedirectToAction(nameof(Index));
                }

                // If we get here, something failed, redisplay form
                if (User.Identity.IsAuthenticated)
                {
                    ViewBag.UserName = User.Identity.Name;
                    ViewBag.UserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "";
                }
                return View(feedback);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving feedback: " + ex.Message);
                return View(feedback);
            }
        }

      
        // GET: Feedback/MyFeedback - Show current user's feedback
        public async Task<IActionResult> MyFeedback()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var feedbacks = await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.SubmittedDate)
                .ToListAsync();

            return View("Index", feedbacks);
        }
    }
}