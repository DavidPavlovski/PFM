using PFM.DataAccess.Entities;

namespace PFM.DataAccess.Repositories.Abstraction
{
    public interface ICategoryRepository
    {
        void InsertCategories(List<Category> entities);
        Task<bool> ExistsAsync(string code);
    }
}
