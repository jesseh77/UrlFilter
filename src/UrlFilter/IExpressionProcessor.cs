using System.Collections.Generic;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter
{
    public interface IExpressionProcessor
    {
        ExpressionCategory ExpressionCategory { get; }
        bool CanProcess(string operand);
        void Process(LinkedList<Token> tokens, ParameterExpression paramExpression);
    }
}
