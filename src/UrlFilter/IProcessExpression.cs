using System.Collections.Generic;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter
{
    public interface IProcessExpression
    {
        ExpressionCategory ExpressionCategory { get; }
        bool CanProcess(string operand, ParameterExpression parameterExpression);
        void Process(LinkedList<Token> tokens, ParameterExpression paramExpression);
    }
}
