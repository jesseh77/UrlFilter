using System.Linq.Expressions;

namespace UrlFilter
{
    public interface ILogicalReducer
    {
        Expression ReduceLogical(string queryText, ParameterExpression parameterExpression);
    }
}