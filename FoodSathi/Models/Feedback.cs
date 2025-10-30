using System;
using System.ComponentModel.DataAnnotations;

namespace FoodSathi.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; }  // From logged-in user

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // From logged-in user

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }  // From logged-in user

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Feedback message is required")]
        [StringLength(1000)]
        [Display(Name = "Your Feedback")]
        public string Message { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.Now;
    }
}