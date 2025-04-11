using ASM_NhomSugar_SD19311.DTO;

namespace ASM_NhomSugar_SD19311.Service
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;

        // Constructor nhận HttpClient qua DI
        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy tất cả tài khoản
        public async Task<List<AccountDto>> GetAllAccountsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/account");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<AccountDto>>();
                }
                return new List<AccountDto>(); // Trả về danh sách rỗng nếu không thành công
            }
            catch (Exception ex)
            {
                // Có thể thêm logging ở đây nếu cần
                Console.WriteLine($"Error in GetAllAccountsAsync: {ex.Message}");
                return new List<AccountDto>();
            }
        }

        // Tạo tài khoản mới
        public async Task<bool> CreateAccountAsync(AccountDto account)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/account/create", account);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAccountAsync: {ex.Message}");
                return false;
            }
        }

        // Cập nhật tài khoản
        public async Task<bool> UpdateAccountAsync(int id, AccountDto account)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/account/{id}", account);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAccountAsync: {ex.Message}");
                return false;
            }
        }

        // Xóa tài khoản
        public async Task<bool> DeleteAccountAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/account/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAccountAsync: {ex.Message}");
                return false;
            }
        }
    }
}
