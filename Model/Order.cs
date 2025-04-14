using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Order
    {
        [Key]
        [StringLength(150)]
        public string Id { get; set; } // Mã đơn hàng (PK)

        [Required]
        public DateTime OrderDate { get; set; } // Ngày đặt hàng

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal TotalPrice { get; set; } // Tổng giá

        [StringLength(450)]
        public string PaymentMode { get; set; } // Phương thức thanh toán

        [Required]
        public int OrderStatus { get; set; } // Trạng thái đơn hàng

        [Required]
        public int AccountId { get; set; } // Mã người dùng (FK)
        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        [Required]
        public int AddressId { get; set; } // Mã địa chỉ (FK)
        [ForeignKey("AddressId")]
        public Address Address { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
