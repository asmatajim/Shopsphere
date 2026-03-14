using System.ComponentModel.DataAnnotations;

namespace ShopSphere.Areas.Admin.Models
{
    public class RoleUserVm
    {
        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string RoleId { get; set; } = string.Empty;
    }
}