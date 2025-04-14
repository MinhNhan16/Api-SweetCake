using ASM_NhomSugar_SD19311.DTO.Request;
using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.Interface
{
    public interface ICategoryRepository
    {

        public Task<Categorie> FindCategoryByName(string categoryName);

        public Task<Categorie> CreateCategoryAsync(CreateCategoryRequest createCategoryRequest);

        public Task<List<Categorie>> GetCategoriesAsync();

        public Task<bool> DeleteCategoryAsync(int categoryId);
        public Task<Categorie?> FindCategoryById(int id);
        public Task<Categorie> UpdateCategoryAsync(Categorie category);
    }
}
