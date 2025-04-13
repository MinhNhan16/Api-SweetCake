using ASM_NhomSugar_SD19311.DTO.Request;
using ASM_NhomSugar_SD19311.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ASM_NhomSugar_SD19311.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]

        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest createCategoryRequest)
        {
            var newCategory = await _categoryService.CreateCategoryAsync(createCategoryRequest);
            return Ok(newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryRequest updateCategoryRequest)
        {
            var updatedCategory = await _categoryService.UpdateCategoryAsync(updateCategoryRequest);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            return Ok(result);
        }
    }
}
