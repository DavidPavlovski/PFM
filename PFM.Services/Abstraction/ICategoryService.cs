using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataTransfer;
using PFM.DataTransfer.Category;

namespace PFM.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<Result<ResponseModel>> ImportCategoriesAsync(IFormFile file);
        Task<CategoryListDto> GetCategories(string? parentCode);
    }
}
