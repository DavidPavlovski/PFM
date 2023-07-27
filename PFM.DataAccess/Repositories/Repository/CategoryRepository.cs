using Microsoft.EntityFrameworkCore;
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

        public async Task ImportCategories(IEnumerable<Category> entities , int batchSize)
        {
            await _dbContext.BulkMergeAsync(entities, options =>
            {
                options.BatchSize = batchSize;
                options.ColumnPrimaryKeyExpression = t => t.Code;
                options.OnMergeUpdateInputExpression = t => new
                {
                    t.ParentCode,
                    t.Name
                };
            });
        }

        public async Task<List<string>> GetCatCodesAsync()
        {
            return await _dbContext.Categories.Select(x => x.Code).ToListAsync();
        }
    }
}
