using System.ComponentModel.DataAnnotations;

namespace FoodSathi.Models
{
    public class MenuItem
    {
        [Key]   // ✅ Marks ItemID as the primary key
        public int ItemID { get; set; }

        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
