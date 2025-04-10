using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly CakeShopContext _context;

        public HomeController(CakeShopContext context)
        {
            _context = context;
        }

        [HttpGet] // Định nghĩa đây là API GET
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImagePath = p.ImagePath
                })
                .ToListAsync();

            return Ok(products); // Trả về dữ liệu dạng JSON
        }

    }
}
