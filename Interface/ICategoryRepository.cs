using ASM_NhomSugar_SD19311.DTO.Request;
using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.Interface
{
    public interface ICategoryRepository
    {

        public Task<Categories> FindCategoryByName(string categoryName);

        public Task<Categories> CreateCategoryAsync(CreateCategoryRequest createCategoryRequest);

        public Task<List<Categories>> GetCategoriesAsync();

        public Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
