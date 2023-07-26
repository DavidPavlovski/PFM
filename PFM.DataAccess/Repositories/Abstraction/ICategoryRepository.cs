using PFM.DataAccess.Entities;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategories(string? parentCode);
        void ImportCategories(IEnumerable<Category> entities);
        Task<bool> ExistsAsync(string code);
        Task<List<string>> GetCatCodesAsync();
    }
}
