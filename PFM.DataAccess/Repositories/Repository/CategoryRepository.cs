﻿using Microsoft.EntityFrameworkCore;
using PFM.DataAccess.DbContextOption;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;

namespace PFM.DataAccess.Repositories.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await _dbContext.Categories.AnyAsync(x => x.Code == code);
        }

        public async Task<List<Category>> GetCategories(string? parentCode)
        {
            return await _dbContext.Categories
                            .Where(x => parentCode == null ?
                            x.ParentCode == string.Empty :
                            x.ParentCode == parentCode)
                            .ToListAsync();
        }

        public void ImportCategories(List<Category> entities)
        {
            foreach (var entity in entities)
            {
                if (_dbContext.Categories.Contains(entity))
                {
                    _dbContext.Categories.Update(entity);
                }
                else
                {
                    _dbContext.Categories.Add(entity);
                }
            }
        }
    }
}
