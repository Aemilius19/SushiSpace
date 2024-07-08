using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using Newtonsoft.Json;
using SushiSpace.Web.Models.DTOs;
using SushiSpace.Web.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace SushiSpace.Web.Areas.admin.Controllers
{
    

    [Area("admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {

        private readonly HttpClient _httpClient;

        public ProductController(HttpClient httpClient)
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
        public IActionResult Create(int categoryId)
        {
            ProductDTO product = new ProductDTO()
            {
                CategoryId = categoryId
            };
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(product.Name), "Name");
                content.Add(new StringContent(product.Description ?? string.Empty), "Description");
                content.Add(new StringContent(product.Price.ToString()), "Price");
                content.Add(new StringContent(product.CategoryId.ToString()), "CategoryId");

                if (product.ImgFile != null)
                {
                    var fileStreamContent = new StreamContent(product.ImgFile.OpenReadStream());
                    fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(product.ImgFile.ContentType);
                    content.Add(fileStreamContent, "ImgFile", product.ImgFile.FileName);
                }

                var productResponse = await _httpClient.PostAsync("http://localhost:5119/api/Product", content);

                if (productResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                }
            }

            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://localhost:5119/api/Product/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while deleting the product.");
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View("Error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var productResponse = await _httpClient.GetAsync($"http://localhost:5119/api/Product/{id}");
                    
            if (!productResponse.IsSuccessStatusCode) { return RedirectToAction("Index"); }

            var content= await productResponse.Content.ReadAsStringAsync();

            var product = JsonConvert.DeserializeObject<ProductViewModel>(content);
            return View(product);

        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                var formContent = new MultipartFormDataContent();

                formContent.Add(new StringContent(productVM.Name), nameof(ProductDTO.Name));
                formContent.Add(new StringContent(productVM.Description ?? string.Empty), nameof(ProductDTO.Description));
                formContent.Add(new StringContent(productVM.Price.ToString()), nameof(ProductDTO.Price));
                formContent.Add(new StringContent(productVM.CategoryId.ToString()), nameof(ProductDTO.CategoryId));
                formContent.Add(new StringContent(productVM.ImgUrl ?? string.Empty), nameof(ProductDTO.ImgUrl));

                if (productVM.ImgFile != null)
                {
                    var fileContent = new StreamContent(productVM.ImgFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(productVM.ImgFile.ContentType);
                    formContent.Add(fileContent, nameof(ProductDTO.ImgFile), productVM.ImgFile.FileName);
                }

                var productResponse = await _httpClient.PutAsync($"http://localhost:5119/api/Product/{productVM.Id}", formContent);

                if (productResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                }
            }

            return View(productVM);
        }


    }
}
