using System.ComponentModel.DataAnnotations;

namespace FoodSathi.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
    }
}
