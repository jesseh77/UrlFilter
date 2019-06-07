using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public interface IPropertyExpressionFactory
    {
        Expression CreatePropertyExpression(string segment, ParameterExpression paramExpression);
    }
}