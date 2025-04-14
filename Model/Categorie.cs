using System.ComponentModel.DataAnnotations;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
