using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSathi.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int ItemID { get; set; }

        [ForeignKey("ItemID")]
        public MenuItem MenuItem { get; set; }

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public string? UserName { get; set; } // Optional - if you track users
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
