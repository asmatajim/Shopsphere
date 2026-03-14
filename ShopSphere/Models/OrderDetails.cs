using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSphere.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Order")]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Products Product { get; set; }
    }
}