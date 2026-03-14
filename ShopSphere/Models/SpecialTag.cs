using System.ComponentModel.DataAnnotations;

namespace ShopSphere.Models
{
    public class SpecialTag
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter tag name properly.")]
        [Display(Name = "Tag Name")]
        public string Name { get; set; } = string.Empty;
    }
}