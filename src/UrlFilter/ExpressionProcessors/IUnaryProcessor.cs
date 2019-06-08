using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public interface IUnaryProcessor
    {
        bool CanProcess(string comparisonType);
        Expression Process(string comparisonType, Expression expression);
    }
}