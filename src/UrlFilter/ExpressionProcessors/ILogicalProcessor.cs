using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public interface ILogicalProcessor
    {
        bool CanProcess(string comparisonType);
        Expression Process(string comparisonType, Expression leftExpression, Expression rightExpression);
    }
}