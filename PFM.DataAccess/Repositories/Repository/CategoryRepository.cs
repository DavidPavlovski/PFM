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

        public void ImportCategories(IEnumerable<Category> entities)
        {
            var existingCategories = new HashSet<Category>(_dbContext.Categories);

            var entitiesToAdd = new List<Category>();
            var entitiesToUpdate = new List<Category>();

            foreach (var entity in entities)
            {
                var e = existingCategories.FirstOrDefault(x => x.Code == entity.Code);
                if (e is null)
                {
                    entitiesToAdd.Add(entity);
                }
                else
                {
                    e.Update(entity);
                    entitiesToUpdate.Add(e);
                }
            }

            if (entitiesToAdd.Count > 0)
            {
                _dbContext.Categories.AddRange(entitiesToAdd);
            }
            if (entitiesToUpdate.Count > 0)
            {
                _dbContext.Categories.UpdateRange(entitiesToUpdate);
            }
        }

        public async Task<List<string>> GetCatCodesAsync()
        {
            return await _dbContext.Categories.Select(x => x.Code).ToListAsync();
        }
    }
}
