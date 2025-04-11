using System.ComponentModel.DataAnnotations;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Categories
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
