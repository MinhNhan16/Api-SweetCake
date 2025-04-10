using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class OrderDetails
    {
        [Key, Column(Order = 0)]
        public int OrderId { get; set; }

        [Key, Column(Order = 1)]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [ForeignKey("OrderId")]
        public Orders Order { get; set; }

        [ForeignKey("ProductId")]
        public Products Product { get; set; }
    }
}
