using PFM.DataAccess.Repositories.Abstraction;

namespace PFM.Validations.Category
{
    public class CategoryValidator : ICategoryValidator
    {
        readonly ICategoryRepository _categoryRepository;

        public CategoryValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> ExistsAsync(string categoryCode)
        {
            return await _categoryRepository.ExistsAsync(categoryCode);
        }
    }
}
