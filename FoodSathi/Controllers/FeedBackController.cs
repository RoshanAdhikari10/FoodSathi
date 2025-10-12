using Microsoft.AspNetCore.Mvc;
using FoodSathi.Data;
using FoodSathi.Models;
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

        // Display Feedback page
        public IActionResult feedback()
        {
            var feedbacks = _context.Feedbacks.OrderByDescending(f => f.Date).ToList();
            ViewBag.AvgRating = feedbacks.Count > 0 ? feedbacks.Average(f => f.Rating) : 0;
            return View(feedbacks);
        }

        // Submit feedback
        [HttpPost]
        public IActionResult SubmitFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}

