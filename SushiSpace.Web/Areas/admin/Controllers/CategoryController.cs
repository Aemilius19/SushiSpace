using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SushiSpace.Web.Models.DTOs;
using SushiSpace.Web.Models.ViewModels;
using System.Text;

namespace SushiSpace.Web.Areas.admin.Controllers
{
    
    [Area("admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {

        private readonly HttpClient _httpClient;

        public CategoryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var categoryResponse = await _httpClient.GetAsync("http://localhost:5119/api/Category");
            categoryResponse.EnsureSuccessStatusCode();
            var categoryResponseString= await categoryResponse.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<IEnumerable<CategoryViewModel>>(categoryResponseString);
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO categoryDTO)
        {
            try
            {
                var jsonProduct = JsonConvert.SerializeObject(categoryDTO);
                var content = new StringContent(jsonProduct, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://localhost:5119/api/Category", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create category.");
                    return View(categoryDTO);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(categoryDTO);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5119/api/Category/{id}");
                response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешен (код 2xx)

                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                // Обработка ошибок HTTP запроса
                ModelState.AddModelError(string.Empty, $"Error deleting category: {ex.Message}");
                return RedirectToAction("Index"); // Можно изменить на другое действие или представление при необходимости
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var responseString = await _httpClient.GetAsync($"http://localhost:5119/api/Category/{id}");
                responseString.EnsureSuccessStatusCode();
                var content= await responseString.Content.ReadAsStringAsync();
                var category=JsonConvert.DeserializeObject<CategoryViewModel>(content);
                return View(category);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, $"Error deleting category: {ex.Message}");
                return RedirectToAction("Index");
            }
           
        }
        [HttpPost]
        public async Task<IActionResult> Update(CategoryViewModel model)
        {
            try
            {
                var dto = new CategoryDTO()
                {
                    Name = model.Name,
                };
                var Jsoncategory=JsonConvert.SerializeObject(dto);
                var content=new StringContent(Jsoncategory,System.Text.Encoding.UTF8, "application/json");
                var request = await _httpClient.PutAsync($"http://localhost:5119/api/Category/{model.Id}", content);
                request.EnsureSuccessStatusCode();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, $"Error deleting category: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

    }
}
