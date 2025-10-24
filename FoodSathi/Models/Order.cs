using System;
using System.ComponentModel.DataAnnotations;

namespace FoodSathi.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public int ItemID { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Required]
        public string Address { get; set; } // 🏠 Delivery address

        [Required]
        public string DeliveryOption { get; set; } // 🚚 e.g. “Home Delivery”, “Pickup”

        [Required]
        public string PaymentMethod { get; set; } // 💳 e.g. “Cash on Delivery”, “eSewa”, “Khalti”

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; } // 💰 Final total

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // 👇 Added — to know which logged-in user placed the order
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }


        // 👇 Optional — you can track order status if you want
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending";
    }
}
