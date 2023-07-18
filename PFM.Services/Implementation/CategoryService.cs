using AutoMapper;
using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataAccess.UnitOfWork;
using PFM.DataTransfer.Category;
using PFM.Helpers.CSVParser;
using PFM.Mapping.CSVMapping;
using PFM.Services.Abstraction;

namespace PFM.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICategoryRepository _categoryRepository;
        readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryResponseDto>> GetCategories(string? parentCode)
        {
            var res = await _categoryRepository.GetCategories(parentCode);
            return _mapper.Map<List<CategoryResponseDto>>(res);
        }

        public async Task<List<CategoryResponseDto>> ImportCategoriesAsync(IFormFile file)
        {
            var categories = CSVParser.ParseCSV<Category, CategoryCSVMap>(file);
            _categoryRepository.ImportCategories(categories);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<List<CategoryResponseDto>>(categories.Take(10).ToList());
            }
            catch
            {
                return null;
            }
        }

    }
}
