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

        public void InsertCategories(List<Category> entities)
        {
            _dbContext.Categories.UpdateRange(entities);
        }
    }
}
