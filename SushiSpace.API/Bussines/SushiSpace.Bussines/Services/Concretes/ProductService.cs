using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using SushiSpace.Core.Helper;
using SushiSpace.Core.Helper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Concretes
{
    public class ProductService : IProductServices

    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        

        public ProductService(
            IProductRepository repository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
            
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            
        }

        public async Task<Product> Create(ProductDTO productDTO)
        {
           var product = _mapper.Map<Product>(productDTO);
           product.CreateTime= DateTime.Now;
           
            if (productDTO.CategoryId.HasValue)
            {
                var category= await _categoryRepository.GetEntity(productDTO.CategoryId.Value.ToString());
                if (category is not null)
                {
                    product.Category = category;
                }
            }
            var result = await _repository.Create(product);
            return result;
        }

        public async Task<bool> Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid product ID", nameof(id));
            }

            var product = await _repository.GetEntity(id.ToString());
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            var result = await _repository.Delete(product.Id);
            return result;
        }


        public async Task<IQueryable<Product>> GetAll(ProductQueryParameters queryParameters)
        {
            var products = await _repository.GetAll(null, c => c.CategoryId > 0, "Category");

            var filteredProducts = products.AsQueryable();

            // Применение фильтров
            if (!string.IsNullOrEmpty(queryParameters.FilterByName))
            {
                var filterByNameLower = queryParameters.FilterByName.ToLower();
                filteredProducts = filteredProducts.Where(p => p.Name.ToLower().Contains(filterByNameLower));
            }

            if (queryParameters.MinPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= queryParameters.MinPrice.Value);
            }

            if (queryParameters.MaxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= queryParameters.MaxPrice.Value);
            }

            // Применение сортировки
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                filteredProducts = filteredProducts.OrderByField(queryParameters.SortBy, queryParameters.SortDescending);
            }

            return filteredProducts;
        }



        public async Task<Product> GetProduct(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid product ID", nameof(id));
            }

            var product = await _repository.GetEntity(id.ToString(), null, p => p.Id == id, "Category");

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            return product;
        }




        public async Task<bool> Update(ProductDTO dto, int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid product ID", nameof(id));
            }
            var existingProduct = await _repository.GetEntity(id.ToString());
            if (existingProduct == null)
            {
                throw new KeyNotFoundException("Product not found");
            }
            if (dto.ImgUrl is null)
            {
                dto.ImgUrl = existingProduct.ImgUrl;
            }

            _mapper.Map(dto, existingProduct);

            if (dto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetEntity(dto.CategoryId.Value.ToString());
                if (category is not null)
                {
                    existingProduct.Category = category;
                }
                
            }

            await _repository.SaveChangesAsync();
            return true;
            


        }
    }
}
