using ASM_NhomSugar_SD19311.Data;
using Microsoft.AspNetCore.Mvc;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly CakeShopDbContext _context;

        public StatisticsController(CakeShopDbContext context)
        {
            _context = context;
        }


    }
}
