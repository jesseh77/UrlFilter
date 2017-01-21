using System;
using System.Linq.Expressions;

namespace UrlFilter
{
    public interface IFilterExpression
    {
        Expression<Func<T, bool>> FromString<T>(string queryString) where T : class;

        Expression<Func<T, bool>> FromString<T>(string queryString, Expression<Func<T, bool>> expression)
            where T : class;
    }
}