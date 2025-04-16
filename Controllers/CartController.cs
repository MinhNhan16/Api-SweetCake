using ASM_NhomSugar_SD19311.DTO;
using ASM_NhomSugar_SD19311.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Lấy giỏ hàng của người dùng theo AccountId
        [HttpGet("user/{accountId}")]
        public async Task<ActionResult<List<CartDto>>> GetCartsByAccountIdAsync(int accountId)
        {
            var carts = await _cartService.GetAllByAccountIdAsync(accountId);
            if (carts == null || carts.Count == 0)
            {
                return NotFound("Giỏ hàng không tồn tại.");
            }
            return Ok(carts);
        }

        // Lấy giỏ hàng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CartDto>> GetCartByIdAsync(int id)
        {
            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null)
            {
                return NotFound($"Giỏ hàng với ID {id} không tồn tại.");
            }
            return Ok(cart);
        }

        // Tạo giỏ hàng mới
        [HttpPost]
        public async Task<ActionResult> CreateCartAsync([FromBody] CartDto cartDto)
        {
            try
            {
                if (cartDto == null)
                {
                    return BadRequest("Dữ liệu giỏ hàng không hợp lệ.");
                }

                int newCartId = await _cartService.CreateAsync(cartDto);
                if (newCartId <= 0)
                {
                    return StatusCode(500, "Không thể lấy ID của giỏ hàng mới.");
                }

                cartDto.Id = newCartId; // Gán ID mới cho cartDto
                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo giỏ hàng: {ex.Message}");
            }
        }

        // Cập nhật giỏ hàng
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCartAsync(int id, [FromBody] CartDto cartDto)
        {
            if (cartDto == null || id != cartDto.Id)
            {
                return BadRequest("Dữ liệu giỏ hàng không hợp lệ.");
            }

            try
            {
                var result = await _cartService.UpdateAsync(cartDto);
                if (result)
                {
                    return NoContent();
                }

                return StatusCode(500, "Đã có lỗi xảy ra khi cập nhật giỏ hàng.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }


        // Xóa giỏ hàng
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCartAsync(int id)
        {
            var result = await _cartService.DeleteAsync(id);
            if (result)
            {
                return NoContent(); // Xóa thành công
            }

            return NotFound($"Giỏ hàng với ID {id} không tồn tại.");
        }
    }
}
