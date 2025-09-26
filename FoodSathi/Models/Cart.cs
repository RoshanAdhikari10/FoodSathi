using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSathi.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        public int ItemID { get; set; }

        [ForeignKey("ItemID")]
        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }

        // Optional if you want per-user cart
        public string? UserId { get; set; }
    }
}
