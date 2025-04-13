using ASM_NhomSugar_SD19311.Interface;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.AspNetCore.Mvc;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        // Tổng doanh thu
        [HttpGet("revenue")]
        public async Task<ActionResult<decimal>> GetTotalRevenue()
        {
            var revenue = await _statisticsService.GetTotalRevenueAsync();
            return Ok(revenue);
        }

        // Tổng số khách hàng
        [HttpGet("customers")]
        public async Task<ActionResult<int>> GetTotalCustomers()
        {
            var customers = await _statisticsService.GetTotalCustomersAsync();
            return Ok(customers);
        }

        // Tổng số đơn hàng
        [HttpGet("orders")]
        public async Task<ActionResult<int>> GetTotalOrders()
        {
            var orders = await _statisticsService.GetTotalOrdersAsync();
            return Ok(orders);
        }

        // Xu hướng doanh thu theo tháng
        [HttpGet("revenue-trend")]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetRevenueTrend()
        {
            var trend = await _statisticsService.GetRevenueTrendAsync();
            return Ok(trend);
        }

        // Phân phối trạng thái đơn hàng
        [HttpGet("order-status")]
        public async Task<ActionResult<Dictionary<string, int>>> GetOrderStatusDistribution()
        {
            var status = await _statisticsService.GetOrderStatusDistributionAsync();
            return Ok(status);
        }

        // Top khách hàng
        [HttpGet("top-customers/{topN}")]
        public async Task<ActionResult<List<TopCustomer>>> GetTopCustomers(int topN)
        {
            var topCustomers = await _statisticsService.GetTopCustomersAsync(topN);
            return Ok(topCustomers);
        }

        // Top sản phẩm
        [HttpGet("top-products/{topN}")]
        public async Task<ActionResult<List<TopProduct>>> GetTopProducts(int topN)
        {
            var topProducts = await _statisticsService.GetTopProductsAsync(topN);
            return Ok(topProducts);
        }


    }
}
