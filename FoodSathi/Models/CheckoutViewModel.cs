using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodSathi.Models
{
    public class CheckoutViewModel
    {
        public bool FromCart { get; set; }
        public List<Cart> CartItems { get; set; } = new();
        public Order SingleOrder { get; set; }
        public decimal TotalAmount { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string DeliveryOption { get; set; }

        [Required]
        public string PaymentOption { get; set; }
    }

}
