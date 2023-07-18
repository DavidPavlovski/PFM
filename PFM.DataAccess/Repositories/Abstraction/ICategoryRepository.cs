using PFM.DataAccess.Entities;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategories(string? parentCode);
        void ImportCategories(List<Category> entities);
        Task<bool> ExistsAsync(string code);
    }
}
