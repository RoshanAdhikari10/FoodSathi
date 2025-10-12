using System;
using System.ComponentModel.DataAnnotations;

namespace FoodSathi.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Review { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
