using System.Linq.Expressions;

namespace UrlFilter
{
    public interface INotNullExpressionProcessor
    {
        Expression NotNullPropertyExpression(string segment, ParameterExpression paramExpression);
    }
}