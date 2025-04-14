using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class OrderDetails
    {
        [Key]
        [StringLength(150)]
        public string Id { get; set; } // Mã đơn hàng (PK)

        [Required]
        public int Quantity { get; set; } // Số lượng

        [Required]
        public int Size { get; set; } // Kích cỡ

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; } // Giá

        [Required]
        public float TotalPrice { get; set; } // Tổng giá

        [Required]
        [StringLength(150)]
        public string OrderId { get; set; } // Mã đơn hàng (FK từ Order)
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
