using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;

namespace PFM.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<List<Category>> ImportCategoriesAsync(IFormFile file);
    }
}
