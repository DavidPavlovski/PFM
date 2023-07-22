using AutoMapper;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using PFM.DataAccess.Entities;
using PFM.DataAccess.Repositories.Abstraction;
using PFM.DataAccess.UnitOfWork;
using PFM.DataTransfer;
using PFM.DataTransfer.Category;
using PFM.Exceptions;
using PFM.Helpers.CSVParser;
using PFM.Mapping.CSVMapping;
using PFM.Services.Abstraction;
using System.Net;

namespace PFM.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICategoryRepository _categoryRepository;
        readonly IMapper _mapper;
        readonly ICSVParser _csvParser;
        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository, IMapper mapper, ICSVParser csvParser)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _csvParser = csvParser;
        }

        public async Task<List<CategoryResponseDto>> GetCategories(string? parentCode)
        {
            var res = await _categoryRepository.GetCategories(parentCode);
            return _mapper.Map<List<CategoryResponseDto>>(res);
        }

        public async Task<Result<ResponseModel>> ImportCategoriesAsync(IFormFile file)
        {

            var csvResponse = _csvParser.ParseCSV<Category, CategoryCSVMap>(file);
            if (csvResponse is null)
            {
                var error = new FileLoadException("Error occured while reading file , make sure the file you uploaded is a valid CSV file.");
                return new Result<ResponseModel>(error);
            }
            if (csvResponse.Errors.Any())
            {
                var exception = new CustomException("Error occured while reading CSV file")
                {
                    Description = "Problem occured while parsing CSV values , see errors below.",
                    StatusCode = HttpStatusCode.BadRequest,
                    Errors = csvResponse.Errors
                };
                return new Result<ResponseModel>(exception);
            }

            _categoryRepository.ImportCategories(csvResponse.Items);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                var res = new ResponseModel
                {
                    Message = "Categories imported successfully."
                };
                return res;
            }
            catch
            {
                var exception = new Exception("Error occured while writing in database");
                return new Result<ResponseModel>(exception);
            }
        }
    }
}
