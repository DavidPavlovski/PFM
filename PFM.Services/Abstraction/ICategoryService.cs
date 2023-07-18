using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataTransfer.Category;

namespace PFM.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> ImportCategoriesAsync(IFormFile file);
        Task<List<CategoryResponseDto>> GetCategories(string? parentCode);
    }
}
