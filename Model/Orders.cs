using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public int OrderStatus { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int? ShipperId { get; set; }

        public int? DiscountId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Accounts Customer { get; set; }

        [ForeignKey("ShipperId")]
        public virtual Accounts Shipper { get; set; }

        [ForeignKey("DiscountId")]
        public virtual Discounts Discount { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
