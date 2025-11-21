using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Address { get; set; } 

        [Required]
        public string DeliveryOption { get; set; } 

        [Required]
        public string PaymentMethod { get; set; } 

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; } 

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        public bool FromCart { get; set; } = false;

        public string DeliveryStatus { get; set; } = "Order Placed";

        public string ItemImage { get; set; }

        [ForeignKey("ItemID")]
        public virtual MenuItem MenuItem { get; set; }
    }
}
