// Services/StatisticsService.cs
using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.Interface;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Service // Fixed namespace to match previous context
{
    public class StatisticsService : IStatisticsService
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
                .Where(a => a.Role != null && a.Role.ToLower() == "customer")
                .CountAsync();
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<Dictionary<string, decimal>> GetRevenueTrendAsync()
        {
            var result = await _context.Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    MonthYear = $"{g.Key.Month:D2}-{g.Key.Year}", // định dạng MM-yyyy
                    Total = g.Sum(o => o.TotalPrice)
                })
                .ToDictionaryAsync(g => g.MonthYear, g => g.Total);

            return result ?? new Dictionary<string, decimal>();
        }



        public async Task<Dictionary<string, int>> GetOrderStatusDistributionAsync()
        {
            var result = await _context.Orders
                .GroupBy(o => o.OrderStatus)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToDictionaryAsync(g => g.Status, g => g.Count);

            return result ?? new Dictionary<string, int>();
        }

        public async Task<List<TopCustomer>> GetTopCustomersAsync(int topN)
        {
            var result = await _context.Orders
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.Account != null)
                .SelectMany(o => o.OrderDetails, (o, od) => new
                {
                    o.Account.Username,
                    ProductName = od.Product != null ? od.Product.Name : "Unknown",
                    TotalSpent = od.Price * od.Quantity
                })
                .GroupBy(x => new { x.Username, x.ProductName })
                .Select(g => new TopCustomer
                {
                    Username = g.Key.Username,
                    ProductName = g.Key.ProductName,
                    TotalSpent = g.Sum(x => x.TotalSpent)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(topN)
                .ToListAsync();

            return result;
        }

        public async Task<List<TopProduct>> GetTopProductsAsync(int topN)
        {
            var result = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.Product != null)
                .GroupBy(od => od.Product.Name)
                .Select(g => new TopProduct
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(topN)
                .ToListAsync();

            return result;
        }
    }
}