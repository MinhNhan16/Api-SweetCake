using ASM_NhomSugar_SD19311.DTO.Request;
using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.Interface
{
    public interface ICategoryService
    {
        public Task<Categories> CreateCategoryAsync(CreateCategoryRequest createCategoryRequest);

        public Task<List<Categories>> GetCategoriesAsync();

        public Task<bool> DeleteCategoryAsync(int categoryId);
        public Task<Categories> UpdateCategoryAsync(UpdateCategoryRequest updateCategoryRequest);

    }
}
