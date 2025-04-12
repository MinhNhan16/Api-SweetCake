using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.Service
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        // Constructor nhận HttpClient qua DI
        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy tất cả sản phẩm
        public async Task<List<Products>> GetAllProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:44366/api/account/");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Products>>();
                }
                return new List<Products>(); // Trả về danh sách rỗng nếu không thành công
            }
            catch (Exception ex)
            {
                // Có thể thêm logging ở đây nếu cần
                Console.WriteLine($"Error in GetAllProductsAsync: {ex.Message}");
                return new List<Products>();
            }
        }

        // Lấy sản phẩm theo ID
        public async Task<Products> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:44366/api/account/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Products>();
                }
                return null; // Trả về null nếu không tìm thấy sản phẩm
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductByIdAsync: {ex.Message}");
                return null;
            }
        }

        // Tạo sản phẩm mới
        public async Task<bool> CreateProductAsync(Products product)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:44366/api/account/", product);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateProductAsync: {ex.Message}");
                return false;
            }
        }

        // Cập nhật sản phẩm
        public async Task<bool> UpdateProductAsync(int id, Products product)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:44366/api/account/{id}", product);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProductAsync: {ex.Message}");
                return false;
            }
        }

        // Xóa sản phẩm
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"https://localhost:44366/api/account/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteProductAsync: {ex.Message}");
                return false;
            }
        }
    }
}

