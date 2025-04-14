using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Address
    {
        [Key]
        public int Id { get; set; } // Mã địa chỉ (PK)

        [StringLength(200)]
        public string Street { get; set; } // Đường phố

        [StringLength(100)]
        public string City { get; set; } // Thành phố

        [StringLength(100)]
        public string State { get; set; } // Tỉnh

        [StringLength(100)]
        public string Country { get; set; } // Quốc gia

        [Required]
        public int AccountId { get; set; } // Mã người dùng (FK)
        [ForeignKey("AccountId")]
        public Account Account { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
