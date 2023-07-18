using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataTransfer.Category;

namespace PFM.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<Result<List<CategoryResponseDto>>> ImportCategoriesAsync(IFormFile file);
        Task<List<CategoryResponseDto>> GetCategories(string? parentCode);
    }
}
