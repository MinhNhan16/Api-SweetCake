using ASM_NhomSugar_SD19311.Enums;
using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.Interface
{
    public interface IProductOrderService
    {
        Task<IEnumerable<Products>> GetProductsAsync();
        Task<Products> GetProductAsync(int id);
        Task<Products> AddProductAsync(Products product);
        Task UpdateProductAsync(int id, Products product);
        Task DeleteProductAsync(int id);

        Task<IEnumerable<Orders>> GetOrdersAsync();
        Task<Orders> GetOrderAsync(int id);
        Task<Orders> AddOrderAsync(Orders order);
        Task UpdateOrderStatusAsync(int id, OrderStatus status);
        Task DeleteOrderAsync(int id);

        Task<Cart> GetCartAsync(int customerId);
        Task AddToCartAsync(int customerId, CartDetails cartDetail);
        Task UpdateCartAsync(int customerId, CartDetails cartDetail);
        Task RemoveFromCartAsync(int customerId, int productId);

        Task<(int OrderId, decimal TotalPrice)> CheckoutCartAsync(int customerId, int? discountId = null);
        Task<IEnumerable<Products>> SearchAndFilterProductsAsync(string name, decimal? minPrice, decimal? maxPrice, int? categoryId);
    }
}
