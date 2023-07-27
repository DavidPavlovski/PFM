using PFM.DataAccess.Entities;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ICategoryRepository
    {
        Task ImportCategories(IEnumerable<Category> entities , int batchSize);
        Task<List<Category>> GetCategories(string? parentCode);
        Task<List<string>> GetCatCodesAsync();
        Task<bool> ExistsAsync(string code);
    }
}
