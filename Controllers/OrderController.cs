using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly CakeShopDbContext _context;

        public OrderController(CakeShopDbContext context)
        {
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    PaymentMode = o.PaymentMode,
                    OrderStatus = o.OrderStatus,
                    AccountId = o.AccountId,
                    AddressId = o.AddressId
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(string id)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var dto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                PaymentMode = order.PaymentMode,
                OrderStatus = order.OrderStatus,
                AccountId = order.AccountId
            };

            return Ok(dto);
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Orders.AnyAsync(o => o.Id == dto.Id))
                return BadRequest("Order ID already exists.");

            if (!await _context.Accounts.AnyAsync(a => a.Id == dto.AccountId))
                return BadRequest("Invalid Account ID.");

            if (!await _context.Addresses.AnyAsync(a => a.Id == dto.AddressId))
                return BadRequest("Invalid Address ID.");

            var order = new Order
            {
                Id = dto.Id,
                OrderDate = dto.OrderDate,
                TotalPrice = dto.TotalPrice,
                PaymentMode = dto.PaymentMode,
                OrderStatus = dto.OrderStatus,
                AccountId = dto.AccountId,
                AddressId = dto.AddressId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, dto);
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, OrderDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Order ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            if (!await _context.Accounts.AnyAsync(a => a.Id == dto.AccountId))
                return BadRequest("Invalid Account ID.");

            if (!await _context.Addresses.AnyAsync(a => a.Id == dto.AddressId))
                return BadRequest("Invalid Address ID.");

            order.OrderDate = dto.OrderDate;
            order.TotalPrice = dto.TotalPrice;
            order.PaymentMode = dto.PaymentMode;
            order.OrderStatus = dto.OrderStatus;
            order.AccountId = dto.AccountId;
            order.AddressId = dto.AddressId;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                    return NotFound();

                throw;
            }
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(string id)
        {
            return _context.Orders.Any(o => o.Id == id);
        }
    }
}
