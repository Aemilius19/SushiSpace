using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SushiSpace.Web.Models.Entites;
using SushiSpace.Web.Models.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SushiSpace.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
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

            var cart = GetCartProducts();
            
            var viewModel = new ProductIndexViewModel
            {
                Products = products,
                Categories = categories,
                cartProducts=cart
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

        private List<CartProduct> GetCartProducts()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var cartKey = $"Cart-{userId}";
                var existingCart = _httpContextAccessor.HttpContext.Request.Cookies[cartKey];
                if (existingCart != null)
                {
                    return JsonConvert.DeserializeObject<List<CartProduct>>(existingCart);
                }
            }

            return new List<CartProduct>();
        }
    }
}
