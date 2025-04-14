using ASM_NhomSugar_SD19311.Model;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Data
{
    public class CakeShopDbContext : DbContext
    {
        public CakeShopDbContext(DbContextOptions<CakeShopDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductQuantities> ProductQuantities { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Account relationships
            modelBuilder.Entity<Account>()
                .HasMany(a => a.CartItems)
                .WithOne(ci => ci.Account)
                .HasForeignKey(ci => ci.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Carts)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Orders)
                .WithOne(o => o.Account)
                .HasForeignKey(o => o.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Addresses)
                .WithOne(ad => ad.Account)
                .HasForeignKey(ad => ad.AccountId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Notifications)
                .WithOne(n => n.Account)
                .HasForeignKey(n => n.AccountId);

            // Address relationships
            modelBuilder.Entity<Address>()
                .HasMany(ad => ad.Carts)
                .WithOne(c => c.Address)
                .HasForeignKey(c => c.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasMany(ad => ad.Orders)
                .WithOne(o => o.Address)
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart relationships
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartDetails)
                .WithOne(cd => cd.Cart)
                .HasForeignKey(cd => cd.CartId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.ProductSize)
                .WithMany()
                .HasForeignKey(c => c.SizeId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            // CartDetail relationships (Composite Key)
            modelBuilder.Entity<CartDetail>()
                .HasKey(cd => new { cd.CartId, cd.ProductId });

            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.Product)
                .WithMany(p => p.CartDetails)
                .HasForeignKey(cd => cd.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

            // Category relationships
            modelBuilder.Entity<Categorie>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);

            // Discount relationships
            modelBuilder.Entity<Discount>()
                .HasMany(d => d.Orders)
                .WithMany();

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            // OrderDetails relationships
            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId);

            // Product relationships
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductQuantities)
                .WithOne(pq => pq.Product)
                .HasForeignKey(pq => pq.ProductId);

            // ProductQuantities relationships
            modelBuilder.Entity<ProductQuantities>()
                .HasOne(pq => pq.Size)
                .WithMany()
                .HasForeignKey(pq => pq.SizeId);

            modelBuilder.Entity<ProductQuantities>()
                .HasOne(pq => pq.Color)
                .WithMany()
                .HasForeignKey(pq => pq.ColorId);

            // Configure decimal precision
            modelBuilder.Entity<Cart>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Cart>()
                .Property(c => c.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CartDetail>()
                .Property(cd => cd.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetails>()
                .Property(od => od.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Discount>()
                .Property(d => d.DiscountValue)
                .HasPrecision(5, 2);
        }
    }
}