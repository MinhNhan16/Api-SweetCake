using System.ComponentModel.DataAnnotations;

namespace ASM_NhomSugar_SD19311.Model
{
    public class Accounts
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^(Admin|Customer|Shipper)$")]
        public string Role { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(20)]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        // Navigation properties
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Orders> CustomerOrders { get; set; }
        public virtual ICollection<Orders> ShipperOrders { get; set; }
        public virtual ICollection<Chats> SentChats { get; set; }
        public virtual ICollection<Chats> ReceivedChats { get; set; }
    }
}
