using System;

namespace FoodSathi.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
