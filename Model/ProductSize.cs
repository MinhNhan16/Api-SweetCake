using System.ComponentModel.DataAnnotations;

namespace ASM_NhomSugar_SD19311.Model
{
    public class ProductSize
    {
        [Key]
        public int Id { get; set; } // Mã kích cỡ (PK)

        [Required]
        public int Size { get; set; } // Tên kích cỡ (ví dụ: S, M, L)
    }
}
