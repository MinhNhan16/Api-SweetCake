using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
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

        public AccountController(CakeShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest googleLoginRequest)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={googleLoginRequest.Token}");
            var content = await result.Content.ReadAsStringAsync();

            GoogleTokenInfo googleTokenInfo = JsonConvert.DeserializeObject<GoogleTokenInfo>(content);

            if (googleTokenInfo != null)
            {
                Account existingAccount = await _context.Accounts.Where(x => x.Email == googleTokenInfo.Email).FirstOrDefaultAsync();

                // chưa tồn tại thì tạo mới
                if (existingAccount == null)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                      "EXEC AddAccount @Username, @Password, @Role, @Email, @FullName, @Phone, @Address",
                      new SqlParameter("@Username", googleTokenInfo.Email),
                      new SqlParameter("@Password", ""),
                      new SqlParameter("@Role", "Customer"),
                      new SqlParameter("@Email", googleTokenInfo.Email),
                      new SqlParameter("@FullName", googleTokenInfo.Name),
                      new SqlParameter("@Phone", "0000000000"),
                      new SqlParameter("@Address", "0000000000")
                  );

                    await _context.SaveChangesAsync();

                    existingAccount = await _context.Accounts.Where(x => x.Email == googleTokenInfo.Email).FirstOrDefaultAsync();
                }


                if (existingAccount == null)
                {
                    return BadRequest("Lỗi đăng nhập");
                }

                var token = GenerateJwtToken(existingAccount);
                return Ok(new
                {
                    message = "Đăng nhập thành công!",
                    token = token,
                    user = new
                    {
                        existingAccount.Id,
                        existingAccount.Username,
                        existingAccount.Email,
                        existingAccount.Role
                    }
                });

            }
            return BadRequest("Lỗi đăng nhập");
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
                    new SqlParameter("@Address", request.Address ?? (object)DBNull.Value),
                       new SqlParameter("@IsDeleted", false)
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
                return Ok(new { message = "Đăng xuất thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Lỗi khi đăng xuất: {ex.Message}" });
            }
        }




        [HttpGet("check-role")]
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
            return Ok(new { allowed = true, redirect = "/layout" });

            // Từ chối các role khác (Shipper, Customer)
            return StatusCode(403, new { allowed = false, redirect = "/access-denied", message = "Bạn không có quyền truy cập trang này." });
        }


        private string GenerateJwtToken(Account account)
        {
            var key = Encoding.UTF8.GetBytes("ThisIsAReallyStrongSecretKeyForJWT123!nhanptmps40527@gmail.com");
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role),

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
        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return accounts;
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Account>>> GetActiveAccounts()
        {
            var activeAccounts = await _context.Accounts
                .Where(a => !a.IsDeleted)
                .ToListAsync();

            return Ok(activeAccounts);
        }

        [HttpGet("deleted")]
        public async Task<ActionResult<IEnumerable<Account>>> GetDeletedAccounts()
        {
            var deletedAccounts = await _context.Accounts
                .Where(a => a.IsDeleted)
                .ToListAsync();

            return Ok(deletedAccounts);
        }



        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Accounts.AnyAsync(a => a.Username == request.Username || a.Email == request.Email))
                return BadRequest("Username hoặc Email đã tồn tại.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var account = new Account
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
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (account == null)
                return NotFound("Không tìm thấy tài khoản.");

            account.IsDeleted = true; // Soft delete
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa tài khoản thành công!" });
        }

        private async Task<int> GetNextAvailableId()
        {
            // Find the first deleted account's ID
            var deletedAccount = await _context.Accounts
                .Where(a => a.IsDeleted)
                .OrderBy(a => a.Id)
                .FirstOrDefaultAsync();

            if (deletedAccount != null)
            {
                return deletedAccount.Id;
            }

            // If no deleted accounts, use the next available ID
            var maxId = await _context.Accounts.MaxAsync(a => (int?)a.Id) ?? 0;
            return maxId + 1;
        }
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null || !account.IsDeleted)
                return NotFound("Không tìm thấy tài khoản đã xóa.");

            account.IsDeleted = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Khôi phục tài khoản thành công!" });
        }
    }
}