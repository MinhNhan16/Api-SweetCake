using ASM_NhomSugar_SD19311.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? ShipperId { get; set; }

        public int? DiscountId { get; set; }

        [ForeignKey("CustomerId")]
        public Accounts Customer { get; set; }

        [ForeignKey("ShipperId")]
        public Accounts Shipper { get; set; }

        [ForeignKey("DiscountId")]
        public Discounts Discount { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } // Add this property
    }
}
