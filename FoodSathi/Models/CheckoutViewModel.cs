using System.Collections.Generic;

namespace FoodSathi.Models
{
    public class CheckoutViewModel
    {
        public bool FromCart { get; set; }                  // true if from cart
        public List<Cart>? CartItems { get; set; }          // for multiple items
        public Order? SingleOrder { get; set; }             // for single order
        public decimal TotalAmount { get; set; }            // total price
    }
}
