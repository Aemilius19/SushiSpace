using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;

namespace SushiSpace.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentDTO commentDTO)
        {
            try
            {
                var result = await _commentService.Create(commentDTO);
                if (result)
                {
                    return StatusCode(StatusCodes.Status201Created, commentDTO);
                }
                return BadRequest("Failed to create comment");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating comment: {ex.Message}");
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetAll(int productId)
        {
            try
            {
                var comments = await _commentService.GetAll(productId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving comments: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _commentService.Delete(id);
                if (result)
                {
                    return Ok("comment deleted succesfully");
                }
                return BadRequest("Failed to delete comment");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting comment: {ex.Message}");
            }
        }
    }
}
