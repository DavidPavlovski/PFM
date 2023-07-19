using AutoMapper;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataAccess.UnitOfWork;
using PFM.DataTransfer.Category;
using PFM.DataTransfer.Transaction;
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

        public async Task<Result<List<CategoryResponseDto>>> ImportCategoriesAsync(IFormFile file)
        {
            List<Category> categories;
            try
            {
                categories = CSVParser.ParseCSV<Category, CategoryCSVMap>(file);
                if (categories is null)
                {
                    var exception = new FileLoadException("Error occured while reading CSV file");
                    return new Result<List<CategoryResponseDto>>(exception);
                }
            }
            catch (ArgumentException aex)
            {
                return new Result<List<CategoryResponseDto>>(aex);
            }

            _categoryRepository.ImportCategories(categories);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<List<CategoryResponseDto>>(categories.Take(10).ToList());
            }
            catch
            {
                var exception = new Exception("Error occured while writing in database");
                return new Result<List<CategoryResponseDto>>(exception);
            }
        }

    }
}
