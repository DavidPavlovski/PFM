using Microsoft.AspNetCore.Mvc;
using PFM.Services.Abstraction;

namespace PFM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("Import")]
        public async Task<IActionResult> ImportCategories(IFormFile file)
        {
            var res = await _categoryService.ImportCategoriesAsync(file);
            if (res == null)
            {
                return BadRequest("Something went wrong while processing your request.");
            }
            return Ok(res);
        }
    }
}
