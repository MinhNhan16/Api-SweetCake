using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class ProductQuantities
    {


        [Key]
        public int Id { get; set; } // Mã thực thể (PK)

        [Required]
        public int ProductId { get; set; } // Mã sản phẩm (FK)
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int SizeId { get; set; } // Mã kích cỡ (FK)
        [ForeignKey("SizeId")]
        public ProductSize Size { get; set; }

        [Required]
        public int ColorId { get; set; } // Mã màu (FK)
        [ForeignKey("ColorId")]
        public ProductColor Color { get; set; }
    }
}
