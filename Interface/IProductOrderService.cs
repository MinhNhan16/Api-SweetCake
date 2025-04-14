using ASM_NhomSugar_SD19311.Enums;
using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.Interface
{
    public interface IProductOrderService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task UpdateProductAsync(int id, Product product);
        Task DeleteProductAsync(int id);

        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> GetOrderAsync(int id);
        Task<Order> AddOrderAsync(Order order);
        Task UpdateOrderStatusAsync(int id, OrderStatus status);
        Task DeleteOrderAsync(int id);

        Task<Cart> GetCartAsync(int customerId);
        Task AddToCartAsync(int customerId, CartDetail cartDetail);
        Task UpdateCartAsync(int customerId, CartDetail cartDetail);
        Task RemoveFromCartAsync(int customerId, int productId);

        Task<(int OrderId, decimal TotalPrice)> CheckoutCartAsync(int customerId, int? discountId = null);
        Task<IEnumerable<Product>> SearchAndFilterProductsAsync(string name, decimal? minPrice, decimal? maxPrice, int? categoryId);
    }
}
