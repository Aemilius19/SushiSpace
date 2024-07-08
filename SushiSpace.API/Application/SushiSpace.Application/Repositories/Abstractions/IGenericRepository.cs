using SushiSpace.Application.Data.Context;
using SushiSpace.Core.Common;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Application.Repositories.Abstractions
{
    public interface IGenericRepository<T> where T: BaseEntity,new()
        
    {
        Task<IQueryable<T>> GetAll(Expression<Func<T, object>>? orders = null, Expression<Func<T, bool>>? filters = null, params string[] includes);
        Task<T> GetEntity(string id,Expression<Func<T, object>>? orders = null, Expression<Func<T, bool>>? expression = null, params string[]? includes);

        Task<T> Create(T entity);

        Task<bool> Delete(int id);

        Task SaveChangesAsync();

    }
}
