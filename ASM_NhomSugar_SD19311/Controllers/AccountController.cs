using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CakeShopContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(CakeShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Đăng ký (Register)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Accounts.AnyAsync(a => a.Username == request.Username))
            {
                return BadRequest("Username đã tồn tại.");
            }

            if (await _context.Accounts.AnyAsync(a => a.Email == request.Email))
            {
                return BadRequest("Email đã tồn tại.");
            }

            try
            {
                // Mã hóa mật khẩu bằng BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Gọi stored procedure AddAccountAndUser với mật khẩu đã mã hóa
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AddAccountAndUser @Username, @Password, @Role, @Email, @FullName, @Phone, @Address",
                    new SqlParameter("@Username", request.Username),
                    new SqlParameter("@Password", hashedPassword), // Lưu mật khẩu đã mã hóa
                    new SqlParameter("@Role", request.Role ?? "Customer"),
                    new SqlParameter("@Email", request.Email),
                    new SqlParameter("@FullName", request.FullName ?? (object)DBNull.Value),
                    new SqlParameter("@Phone", request.Phone ?? (object)DBNull.Value),
                    new SqlParameter("@Address", request.Address ?? (object)DBNull.Value)
                );

                return Ok(new { message = "Đăng ký thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi đăng ký: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == login.Email); // Đăng nhập bằng Email

            // Kiểm tra tài khoản và mật khẩu
            if (account == null || !BCrypt.Net.BCrypt.Verify(login.Password, account.Password))
            {
                return Unauthorized("Email hoặc mật khẩu không đúng.");
            }

            // Tạo JWT token
            var token = GenerateJwtToken(account);

            return Ok(new
            {
                message = "Đăng nhập thành công!",
                token = token,
                user = new
                {
                    account.Id,
                    account.Username,
                    account.Email,
                    account.Role
                }
            });
        }


        // Hàm tạo JWT token (giữ nguyên)
        private string GenerateJwtToken(Accounts account)
        {
            var key = Encoding.UTF8.GetBytes("ThisIsAReallyStrongSecretKeyForJWT123!nhanptmps40527@gmail.com");
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role)
            };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}