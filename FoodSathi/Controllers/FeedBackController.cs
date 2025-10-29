using Microsoft.AspNetCore.Mvc;
using FoodSathi.Data;
using FoodSathi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace FoodSathi.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===========================
        // 🔹 FRONTEND SECTION (for users)
        // ===========================
        // Show feedback page
        public IActionResult Feedback()
        {
            var feedbacks = _context.Feedbacks
                .OrderByDescending(f => f.Date)
                .ToList();

            ViewBag.AvgRating = feedbacks.Count > 0 ? feedbacks.Average(f => f.Rating) : 0;
            return View(feedbacks);
        }

        // ✅ Handle feedback submission properly
        [HttpPost]
        [ValidateAntiForgeryToken] // important for security
        public IActionResult SubmitFeedback(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, errors });
            }

            feedback.Date = DateTime.Now; // ensure date is set
            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();

            return Json(new { success = true });
        }


        // ===========================
        // 🔹 ADMIN SECTION (manage feedback)
        // ===========================
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var feedbacks = _context.Feedbacks
                .OrderByDescending(f => f.Date)
                .ToList();
            return View(feedbacks);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id)
        {
            var feedback = _context.Feedbacks.Find(id);
            if (feedback == null) return NotFound();
            return View(feedback);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var feedback = _context.Feedbacks.Find(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
