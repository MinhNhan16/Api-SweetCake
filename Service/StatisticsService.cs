// Services/StatisticsService.cs
using ASM_NhomSugar_SD19311.Data;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Service // Fixed namespace to match previous context
{
    public class StatisticsService
    {
        private readonly CakeShopDbContext _context;

        public StatisticsService(CakeShopDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders.SumAsync(o => o.TotalPrice);
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            return await _context.Accounts
             .Where(u => u.Role.ToLower() == "customer")
                .CountAsync();
        }


        public async Task<int> GetTotalOrdersAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<Dictionary<string, decimal>> GetRevenueTrendAsync()
        {
            var orders = await _context.Orders
                .GroupBy(o => o.OrderDate.ToString("MM-yy"))
                .Select(g => new { Month = g.Key, Total = g.Sum(o => o.TotalPrice) })
                .ToDictionaryAsync(g => g.Month, g => g.Total);

            return orders;
        }

        public async Task<Dictionary<string, int>> GetOrderStatusDistributionAsync()
        {
            return await _context.Orders
                .GroupBy(o => o.OrderStatus)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(g => g.Status, g => g.Count);
        }

        public async Task<List<(string Username, string ProductName, decimal TotalSpent)>> GetTopCustomersAsync(int topN = 5)
        {
            var result = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .SelectMany(o => o.OrderDetails, (o, od) => new
                {
                    o.Customer.Username,
                    ProductName = od.Product.Name,
                    TotalSpent = od.Price * od.Quantity
                })
                .GroupBy(x => new { x.Username, x.ProductName })
                .Select(g => new
                {
                    g.Key.Username,
                    g.Key.ProductName,
                    TotalSpent = g.Sum(x => x.TotalSpent)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(topN)
                .ToListAsync();

            return result
                .Select(x => (x.Username, x.ProductName, x.TotalSpent))
                .ToList();
        }


        public async Task<List<(string ProductName, int QuantitySold)>> GetTopProductsAsync(int topN = 5)
        {
            // Project into an anonymous type first to avoid tuple literals in the expression tree
            var queryResult = await _context.OrderDetails
                .GroupBy(od => od.Product.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.QuantitySold)
                .Take(topN)
                .ToListAsync();

            // Convert the anonymous type to a tuple after materializing the query
            var result = queryResult.Select(g => (g.ProductName, g.QuantitySold)).ToList();

            return result;
        }
    }
}