using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Accounts Customer { get; set; }

        public virtual ICollection<CartDetails> CartDetails { get; set; }
    }
}
