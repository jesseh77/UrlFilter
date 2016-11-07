using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace UrlFilter
{
    public static class QueryString<T> where T : class
    {        
        public static Expression Process(string queryString)
        {
            var segments = queryString.Split(' ');
            var paramExpression = Expression.Parameter(typeof(T));
            var left = Expression.Property(paramExpression, typeof(T).GetRuntimeProperty(segments[0]));
            var right = Expression.Constant(segments[2]);
            return Expression.Equal(left, right);
        }
    }
}
