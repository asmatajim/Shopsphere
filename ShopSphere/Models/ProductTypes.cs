using System.ComponentModel.DataAnnotations;

namespace ShopSphere.Models
{
    public class ProductTypes
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Product Type")]
        [Display(Name = "Product Type")]
        public string ProductType { get; set; } = string.Empty;
    }
}