using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/productsize")]
    [ApiController]
    public class ProductSizeController : ControllerBase
    {
        private readonly CakeShopDbContext _context;

        public ProductSizeController(CakeShopDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductSize
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductSize>>> GetProductSizes()
        {
            return await _context.ProductSizes.ToListAsync();
        }

        // GET: api/ProductSize/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductSize>> GetProductSize(int id)
        {
            var productSize = await _context.ProductSizes.FindAsync(id);

            if (productSize == null)
            {
                return NotFound();
            }

            return productSize;
        }

        // POST: api/ProductSize
        [HttpPost]
        public async Task<ActionResult<ProductSize>> PostProductSize(ProductSize productSize)
        {
            _context.ProductSizes.Add(productSize);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductSize), new { id = productSize.Id }, productSize);
        }

        // PUT: api/ProductSize/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductSize(int id, ProductSize productSize)
        {
            if (id != productSize.Id)
            {
                return BadRequest();
            }

            _context.Entry(productSize).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductSizeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ProductSize/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductSize(int id)
        {
            var productSize = await _context.ProductSizes.FindAsync(id);
            if (productSize == null)
            {
                return NotFound();
            }

            _context.ProductSizes.Remove(productSize);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductSizeExists(int id)
        {
            return _context.ProductSizes.Any(e => e.Id == id);
        }
    }
}
