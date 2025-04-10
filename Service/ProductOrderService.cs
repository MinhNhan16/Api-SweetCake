using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.Enums;
using ASM_NhomSugar_SD19311.Interface;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Service
{
    public class ProductOrderService : IProductOrderService
    {
        private readonly CakeShopContext _context;

        public ProductOrderService(CakeShopContext context)
        {
            _context = context;
        }

        // Validate Role value
        private void ValidateRole(string role)
        {
            var validRoles = new[] { "Admin", "Customer", "Shipper" };
            if (!validRoles.Contains(role))
            {
                throw new ArgumentException($"Invalid Role value. Role must be one of: {string.Join(", ", validRoles)}.");
            }
        }

        // Manage Products
        public async Task<IEnumerable<Products>> GetProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Products> GetProductAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            return product;
        }

        public async Task<Products> AddProductAsync(Products product)
        {
            if (product.Price < 0 || product.Stock < 0)
            {
                throw new ArgumentException("Price and Stock must be non-negative.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductAsync(int id, Products product)
        {
            if (id != product.Id)
            {
                throw new ArgumentException("Product ID mismatch.");
            }

            if (product.Price < 0 || product.Stock < 0)
            {
                throw new ArgumentException("Price and Stock must be non-negative.");
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // Manage Orders
        public async Task<IEnumerable<Orders>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .Include(o => o.Discount)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
        }

        public async Task<Orders> GetOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .Include(o => o.Discount)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            return order;
        }

        public async Task<Orders> AddOrderAsync(Orders order)
        {
            // Validate CustomerId
            var customer = await _context.Accounts.FindAsync(order.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {order.CustomerId} not found.");
            }

            // Validate ShipperId if provided
            if (order.ShipperId.HasValue)
            {
                var shipper = await _context.Accounts.FindAsync(order.ShipperId.Value);
                if (shipper == null)
                {
                    throw new ArgumentException($"Shipper with ID {order.ShipperId} not found.");
                }
                if (shipper.Role != "Shipper")
                {
                    throw new ArgumentException($"Account with ID {order.ShipperId} is not a Shipper.");
                }
            }

            // Validate DiscountId if provided
            if (order.DiscountId.HasValue)
            {
                var discount = await _context.Discounts.FindAsync(order.DiscountId.Value);
                if (discount == null)
                {
                    throw new ArgumentException($"Discount with ID {order.DiscountId} not found.");
                }
                if (discount.StartDate > DateTime.UtcNow || discount.EndDate < DateTime.UtcNow)
                {
                    throw new ArgumentException($"Discount with ID {order.DiscountId} is not valid at this time.");
                }
            }

            order.OrderDate = DateTime.UtcNow;
            order.OrderStatus = OrderStatus.Placed;

            // Validate stock for each order detail
            foreach (var detail in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {detail.ProductId} not found.");
                }
                if (product.Stock < detail.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {detail.Quantity}.");
                }
                product.Stock -= detail.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            order.OrderStatus = status;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        // Manage Cart
        public async Task<Cart> GetCartAsync(int customerId)
        {
            var customer = await _context.Accounts.FindAsync(customerId);
            if (customer == null || customer.Role != "Customer")
            {
                throw new ArgumentException($"Customer with ID {customerId} not found or is not a Customer.");
            }

            var cart = await _context.Cart
                .Include(c => c.Customer)
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedDate = DateTime.UtcNow
                };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task AddToCartAsync(int customerId, CartDetails cartDetail)
        {
            var customer = await _context.Accounts.FindAsync(customerId);
            if (customer == null || customer.Role != "Customer")
            {
                throw new ArgumentException($"Customer with ID {customerId} not found or is not a Customer.");
            }

            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedDate = DateTime.UtcNow
                };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products.FindAsync(cartDetail.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {cartDetail.ProductId} not found.");
            }

            if (product.Stock < cartDetail.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {cartDetail.Quantity}.");
            }

            var existingDetail = cart.CartDetails?.FirstOrDefault(cd => cd.ProductId == cartDetail.ProductId);
            if (existingDetail != null)
            {
                existingDetail.Quantity += cartDetail.Quantity;
                if (product.Stock < existingDetail.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {existingDetail.Quantity}.");
                }
                existingDetail.Price = product.Price;
            }
            else
            {
                cartDetail.CartId = cart.Id;
                cartDetail.Price = product.Price;
                _context.CartDetails.Add(cartDetail);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(int customerId, CartDetails cartDetail)
        {
            var customer = await _context.Accounts.FindAsync(customerId);
            if (customer == null || customer.Role != "Customer")
            {
                throw new ArgumentException($"Customer with ID {customerId} not found or is not a Customer.");
            }

            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                throw new KeyNotFoundException($"Cart for customer ID {customerId} not found.");
            }

            var existingDetail = cart.CartDetails?.FirstOrDefault(cd => cd.ProductId == cartDetail.ProductId);
            if (existingDetail == null)
            {
                throw new KeyNotFoundException($"Product with ID {cartDetail.ProductId} not found in cart.");
            }

            var product = await _context.Products.FindAsync(cartDetail.ProductId);
            if (product.Stock < cartDetail.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {cartDetail.Quantity}.");
            }

            existingDetail.Quantity = cartDetail.Quantity;
            existingDetail.Price = product.Price;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int customerId, int productId)
        {
            var customer = await _context.Accounts.FindAsync(customerId);
            if (customer == null || customer.Role != "Customer")
            {
                throw new ArgumentException($"Customer with ID {customerId} not found or is not a Customer.");
            }

            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                throw new KeyNotFoundException($"Cart for customer ID {customerId} not found.");
            }

            var cartDetail = cart.CartDetails?.FirstOrDefault(cd => cd.ProductId == productId);
            if (cartDetail == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found in cart.");
            }

            _context.CartDetails.Remove(cartDetail);
            await _context.SaveChangesAsync();
        }

        // Checkout Cart
        public async Task<(int OrderId, decimal TotalPrice)> CheckoutCartAsync(int customerId, int? discountId = null)
        {
            var customer = await _context.Accounts.FindAsync(customerId);
            if (customer == null || customer.Role != "Customer")
            {
                throw new ArgumentException($"Customer with ID {customerId} not found or is not a Customer.");
            }

            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null || !cart.CartDetails.Any())
            {
                throw new InvalidOperationException("Cart is empty or not found.");
            }

            // Validate stock for all items in the cart
            foreach (var detail in cart.CartDetails)
            {
                var product = detail.Product;
                if (product.Stock < detail.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {detail.Quantity}.");
                }
            }

            // Calculate total price
            decimal totalPrice = cart.CartDetails.Sum(cd => cd.Price * cd.Quantity);

            // Apply discount if provided
            Discounts discount = null;
            if (discountId.HasValue)
            {
                discount = await _context.Discounts.FindAsync(discountId.Value);
                if (discount == null)
                {
                    throw new ArgumentException($"Discount with ID {discountId} not found.");
                }
                if (discount.StartDate > DateTime.UtcNow || discount.EndDate < DateTime.UtcNow)
                {
                    throw new ArgumentException($"Discount with ID {discountId} is not valid at this time.");
                }
                totalPrice *= (1 - discount.DiscountValue);
            }

            // Create order
            var order = new Orders
            {
                OrderDate = DateTime.UtcNow,
                TotalPrice = totalPrice,
                OrderStatus = OrderStatus.Placed,
                CustomerId = customerId,
                DiscountId = discountId,
                OrderDetails = cart.CartDetails.Select(cd => new OrderDetails
                {
                    ProductId = cd.ProductId,
                    Quantity = cd.Quantity,
                    Price = cd.Price
                }).ToList()
            };

            // Update stock
            foreach (var detail in cart.CartDetails)
            {
                var product = detail.Product;
                product.Stock -= detail.Quantity;
            }

            _context.Orders.Add(order);
            _context.CartDetails.RemoveRange(cart.CartDetails);
            await _context.SaveChangesAsync();

            return (order.Id, totalPrice);
        }

        // Search and Filter Products
        public async Task<IEnumerable<Products>> SearchAndFilterProductsAsync(string name, decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
