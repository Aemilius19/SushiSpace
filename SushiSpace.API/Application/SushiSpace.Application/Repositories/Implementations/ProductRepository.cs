using AutoMapper;
using SushiSpace.Application.Data.Context;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Application.Repositories.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(SushiSpaceDb context, IMapper mapper ) : base(context, mapper)
        {
        }
        
    }
}
