using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using SushiSpace.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Bussines.Services.Abstractions
{
    public interface ICategoryService
    {
        Task<bool> Create(CategoryDTO categoryDTO);
        Task<bool> Update(CategoryDTO categoryDTO, int id);
        Task<bool> Delete(int id);
        Task<Category> GetCategory(int id);
        Task<IEnumerable<Category>> GetAll(CategoryQueryParameters queryParameters);
    }
}
