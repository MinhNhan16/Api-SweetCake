using ASM_NhomSugar_SD19311.DTO.Request;
using ASM_NhomSugar_SD19311.Interface;
using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Categories> CreateCategoryAsync(CreateCategoryRequest createCategoryRequest)
        {

            var existingCategory = await _categoryRepository.FindCategoryByName(createCategoryRequest.Name);

            if (existingCategory != null)
            {
                throw new Exception("Danh mục đã tồn tại");
            }

            return await _categoryRepository.CreateCategoryAsync(createCategoryRequest);
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            return await _categoryRepository.DeleteCategoryAsync(categoryId);
        }

        public async Task<List<Categories>> GetCategoriesAsync()
        {
            return await _categoryRepository.GetCategoriesAsync();
        }
    }
}
