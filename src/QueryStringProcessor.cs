using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UrlFilter
{
    public static class QueryString<T> where T : class
    {        
        public static Expression<Func<T,bool>> GetWhereExpression(string queryString)
        {
            var segments = queryString.Split(' ');
            var paramExpression = Expression.Parameter(typeof(T));

            var prop = GetPropertyInfo(segments[0]);

            var left = Expression.Property(paramExpression, prop);

            var propValue = FromString(segments[2]);
            var right = Expression.Constant(propValue);
            var expression = ExpressionOperators.OperatorExpression(segments[1], left, right);

            return Expression.Lambda<Func<T, bool>>(expression, paramExpression);
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            return typeof(T).GetRuntimeProperties()
                .Single(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
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
    }
}
