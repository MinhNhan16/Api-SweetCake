using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Chats
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        [ForeignKey("SenderId")]
        public virtual Accounts Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual Accounts Receiver { get; set; }
    }
}
