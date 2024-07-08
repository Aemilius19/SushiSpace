using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Core.Helper.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> query, string fieldName, bool descending)
        {
            var entityType = typeof(T);
            var property = entityType.GetProperty(fieldName);
            if (property == null)
            {
                throw new ArgumentException($"Property '{fieldName}' not found on type '{entityType.Name}'");
            }

            var parameter = Expression.Parameter(entityType, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            string methodName = descending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { entityType, property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExpression)
            );

            return query.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
