using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public interface IPropertyProcessor
    {
        bool CanProcess(string operand, ParameterExpression paramExpression);
        Expression Process(string segment, ParameterExpression paramExpression);
    }
}