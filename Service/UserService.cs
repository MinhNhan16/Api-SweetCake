//using ASM_NhomSugar_SD19311.Data;
//using ASM_NhomSugar_SD19311.Interface;
//using ASM_NhomSugar_SD19311.Model;
//using Microsoft.EntityFrameworkCore;

//namespace ASM_NhomSugar_SD19311.Service
//{
//    public class UserService : IUserService
//    {
//        private readonly CakeShopDbContext _context;
//        public UserService(CakeShopDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<Accounts>> GetAllUsersAsync()
//        {
//            return await _context.Accounts.Include(u => u.Account).ToListAsync();
//        }


//        public async Task<Accounts> GetUserByIdAsync(int id)
//        {
//            return await _context.Accounts
//                .Include(u => u.Account)
//                .FirstOrDefaultAsync(u => u.Id == id);
//        }

//        public async Task AddUserAsync(Accounts user)
//        {
//            var accountExists = await _context.Accounts.AnyAsync(a => a.Id == user.AccountId);
//            if (!accountExists)
//            {
//                throw new ArgumentException($"AccountId {user.AccountId} không tồn tại.");
//            }
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateUserAsync(Users user)
//        {
//            var accountExists = await _context.Accounts.AnyAsync(a => a.Id == user.AccountId);
//            if (!accountExists)
//            {
//                throw new ArgumentException($"AccountId {user.AccountId} không tồn tại.");
//            }
//            _context.Entry(user).State = EntityState.Modified;
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteUserAsync(int id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user != null)
//            {
//                _context.Users.Remove(user);
//                await _context.SaveChangesAsync();
//            }
//        }
//        public async Task<bool> TestConnectionAsync()
//        {
//            try
//            {
//                var canConnect = await _context.Database.CanConnectAsync();
//                Console.WriteLine($"Database connection successful: {canConnect}");
//                return canConnect;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Database connection failed: {ex.Message} - {ex.StackTrace}");
//                return false;
//            }
//        }

//    }
//}
