using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SushiSpace.Web.Models.DTOs;
using SushiSpace.Web.Models.ViewModels;

namespace SushiSpace.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                var product = await GetProductFromApi(id);
                var relatedProducts = await GetRelatedProducts(product.CategoryId);
                var comments = await GetAllComments(id);// Получаем связанные продукты по категории

                var viewModel = new ProductAndCategoryProductViewModel
                {
                    Product = product,
                    RelatedProducts = relatedProducts,
                    Comments = comments
                    
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        private async Task<ProductViewModel> GetProductFromApi(int id)
        {
            var productsResponse = await _httpClient.GetAsync($"http://localhost:5119/api/Product/{id}");
            productsResponse.EnsureSuccessStatusCode();
            var productsResponseString = await productsResponse.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<ProductViewModel>(productsResponseString);
            return products;
        }

        private async Task<IEnumerable<ProductViewModel>> GetRelatedProducts(int id)
        {
            
            var productsByCategoryResponse = await _httpClient.GetAsync($"http://localhost:5119/api/Product/category/{id}");
            productsByCategoryResponse.EnsureSuccessStatusCode();
            var productsByCategoryResponseString = await productsByCategoryResponse.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(productsByCategoryResponseString);

            return products;

        }

        private async Task<List<CommentDTO>> GetAllComments(int id)
        {
            var comments = await _httpClient.GetAsync($"http://localhost:5119/api/Comment/product/{id}");
            comments.EnsureSuccessStatusCode();
            var commentsresponse= await comments.Content.ReadAsStringAsync();
            var commentaries = JsonConvert.DeserializeObject<List<CommentDTO>>(commentsresponse);
            return commentaries;
        }
    }
}
