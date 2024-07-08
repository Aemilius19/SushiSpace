using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Bussines.Services.Concretes;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using SushiSpace.Core.Helper;
using System;
using System.Linq.Expressions;

namespace SushiSpace.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productservice;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IProductServices services, IWebHostEnvironment environment)
        {
            _productservice = services;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryParameters queryParameters)
        {
            try
            {

                var data = await _productservice.GetAll(
                   queryParameters
                );

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving products: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductDTO dto)
        {
            try
            {
                if (dto.ImgFile == null || dto.ImgFile.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Ensure the uploads folder exists
                string path = Path.Combine(_environment.WebRootPath, "Images", "products");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Generate a unique filename
                string filename = Guid.NewGuid() + Path.GetExtension(dto.ImgFile.FileName);
                string fullpath = Path.Combine(path, filename);

                // Save the image to the server
                using (FileStream stream = new FileStream(fullpath, FileMode.Create))
                {
                    await dto.ImgFile.CopyToAsync(stream);
                }

                // Set the image URL
                dto.ImgUrl = filename;

                // Call the service to create the product
                var result = await _productservice.Create(dto);

                if (result!=null)
                {
                    

                    return StatusCode(StatusCodes.Status201Created, dto);
                }

                return BadRequest("Failed to create the product.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productservice.Delete(id);
                if (result is false)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var result = await _productservice.GetProduct(id);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromForm]ProductDTO dto, int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid product ID");
                }

                // Проверяем, было ли загружено новое изображение
                if (dto.ImgFile != null && dto.ImgFile.Length > 0)
                {
                    // Удаляем старое изображение, если оно существует
                    var existingProduct = await _productservice.GetProduct(id);
                    if (existingProduct == null)
                    {
                        return NotFound("Product not found");
                    }

                    // Удаляем старое изображение
                    await DeleteExistingImage(existingProduct);

                    // Сохраняем новое изображение
                    string imagePath = await SaveNewImage(dto.ImgFile);

                    // Обновляем URL изображения в DTO
                    dto.ImgUrl = imagePath;
                }

                // Вызываем метод сервиса для обновления продукта
                var result = await _productservice.Update(dto, id);

                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to update product");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        private async Task<string> SaveNewImage(IFormFile imgFile)
        {
            try
            {
                // Генерируем уникальное имя файла
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imgFile.FileName;

                // Путь для сохранения файла в папке Images/products
                string imagePath = Path.Combine(_environment.WebRootPath, "Images", "products", uniqueFileName);

                // Сохраняем файл на сервере
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await imgFile.CopyToAsync(fileStream);
                }

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save image: {ex.Message}");
            }
        }

        private async Task DeleteExistingImage(Product existingProduct)
        {
            try
            {
                // Путь к существующему изображению
                string existingImagePath = Path.Combine(_environment.WebRootPath, "Images", "products", existingProduct.ImgUrl);
                FileInfo File=new FileInfo(existingImagePath);
                // Удаляем файл изображения, если он существует
                if (File.Exists)
                {
                    File.Delete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete existing image: {ex.Message}");
            }
        }
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var allProducts = await _productservice.GetAll(new ProductQueryParameters());
                var productsInCategory = allProducts.Where(p => p.CategoryId == categoryId).ToList();
                return Ok(productsInCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving products: {ex.Message}");
            }
        }

    }
}

