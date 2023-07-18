using Microsoft.AspNetCore.Mvc;
using PFM.Helpers.Extensions;
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
            return res.ToOk();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] string? parentCode = null)
        {
            var res = await _categoryService.GetCategories(parentCode);
            return Ok(res);
        }
    }
}
