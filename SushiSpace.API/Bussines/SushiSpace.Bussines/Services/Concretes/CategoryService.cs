using AutoMapper;
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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<bool> Create(CategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);
            category.CreateTime= DateTime.Now;
            category.Products = null;
            var result = await _repository.Create(category);
            if (result is not null)
            {
                return true;
            }
            return false;
        }

        public Task<bool> Delete(int id)
        {
            if (id<=0)
            {
                throw new NullReferenceException("category id can not be 0");
            }
            var result = _repository.Delete(id);
            return result;
        }

        public async Task<IEnumerable<Category>> GetAll(CategoryQueryParameters queryParameters)
        {
            var categories = await _repository.GetAll(null, c => c.Id>0, "Products");

            var filteredCategories = categories.AsQueryable();

            // Применение фильтров
            if (!string.IsNullOrEmpty(queryParameters.FilterByName))
            {
                var filterByNameLower = queryParameters.FilterByName.ToLower();
                filteredCategories = filteredCategories.Where(c => c.Name.ToLower().Contains(filterByNameLower));
            }

            // Применение сортировки
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                filteredCategories = filteredCategories.OrderByField(queryParameters.SortBy, queryParameters.SortDescending);
            }

            return await filteredCategories.ToListAsync();
        }



        public async Task<Category> GetCategory(int id)
        {
            var category = await _repository.GetEntity(id.ToString(), null,null, "Products") ;
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            return category;
        }

        public async Task<bool> Update(CategoryDTO categoryDTO, int id)
        {
            if (id <= 0)
            {
                throw new NullReferenceException("category id can not be 0");
            }
            var existingCategory= await _repository.GetEntity(id.ToString());
            if (existingCategory == null) { throw new KeyNotFoundException("this category doesnt exist"); }
            _mapper.Map(categoryDTO, existingCategory);
            existingCategory.CreateTime= DateTime.Now;
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
