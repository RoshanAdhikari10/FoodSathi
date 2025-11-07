using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace FoodSathi.Models
{
    public class MenuItem
    {
        [Key]
        public int ItemID { get; set; }

        [Required, StringLength(100)]
        public string ItemName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 10000)]
        public decimal Price { get; set; }

        public string? ImagePath { get; set; }

        // ✅ Correct foreign key declaration
        [Required]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category? Category { get; set; }

        public bool IsAvailable { get; set; } = true;

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
