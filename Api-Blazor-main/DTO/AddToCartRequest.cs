using System.ComponentModel.DataAnnotations;

namespace ASM_NhomSugar_SD19311.DTO
{
    public class AddToCartRequest
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
