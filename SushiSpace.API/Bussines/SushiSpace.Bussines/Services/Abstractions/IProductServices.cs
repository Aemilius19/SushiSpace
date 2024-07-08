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
   public interface  IProductServices
    {
        Task<Product> Create(ProductDTO productDTO);

        Task<IQueryable<Product>> GetAll(ProductQueryParameters queryParameters);

        Task<Product> GetProduct(int id);

        Task<bool> Update(ProductDTO dto, int id);

        Task<bool> Delete(int id);
    }
}
