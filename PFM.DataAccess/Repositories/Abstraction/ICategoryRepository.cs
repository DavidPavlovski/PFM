using PFM.DataAccess.Entities;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategories(string? parentCode);
        Task ImportCategories(IEnumerable<Category> entities , int batchSize);
        Task<bool> ExistsAsync(string code);
        Task<List<string>> GetCatCodesAsync();
    }
}
