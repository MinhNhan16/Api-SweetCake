﻿using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly CakeShopDbContext _context;

        public CartController(CakeShopDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var customer = await _context.Accounts.FindAsync(request.CustomerId);
            if (customer == null)
            {
                return BadRequest("Customer not found.");
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return BadRequest("Product not found.");
            }

            // Gọi stored procedure AddToCart để thêm sản phẩm vào giỏ hàng
            await _context.Database.ExecuteSqlRawAsync("EXEC AddToCart @CustomerId = {0}, @ProductId = {1}, @Quantity = {2}",
                request.CustomerId, request.ProductId, request.Quantity);

            return Ok("Product added to cart successfully.");
        }
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCartByCustomerId(int customerId)
        {
            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return NotFound("Không tìm thấy giỏ hàng.");

            var result = new
            {
                cart.Id,
                cart.CustomerId,
                cart.CreatedDate,
                Items = cart.CartDetails.Select(cd => new
                {
                    cd.ProductId,
                    cd.Product.Name,
                    cd.Product.Price,
                    cd.Quantity,
                    Total = cd.Product.Price * cd.Quantity
                })
            };

            return Ok(result);
        }

    }
}
