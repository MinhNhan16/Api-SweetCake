﻿using ASM_NhomSugar_SD19311.Data;
using ASM_NhomSugar_SD19311.DTO.Request;
using ASM_NhomSugar_SD19311.Interface;
using ASM_NhomSugar_SD19311.Model;
using Microsoft.EntityFrameworkCore;

namespace ASM_NhomSugar_SD19311.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CakeShopDbContext _dbContext;
        public CategoryRepository(CakeShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Categorie> FindCategoryByName(string categoryName)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(x => x.Name.ToLower() == categoryName.ToLower());
        }

        public async Task<Categorie> CreateCategoryAsync(CreateCategoryRequest createCategoryRequest)
        {
            var result = await _dbContext.Categories.AddAsync(new Categorie
            {
                Name = createCategoryRequest.Name,
            });
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return false;
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Categorie?> FindCategoryById(int id)
        {
            return await _dbContext.Categories.FindAsync(id);
        }

        public async Task<Categorie> UpdateCategoryAsync(Categorie category)
        {
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Categorie>> GetCategoriesAsync()
        {
            return await _dbContext.Categories.OrderByDescending(c => c.Id).ToListAsync();
        }

    }
}
