using ASM_NhomSugar_SD19311.Enums;
using ASM_NhomSugar_SD19311.Interface;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.AspNetCore.Mvc;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOrderController : Controller
    {
        private readonly IProductOrderService _service;

        public ProductOrderController(IProductOrderService service)
        {
            _service = service;
        }


        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            var products = await _service.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult<Products>> GetProduct(int id)
        {
            try
            {
                var product = await _service.GetProductAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("products")]
        public async Task<ActionResult<Products>> AddProduct(Products product)
        {
            var createdProduct = await _service.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Products product)
        {
            try
            {
                await _service.UpdateProductAsync(id, product);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _service.DeleteProductAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 2. Manage Orders
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<Orders>>> GetOrders()
        {
            var orders = await _service.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("orders/{id}")]
        public async Task<ActionResult<Orders>> GetOrder(int id)
        {
            try
            {
                var order = await _service.GetOrderAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("orders")]
        public async Task<ActionResult<Orders>> AddOrder(Orders order)
        {
            var createdOrder = await _service.AddOrderAsync(order);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
        {
            try
            {
                await _service.UpdateOrderStatusAsync(id, status);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("orders/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _service.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 3. Manage Cart
        [HttpGet("cart/{customerId}")]
        public async Task<ActionResult<Cart>> GetCart(int customerId)
        {
            var cart = await _service.GetCartAsync(customerId);
            return Ok(cart);
        }

        [HttpPost("cart/{customerId}/add")]
        public async Task<IActionResult> AddToCart(int customerId, [FromBody] CartDetails cartDetail)
        {
            try
            {
                await _service.AddToCartAsync(customerId, cartDetail);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("cart/{customerId}/update")]
        public async Task<IActionResult> UpdateCart(int customerId, [FromBody] CartDetails cartDetail)
        {
            try
            {
                await _service.UpdateCartAsync(customerId, cartDetail);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("cart/{customerId}/remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int customerId, int productId)
        {
            try
            {
                await _service.RemoveFromCartAsync(customerId, productId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 4. Checkout Cart
        [HttpPost("cart/{customerId}/checkout")]
        public async Task<IActionResult> CheckoutCart(int customerId)
        {
            try
            {
                var (orderId, totalPrice) = await _service.CheckoutCartAsync(customerId);
                return Ok(new { OrderId = orderId, TotalPrice = totalPrice });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 5. Search and Filter Products
        [HttpGet("products/search")]
        public async Task<ActionResult<IEnumerable<Products>>> SearchAndFilterProducts(
            [FromQuery] string name = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? categoryId = null)
        {
            var products = await _service.SearchAndFilterProductsAsync(name, minPrice, maxPrice, categoryId);
            return Ok(products);
        }
    }
}
