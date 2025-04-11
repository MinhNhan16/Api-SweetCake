using ASM_NhomSugar_SD19311.Model;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Data
{
    public class CakeShopDbContext : DbContext
    {
        public CakeShopDbContext(DbContextOptions<CakeShopDbContext> options) : base(options) { }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Discounts> Discounts { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Chats> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartDetails>()
                 .HasKey(cd => new { cd.CartId, cd.ProductId });

            modelBuilder.Entity<OrderDetails>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            // Định nghĩa ràng buộc UNIQUE
            modelBuilder.Entity<Accounts>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Accounts>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Discounts>()
                .HasIndex(d => d.Code)
                .IsUnique();

            // Cấu hình mối quan hệ giữa Accounts và Orders
            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Customer)
                .WithMany(a => a.CustomerOrders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Shipper)
                .WithMany(a => a.ShipperOrders)
                .HasForeignKey(o => o.ShipperId)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình mối quan hệ giữa Accounts và Chats
            // 1. Mối quan hệ 1-nhiều: Accounts (Sender) -> Chats
            modelBuilder.Entity<Chats>()
                .HasOne(c => c.Sender)              // Chats có một Sender
                .WithMany(a => a.SentChats)         // Accounts có nhiều SentChats
                .HasForeignKey(c => c.SenderId)     // Khóa ngoại là SenderId
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa nếu có tin nhắn liên quan

            // 2. Mối quan hệ 1-nhiều: Accounts (Receiver) -> Chats
            modelBuilder.Entity<Chats>()
                .HasOne(c => c.Receiver)            // Chats có một Receiver
                .WithMany(a => a.ReceivedChats)     // Accounts có nhiều ReceivedChats
                .HasForeignKey(c => c.ReceiverId)   // Khóa ngoại là ReceiverId
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa nếu có tin nhắn liên quan
        }
    }
}
