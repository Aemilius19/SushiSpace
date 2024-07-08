using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SushiSpace.Web.Models.DTOs;
using SushiSpace.Web.Models.Entites;
using SushiSpace.Web.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SushiSpace.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public CartController(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }

        // Просмотр корзины
        public async Task<IActionResult> Index()
        {
            var cartProducts = GetCartProducts();
            IEnumerable<ProductViewModel> productsRelated = new List<ProductViewModel>();

            foreach (var product in cartProducts)
            {
                var categoryId = product.CategoryId;
                var productsByCategoryResponse = await _httpClient.GetAsync($"http://localhost:5119/api/Product/category/{categoryId}");

                if (productsByCategoryResponse.IsSuccessStatusCode)
                {
                    var productsByCategoryResponseString = await productsByCategoryResponse.Content.ReadAsStringAsync();
                    productsRelated = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(productsByCategoryResponseString);

                    if (productsRelated.Any())
                    {
                        // Убираем из списка связанные продукты, которые уже есть в корзине
                        productsRelated = productsRelated.Where(p => !cartProducts.Any(cp => cp.ProductId == p.Id)).ToList();
                        if (productsRelated.Any())
                        {
                            break;
                        }
                    }
                }
            }

            var cartViewModel = new CartViewModel
            {
                CartProducts = cartProducts,
                RelatedProducts = productsRelated
            };

            return View(cartViewModel);
        }

        // Добавление продукта в корзину
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var cartProducts = GetCartProducts();
            var existingProduct = cartProducts.FirstOrDefault(p => p.ProductId == productId);

            if (existingProduct != null)
            {
                existingProduct.Quantity += quantity;
            }
            else
            {
                var productsResponse = await _httpClient.GetAsync($"http://localhost:5119/api/Product/{productId}");
                productsResponse.EnsureSuccessStatusCode();
                var productsResponseString = await productsResponse.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductViewModel>(productsResponseString);

                if (product != null)
                {
                    cartProducts.Add(new CartProduct
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        ImageUrl = product.ImgUrl,
                        Quantity = quantity,
                        CategoryId = product.CategoryId // Заполнение CategoryId
                    });
                }
            }

            SaveCartProducts(cartProducts);
            return RedirectToAction("Index");
        }

        // Обновление количества продуктов в корзине
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cartProducts = GetCartProducts();
            var existingProduct = cartProducts.FirstOrDefault(p => p.ProductId == productId);

            if (existingProduct != null)
            {
                existingProduct.Quantity = quantity;
            }

            SaveCartProducts(cartProducts);
            return RedirectToAction("Index");
        }

        // Удаление продукта из корзины
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cartProducts = GetCartProducts();
            var productToRemove = cartProducts.FirstOrDefault(p => p.ProductId == productId);

            if (productToRemove != null)
            {
                cartProducts.Remove(productToRemove);
            }

            SaveCartProducts(cartProducts);
            return RedirectToAction("Index");
        }

        // Очистка корзины
        [HttpPost]
        public IActionResult ClearCart()
        {
            SaveCartProducts(new List<CartProduct>());
            return RedirectToAction("Index");
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

        private void SaveCartProducts(List<CartProduct> cartProducts)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var cartKey = $"Cart-{userId}";
                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    Secure = true,
                    HttpOnly = true,
                    Path = "/"
                };

                var jsonCart = JsonConvert.SerializeObject(cartProducts);
                _httpContextAccessor.HttpContext.Response.Cookies.Append(cartKey, jsonCart, options);
            }
        }
    }
}
