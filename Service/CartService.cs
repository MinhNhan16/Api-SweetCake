using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using ASM_NhomSugar_SD19311.Interface;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Service
{
    public class CartService : ICartService
    {
        private readonly CakeShopDbContext _context;

        public CartService(CakeShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartDto>> GetAllAsync()
        {
            return await _context.Carts
                .Select(c => new CartDto
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    Size = c.Size,
                    CheckoutCount = c.CheckoutCount,
                    Price = c.Price,
                    PaymentMode = c.PaymentMode,
                    DateCreated = c.DateCreated,
                    TotalPrice = c.TotalPrice,
                    PayPalPayment = c.PayPalPayment,
                    AccountId = c.AccountId,
                    AddressId = c.AddressId,
                    SizeId = c.SizeId,
                    ProductId = c.ProductId
                })
                .ToListAsync();
        }

        // Lấy giỏ hàng của người dùng theo AccountId
        public async Task<List<CartDto>> GetAllByAccountIdAsync(int accountId)
        {
            return await _context.Carts
                .Where(c => c.AccountId == accountId)
                .Select(c => new CartDto
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    Size = c.Size,
                    CheckoutCount = c.CheckoutCount,
                    Price = c.Price,
                    PaymentMode = c.PaymentMode,
                    DateCreated = c.DateCreated,
                    TotalPrice = c.TotalPrice,
                    PayPalPayment = c.PayPalPayment,
                    AccountId = c.AccountId,
                    AddressId = c.AddressId,
                    SizeId = c.SizeId,
                    ProductId = c.ProductId
                })
                .ToListAsync();
        }

        public async Task<CartDto?> GetByIdAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return null;

            return new CartDto
            {
                Id = cart.Id,
                Quantity = cart.Quantity,
                Size = cart.Size,
                CheckoutCount = cart.CheckoutCount,
                Price = cart.Price,
                PaymentMode = cart.PaymentMode,
                DateCreated = cart.DateCreated,
                TotalPrice = cart.TotalPrice,
                PayPalPayment = cart.PayPalPayment,
                AccountId = cart.AccountId,
                AddressId = cart.AddressId,
                SizeId = cart.SizeId,
                ProductId = cart.ProductId
            };
        }

        public async Task<int> CreateAsync(CartDto cartDto)
        {
            try
            {
                Console.WriteLine($"Creating cart with ProductId: {cartDto.ProductId}, AccountId: {cartDto.AccountId}, SizeId: {cartDto.SizeId}, AddressId: {cartDto.AddressId}");

                if (cartDto == null || cartDto.ProductId <= 0 || cartDto.AccountId <= 0 || cartDto.SizeId <= 0 || cartDto.AddressId <= 0)
                {
                    throw new ArgumentException("Dữ liệu giỏ hàng không hợp lệ.");
                }

                var productExists = await _context.Products.AnyAsync(p => p.Id == cartDto.ProductId);
                var accountExists = await _context.Accounts.AnyAsync(a => a.Id == cartDto.AccountId);
                var sizeExists = await _context.ProductSizes.AnyAsync(s => s.Id == cartDto.SizeId);
                var addressExists = await _context.Addresses.AnyAsync(a => a.Id == cartDto.AddressId);

                Console.WriteLine($"Product exists: {productExists}, Account exists: {accountExists}, Size exists: {sizeExists}, Address exists: {addressExists}");

                if (!productExists || !accountExists || !sizeExists || !addressExists)
                {
                    throw new ArgumentException("Một hoặc nhiều khóa ngoại không tồn tại (ProductId, AccountId, SizeId, AddressId).");
                }

                var existingCart = await _context.Carts
                    .FirstOrDefaultAsync(c =>
                        c.ProductId == cartDto.ProductId &&
                        c.AccountId == cartDto.AccountId &&
                        c.SizeId == cartDto.SizeId);

                int newCartId;

                if (existingCart != null)
                {
                    existingCart.Quantity += cartDto.Quantity;
                    existingCart.TotalPrice = existingCart.Quantity * existingCart.Price;
                    _context.Carts.Update(existingCart);
                    await _context.SaveChangesAsync();
                    newCartId = existingCart.Id;
                    Console.WriteLine($"Updated existing cart with ID: {newCartId}");
                }
                else
                {
                    var newCart = new Cart
                    {
                        Quantity = cartDto.Quantity,
                        Size = cartDto.Size,
                        CheckoutCount = cartDto.CheckoutCount,
                        Price = cartDto.Price,
                        PaymentMode = cartDto.PaymentMode,
                        DateCreated = DateTime.Now,
                        TotalPrice = cartDto.Quantity * cartDto.Price,
                        PayPalPayment = cartDto.PayPalPayment,
                        AccountId = cartDto.AccountId,
                        AddressId = cartDto.AddressId,
                        SizeId = cartDto.SizeId,
                        ProductId = cartDto.ProductId
                    };

                    _context.Carts.Add(newCart);
                    await _context.SaveChangesAsync();
                    newCartId = newCart.Id;
                    Console.WriteLine($"Created new cart with ID: {newCartId}");
                }

                return newCartId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating cart: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(CartDto cartDto)
        {
            var cart = await _context.Carts.FindAsync(cartDto.Id);
            if (cart == null) return false;

            cart.Quantity = cartDto.Quantity;
            cart.TotalPrice = cart.Quantity * cart.Price;

            _context.Carts.Update(cart);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return false;

            _context.Carts.Remove(cart);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
