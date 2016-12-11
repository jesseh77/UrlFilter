using System;
using System.Linq.Expressions;

namespace UrlFilter
{
    public interface IFilterExpression
    {
        Expression<Func<T, bool>> FromString<T>(string queryString) where T : class;
    }
}