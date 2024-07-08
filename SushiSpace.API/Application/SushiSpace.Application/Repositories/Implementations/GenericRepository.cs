using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SushiSpace.Application.Data.Context;
using SushiSpace.Application.Mapper;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Core.Common;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Application.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
        
    {
        private readonly SushiSpaceDb _context;
        private readonly IMapper _mapper;
        public GenericRepository(SushiSpaceDb context, IMapper mapper = null)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<T> Create(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                // Логирование внутреннего исключения
                throw new Exception("An error occurred while saving the entity changes.", ex);
            }

        }

        public async Task<bool> Delete(int id)
        {
            var entity= _context.Set<T>().FirstOrDefault(p => p.Id == id);
            if (entity == null) { return false; }
           _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<IQueryable<T>> GetAll(Expression<Func<T,object>>? orders=null,Expression<Func<T,bool>>? filters=null,params string[] includes)
        {
            IQueryable<T> entity =_context.Set<T>();
            
            if (filters is not null)
            {
                entity=entity.Where(filters);
            }
            if (includes is not null)
            {
                for (int i = 0; i < includes.Length; i++)
                {
                 entity=entity.Include(includes[i]);
                }
            }

            if (orders is not null)
            {
                entity = entity.OrderBy(orders);
            }

            return await  Task.FromResult(entity);
        }

        
        public async Task<T> GetEntity(string? id, Expression<Func<T, object>>? orders = null, Expression<Func<T, bool>>? filters = null, params string[]? includes)
        {

           IQueryable<T> entity =  _context.Set<T>();
            
            if (includes is not null)
            {
                for (int i = 0; i < includes.Length; i++)
                {
                    entity = entity.Include(includes[i]);
                }
            }
            if (filters is not null)
            {
                entity=entity.Where(filters);
            }
            if (orders is not null)
            {
                entity=entity.OrderBy(orders);
            }
            
            var data=await entity.FirstOrDefaultAsync(c=>c.Id.ToString()==id);
            return  data;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
