using ASM_NhomSugar_SD19311.DTO;

namespace ASM_NhomSugar_SD19311.Interface
{
    public interface ICartService
    {
        Task<List<CartDto>> GetAllAsync();
        Task<List<CartDto>> GetAllByAccountIdAsync(int accountId);
        Task<CartDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CartDto cartDto); // Đổi từ Task<bool> thành Task<int>
        Task<bool> UpdateAsync(CartDto cartDto);
        Task<bool> DeleteAsync(int id);
    }
}
