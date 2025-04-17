using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using ASM_NhomSugar_SD19311.Enums;
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

            // Kiểm tra PaymentMode hợp lệ
            if (dto.PaymentMode != "VNPAY" && dto.PaymentMode != "MOMO")
            {
                return BadRequest("Invalid PaymentMode. Allowed values are VNPAY or COD.");
            }

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

        [HttpGet("latest/{accountId}")]
        public async Task<ActionResult<OrderDto>> GetLatestOrder(int accountId)
        {
            var latestOrder = await _context.Orders
                .Where(o => o.AccountId == accountId)
                .OrderByDescending(o => o.OrderDate)
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
                .FirstOrDefaultAsync();

            if (latestOrder == null)
                return NotFound();

            return Ok(latestOrder);
        }

        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<OrderDto>> GetOrderByAccountId(int accountId)
        {
            var order = await _context.Orders
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .Where(o => o.AccountId == accountId)
                .OrderByDescending(o => o.OrderDate) // Hoặc OrderByDescending nếu cần mới nhất
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    PaymentMode = o.PaymentMode,
                    OrderStatus = o.OrderStatus,
                    AccountId = o.AccountId,
                    AddressId = o.AddressId,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetails
                    {
                        Id = od.Id,
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        Size = od.Size,
                        Price = od.Price,
                        TotalPrice = od.TotalPrice,
                        Product = od.Product
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] OrderCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _context.Accounts.AnyAsync(a => a.Id == request.AccountId))
                return BadRequest("Tài khoản không tồn tại.");

            if (!await _context.Addresses.AnyAsync(a => a.Id == request.AddressId))
                return BadRequest("Địa chỉ không tồn tại.");

            if (request.PaymentMode != "VNPAY" && request.PaymentMode != "M0M0" && request.PaymentMode != "Thanh toán bằng tiền mặt")
                return BadRequest("Phương thức thanh toán không hợp lệ.");

            if (request.Carts == null || !request.Carts.Any())
                return BadRequest("Giỏ hàng trống.");

            // Tạo đơn hàng
            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                OrderDate = DateTime.UtcNow,
                PaymentMode = request.PaymentMode,
                OrderStatus = (int)OrderStatus.Placed,
                AccountId = request.AccountId,
                AddressId = request.AddressId,
                TotalPrice = request.Carts.Sum(c => c.TotalPrice)
            };

            _context.Orders.Add(order);

            foreach (var cart in request.Carts)
            {
                var size = await _context.ProductSizes.FindAsync(cart.SizeId);
                if (size == null)
                    return BadRequest($"Kích cỡ không tồn tại: {cart.SizeId}");

                var orderDetail = new OrderDetails
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = order.Id,
                    ProductId = cart.ProductId,
                    Quantity = cart.Quantity,
                    Size = cart.Size,
                    Price = cart.Price,
                    TotalPrice = (float)cart.TotalPrice
                };

                _context.OrderDetails.Add(orderDetail);
            }
            var existingCarts = await _context.Carts
                .Where(c => request.Carts.Select(x => x.Id).Contains(c.Id))
                .ToListAsync();

            foreach (var cart in existingCarts)
            {
                cart.IsCheckout = true;
                cart.CheckoutCount += cart.Quantity;
                cart.Quantity = 0;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo đơn hàng thành công", orderId = order.Id });
        }


        private bool OrderExists(string id)
        {
            return _context.Orders.Any(o => o.Id == id);
        }
    }
}
