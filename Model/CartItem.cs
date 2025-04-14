using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; } // Mã item giỏ hàng (PK)

        [Required]
        public int AccountId { get; set; } // Mã người dùng (FK)
        [ForeignKey("AccountId")]
        public Account Account { get; set; }
    }
}
