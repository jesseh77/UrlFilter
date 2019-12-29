using System.Linq.Expressions;

namespace UrlFilter.ExpressionReducers
{
    public interface IComparisonReducer
    {
        Expression ReduceComparison(string leftValue, string comparisonOperator, string rightValue, ParameterExpression paramExpression);
    }
}