using CommerceSystemAPI.DTOs;
using CommerceSystemAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommerceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/Category")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("GetCategoryById")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound("Category Not Found");
            }

            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(CategoryCreateDTO dto)
        {
            await _categoryService.AddAsync(dto);

            return Ok("Category Added Successfully");
        }

    }
}

