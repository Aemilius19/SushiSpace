using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Helper.Extensions
{
    public static class FilterExtensions
    {
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                var entityType = typeof(T);
                var property = entityType.GetProperty(filter.Key);
                if (property == null)
                {
                    throw new ArgumentException($"Property '{filter.Key}' not found on type '{entityType.Name}'");
                }

                var parameter = Expression.Parameter(entityType, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var filterValue = Expression.Constant(filter.Value);

                Expression comparison;
                if (property.PropertyType == typeof(string))
                {
                    var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    comparison = Expression.Call(propertyAccess, method, filterValue);
                }
                else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(int))
                {
                    comparison = Expression.GreaterThanOrEqual(propertyAccess, filterValue);
                }
                else
                {
                    comparison = Expression.Equal(propertyAccess, filterValue);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                query = query.Where(lambda);
            }

            return query;
        }
    }
}

