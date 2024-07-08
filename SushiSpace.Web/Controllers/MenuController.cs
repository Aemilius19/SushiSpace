using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SushiSpace.Web.Models.ViewModels;

namespace SushiSpace.Web.Controllers
{
    public class MenuController:Controller
    {
        private readonly HttpClient _httpClient;

        public MenuController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var productsResponse = await _httpClient.GetAsync("http://localhost:5119/api/Product");
            productsResponse.EnsureSuccessStatusCode();
            var productsResponseString = await productsResponse.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(productsResponseString);

            var categoriesResponse = await _httpClient.GetAsync("http://localhost:5119/api/Category");
            categoriesResponse.EnsureSuccessStatusCode();
            var categoriesResponseString = await categoriesResponse.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<IEnumerable<CategoryViewModel>>(categoriesResponseString);

            var viewModel = new ProductIndexViewModel
            {
                Products = products,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var productsResponse = await _httpClient.GetAsync($"http://localhost:5119/api/Product/category/{categoryId}");
            productsResponse.EnsureSuccessStatusCode();
            var productsResponseString = await productsResponse.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(productsResponseString);

            return Json(products);
        }

    }
}
