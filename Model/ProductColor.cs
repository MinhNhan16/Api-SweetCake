using System.ComponentModel.DataAnnotations;

namespace ASM_NhomSugar_SD19311.Model
{
    public class ProductColor
    {


        [Key]
        public int Id { get; set; } // Mã màu (PK)

        [Required]
        public string Color { get; set; } // Tên màu
    }
}
