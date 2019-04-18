using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public interface IUnaryProcessor
    {
        Expression Process(string comparisonType, Expression expression);
    }
}