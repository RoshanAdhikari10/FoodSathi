using System.ComponentModel.DataAnnotations;

namespace FoodSathi.Models
{
    public class MenuItem
    {
        [Key]   // ✅ Marks ItemID as the primary key
        public int ItemID { get; set; }

        [Required]
        public string ItemName { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string ImageURL { get; set; }

        public bool IsAvailable { get; set; } = true;

        // ----------------- New Fields -----------------

        // Category of the food (Pizza, Burger, Drinks, Desserts, etc.)
        public string Category { get; set; }

        // Dietary type (Vegetarian, Non-Vegetarian, Vegan, etc.)
        public string FoodType { get; set; }

        // Spicy level: Mild, Medium, Hot, etc.
        public string SpicyLevel { get; set; }
    }
}
