using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace UrlFilter
{
    public static class QueryString<T> where T : class
    {        
        public static IEnumerable<T> Process(string queryString, IQueryable<T> data)
        {
            var segments = queryString.Split(' ');
            var paramExpression = Expression.Parameter(typeof(T));
            var prop = typeof(T).GetRuntimeProperty(segments[0]);
            var left = Expression.Property(paramExpression, prop);

            var propValue = FromString(segments[2]);
            var right = Expression.Constant(propValue);
            var expression = Expression.Equal(left, right);
            var callExpression = GetCallExpression(expression, paramExpression, data);
            var results = data.Provider.CreateQuery<T>(callExpression).ToList();
            return results;
        }

        private static int FromString(string value)
        {
            int val;
            if (int.TryParse(value, out val))
            {
                return val;
            }
            throw new InvalidCastException($"Pameter {value} is not a valid integer");
        }

        private static MethodCallExpression GetCallExpression(Expression expression, ParameterExpression paramExpression, IQueryable data)
        {
            var callExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] {typeof(T)},
                data.Expression,
                Expression.Lambda<Func<T, bool>>(expression, paramExpression));

            return callExpression;
        }
    }
}
