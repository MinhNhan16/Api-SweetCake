using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/productcolor")]
    [ApiController]
    public class ProductColorController : ControllerBase
    {
        private readonly CakeShopDbContext _context;

        public ProductColorController(CakeShopDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductColors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductColor>>> GetProductColors()
        {
            return await _context.ProductColors.ToListAsync();
        }

        // GET: api/ProductColors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductColor>> GetProductColor(int id)
        {
            var productColor = await _context.ProductColors.FindAsync(id);

            if (productColor == null)
            {
                return NotFound();
            }

            return productColor;
        }

        // POST: api/ProductColors
        [HttpPost]
        public async Task<ActionResult<ProductColor>> CreateProductColor(ProductColor productColor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductColors.Add(productColor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductColor), new { id = productColor.Id }, productColor);
        }

        // PUT: api/ProductColors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductColor(int id, ProductColor productColor)
        {
            if (id != productColor.Id)
            {
                return BadRequest();
            }

            _context.Entry(productColor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductColorExists(id))
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

        // DELETE: api/ProductColors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductColor(int id)
        {
            var productColor = await _context.ProductColors.FindAsync(id);
            if (productColor == null)
            {
                return NotFound();
            }

            _context.ProductColors.Remove(productColor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductColorExists(int id)
        {
            return _context.ProductColors.Any(e => e.Id == id);
        }
    }
}
