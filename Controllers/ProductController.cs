using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CakeShopDbContext _context;

        public ProductController(CakeShopDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả sản phẩm
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImagePath = p.ImagePath,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            return Ok(products);
        }

        // Lấy sản phẩm theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return NotFound();

            return Ok(new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImagePath = p.ImagePath,
                CategoryId = p.CategoryId
            });
        }

        // Tạo sản phẩm mới
        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] ProductDto dto)
        {
            var p = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImagePath = dto.ImagePath,
                CategoryId = dto.CategoryId
            };

            _context.Products.Add(p);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = p.Id }, dto);
        }

        // Cập nhật sản phẩm
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductDto dto)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return NotFound();

            p.Name = dto.Name;
            p.Description = dto.Description;
            p.Price = dto.Price;
            p.Stock = dto.Stock;
            p.ImagePath = dto.ImagePath;
            p.CategoryId = dto.CategoryId;

            _context.Products.Update(p);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Xóa sản phẩm
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return NotFound();

            _context.Products.Remove(p);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
