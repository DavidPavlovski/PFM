using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataAccess.UnitOfWork;
using PFM.Helpers.CSVParser;
using PFM.Mapping.CSVMapping;
using PFM.Services.Abstraction;

namespace PFM.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICategoryRepository _categoryRepository;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> ImportCategoriesAsync(IFormFile file)
        {
            var categories = CSVParser.ParseCSV<Category, CategoryCSVMap>(file);
            _categoryRepository.InsertCategories(categories);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return categories.Take(10).ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}
