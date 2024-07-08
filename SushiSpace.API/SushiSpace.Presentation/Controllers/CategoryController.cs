using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Helper;

namespace SushiSpace.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
      

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CategoryQueryParameters queryParameters)
        {
            try
            {
                var categories = await _categoryService.GetAll(
                    queryParameters
                );
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving categories: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                
                var category = await _categoryService.GetCategory(id);
                return Ok(category);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO categoryDTO)
        {
            try
            {
                
                var result = await _categoryService.Create(categoryDTO);
                if (result)
                {
                    return StatusCode(StatusCodes.Status201Created, categoryDTO);
                }
                return BadRequest("Failed to create category");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating category: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CategoryDTO categoryDTO, int id)
        {
            try
            {
                var result = await _categoryService.Update(categoryDTO, id);
                if (result)
                {
                    return Ok();
                }
                return BadRequest("Failed to update category");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating category: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.Delete(id);
                if (result)
                {
                    return Ok();
                }
                return BadRequest("Failed to delete category");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting category: {ex.Message}");
            }
        }
    }
}
