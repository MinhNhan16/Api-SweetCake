using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using ASM_NhomSugar_SD19311.Model;
using ASM_NhomSugar_SD19311.Service;
using Microsoft.AspNetCore.Authorization;
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
        private readonly CakeShopDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;

        public AccountController(CakeShopDbContext context, IConfiguration configuration, AuthService authService)
        {
            _context = context;
            _configuration = configuration;
            _authService = authService;
        }


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
                // Xác định role dựa trên email
                string role;
                if (request.Email.ToLower().Contains("admin"))
                {
                    role = "Admin";
                }
                else if (request.Email.ToLower().Contains("shipper"))
                {
                    role = "Shipper";
                }
                else
                {
                    role = "Customer";
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AddAccount @Username, @Password, @Role, @Email, @FullName, @Phone, @Address",
                    new SqlParameter("@Username", request.Username),
                    new SqlParameter("@Password", hashedPassword),
                    new SqlParameter("@Role", role), // Sử dụng role đã xác định
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
                .FirstOrDefaultAsync(a => a.Email == login.Email);
            if (account == null || !BCrypt.Net.BCrypt.Verify(login.Password, account.Password))
            {
                return Unauthorized("Email hoặc mật khẩu không đúng.");
            }
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
                    account.Role // Đảm bảo trả về role
                }
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _authService.LogoutAsync();
                return Ok(new { message = "Đăng xuất thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Lỗi khi đăng xuất: {ex.Message}" });
            }
        }




        [HttpGet("check-role")]
        [Authorize]
        public IActionResult CheckRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
            {
                return Unauthorized(new { message = "Không xác thực được người dùng." });
            }

            Console.WriteLine($"Role from token: {role}"); // Debug để kiểm tra role

            if (role == "Admin")
            {
                return Ok(new { allowed = true, redirect = "/layout" });
            }

            // Từ chối các role khác (Shipper, Customer)
            return StatusCode(403, new { allowed = false, redirect = "/access-denied", message = "Bạn không có quyền truy cập trang này." });
        }


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


        [HttpGet]
        public async Task<List<Accounts>> GetAllAccountsAsync()
        {
            // Truy vấn trực tiếp từ cơ sở dữ liệu hoặc API
            var accounts = await _context.Accounts.ToListAsync(); // Sử dụng Entity Framework hoặc cách lấy dữ liệu tương ứng

            return accounts; // Trả về danh sách Account
        }



        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Accounts.AnyAsync(a => a.Username == request.Username || a.Email == request.Email))
                return BadRequest("Username hoặc Email đã tồn tại.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var account = new Accounts
            {
                Username = request.Username,
                Password = hashedPassword,
                Email = request.Email,
                FullName = request.FullName,
                Phone = request.Phone,
                Address = request.Address,
                Role = request.Role ?? "Customer"
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm tài khoản thành công!" });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] RegisterRequest request)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound("Không tìm thấy tài khoản.");

            account.FullName = request.FullName;
            account.Phone = request.Phone;
            account.Address = request.Address;
            account.Role = request.Role;

            // Nếu có mật khẩu mới
            if (!string.IsNullOrEmpty(request.Password))
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật tài khoản thành công!" });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound("Không tìm thấy tài khoản.");

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xoá tài khoản thành công!" });
        }

    }
}