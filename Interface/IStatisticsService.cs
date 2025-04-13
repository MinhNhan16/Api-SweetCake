using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.Interface
{
    public interface IStatisticsService
    {
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalCustomersAsync();
        Task<int> GetTotalOrdersAsync();
        Task<Dictionary<string, decimal>> GetRevenueTrendAsync();
        Task<Dictionary<string, int>> GetOrderStatusDistributionAsync();
        Task<List<TopCustomer>> GetTopCustomersAsync(int topN);
        Task<List<TopProduct>> GetTopProductsAsync(int topN);
    }
}
