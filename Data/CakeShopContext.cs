using ASM_NhomSugar_SD19311.Model;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Data
{
    public class CakeShopContext : DbContext
    {
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Discounts> Discounts { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Chats> Chats { get; set; }

        public CakeShopContext(DbContextOptions<CakeShopContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite keys
            modelBuilder.Entity<CartDetails>()
                .HasKey(cd => new { cd.CartId, cd.ProductId });

            modelBuilder.Entity<OrderDetails>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            // Configure relationships for OrderDetails
            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails) // Define the one-to-many relationship
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Product)
                .WithMany() // Product does not need a navigation property back to OrderDetails
                .HasForeignKey(od => od.ProductId);

            // Configure the Role property
            modelBuilder.Entity<Accounts>()
                .Property(a => a.Role)
                .HasDefaultValue("Customer")
                .IsRequired()
                .HasMaxLength(20);

            // Configure unique constraints
            modelBuilder.Entity<Accounts>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Accounts>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Discounts>()
                .HasIndex(d => d.Code)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.AccountId)
                .IsUnique();

            modelBuilder.Entity<Orders>()
                .Property(o => o.OrderStatus)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
