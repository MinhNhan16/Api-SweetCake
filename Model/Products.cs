using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Products
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [StringLength(200)]
        public string ImagePath { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Categories Category { get; set; }

        public virtual ICollection<CartDetails> CartDetails { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
