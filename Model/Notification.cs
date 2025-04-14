using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public DateTime Created { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Message { get; set; }

        [StringLength(500)]
        public string DataUrl { get; set; }

        [StringLength(500)]
        public string Data { get; set; }

        public bool IsRead { get; set; }

        [Required]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }

    }
}
