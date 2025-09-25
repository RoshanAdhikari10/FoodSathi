using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSathi.Models   // <-- Make sure this namespace matches your project
{
    public class Offer
    {
        public int OfferID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(5,2)")]   // e.g., 99.99% max
        public decimal DiscountPercentage { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
