using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace ASM_NhomSugar_SD19311.Service
{
    public class AuthService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;


        public AuthService(ILocalStorageService localStorage, HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            // Xóa thêm các key khác nếu có
            await _localStorage.ClearAsync(); // Xóa toàn bộ localStorage (tùy chọn, nếu cần)
        }

        // Configure HttpClient to include the JWT token in requests
        public async Task ConfigureHttpClientAsync()
        {
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
