using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Service
{
    public class ShoppingCartService
    {
        private readonly CakeShopDbContext _context;

        public ShoppingCartService(CakeShopDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartAsync(int customerId)
        {
            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedDate = DateTime.Now,
                    CartDetails = new List<CartDetails>()
                };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task UpdateCartItemAsync(int customerId, int productId, int quantity)
        {
            var cart = await GetCartAsync(customerId);
            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);
            if (cartDetail != null)
            {
                cartDetail.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddToCartAsync(int customerId, int productId, int quantity = 1)
        {
            var cart = await GetCartAsync(customerId);
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);
            if (cartDetail != null)
            {
                cartDetail.Quantity += quantity;
            }
            else
            {
                cart.CartDetails.Add(new CartDetails
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int customerId, int productId)
        {
            var cart = await GetCartAsync(customerId);
            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);

            if (cartDetail != null)
            {
                cart.CartDetails.Remove(cartDetail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Accounts> GetAccountByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<int> GetCartItemCountAsync(int customerId)
        {
            var cart = await GetCartAsync(customerId);
            return cart.CartDetails.Sum(cd => cd.Quantity);
        }

        public async Task ClearCartAsync(int customerId)
        {
            var cart = await GetCartAsync(customerId);
            _context.CartDetails.RemoveRange(cart.CartDetails);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}
