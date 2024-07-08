using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using SushiSpace.Web.Models.DTOs;

namespace SushiSpace.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly HttpClient _httpClient;

        public CommentController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> SendComment(CommentDTO dto, string returnUrl)
        {
            var JsonDto=JsonConvert.SerializeObject(dto);
            var content= new StringContent(JsonDto,System.Text.Encoding.UTF8,"application/json");
            var response = await _httpClient.PostAsync($"http://localhost:5119/api/Comment", content);
            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index", "Product", new { id = dto.ProductId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id, int productId)
        {
            var response = await _httpClient.DeleteAsync($"http://localhost:5119/api/Comment/{id}");
            response.EnsureSuccessStatusCode();
            TempData["Notification"] = "Comment deleted successfully.";

            return RedirectToAction("Index", "Product", new { id = productId });
        }
    }
}
