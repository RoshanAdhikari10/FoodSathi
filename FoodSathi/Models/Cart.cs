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

        public string UserName { get; set; }
        public string Address { get; internal set; }
        public string DeliveryOption { get; internal set; }
    }
}
