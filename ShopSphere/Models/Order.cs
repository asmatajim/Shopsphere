using System.ComponentModel.DataAnnotations;

namespace ShopSphere.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetails>();
        }

        public int Id { get; set; }

        [Display(Name = "Order No")]
        public string OrderNo { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name must contain only letters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone No")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Phone must be 10-15 digits")]
        public string PhoneNo { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } 

        [Required]
        public string Address { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public List<OrderDetails> OrderDetails { get; set; }
    }
}