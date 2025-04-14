using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Cart
    {
        [Key]
        public int Id { get; set; } // Mã giỏ hàng (PK)

        [Required]
        public int Quantity { get; set; } // Số lượng

        [Required]
        public int Size { get; set; } // Kích cỡ

        [Required]
        public int CheckoutCount { get; set; } // Số lượng bán

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; } // Giá

        [StringLength(450)]
        public string PaymentMode { get; set; } // Phương thức thanh toán

        [Required]
        public DateTime DateCreated { get; set; } // Ngày tạo

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal TotalPrice { get; set; } // Tổng giá

        [Required]
        public float PayPalPayment { get; set; } // Tổng tiền thanh toán PayPal

        [Required]
        public int AccountId { get; set; } // Mã người dùng (FK)
        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        [Required]
        public int AddressId { get; set; } // Mã địa chỉ (FK)
        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        [Required]
        public int SizeId { get; set; } // Mã kích cỡ (FK)
        [ForeignKey("SizeId")]
        public ProductSize ProductSize { get; set; }

        [Required]
        public int ProductId { get; set; } // Mã sản phẩm (FK)
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
