using System.ComponentModel.DataAnnotations;

namespace FoodSathi.Models
{
    public class MenuItem
    {
        [Key] // Primary Key
        public int ItemID { get; set; }

        [Required] // cannot be null
        [StringLength(100)] // max length
        public string ItemName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 10000)] // valid price range
        public decimal Price { get; set; }

        public string ImageURL { get; set; } // store direct image links

        public bool IsAvailable { get; set; } = true;

        [Required]
        public string Category { get; set; } // Pizza, Burger, Drinks, Desserts
    }
}
